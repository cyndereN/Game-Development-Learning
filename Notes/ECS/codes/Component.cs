// Get component
var bomb = _entityManager.GetComponent<Bomb>(entity);
int fallIndex = world.EntityManager.GetComponentIndex<FallDown>();
var fall = entity.Comps[fallIndex] as FallDown;

public class ElementEntity {
    public IElementComponent[] Comps;
}

public interface IElementComponent {
}

 // 同一类entity共用一个component实例，而不是每个entity生成一个新的component
    // 如果没有数据成员，或者对同样的entity类型，数据值都相同，应该用 IElementComponentShared，节省资源
public interface IElementComponentShared: IElementComponent {
}

namespace Comp {
    // 可以下落
    public class FallDown : IElementComponent {
        public float Speed;
        public float Distance;
        public int FallDelta;
        public int StartFallingRow;
        public bool IsFalling;
        public int CurRow;
    }
}

public T GetComponent<T>(ElementEntity entity) where T: class, IElementComponent {
    int compIndex = _factory.GetComponentIndex(typeof(T));
    if (compIndex < 0)
        return null;
    IElementComponent comp;
    return entity.Comps[compIndex] as T;
}


public int GetComponentIndex<T>() where T : class, IElementComponent {
    return _factory.GetComponentIndex(typeof(T));
}
public partial class ElementEntityFactory {
    //每定义一个entity的component，查询是否在_compTypes定义过，如果没有就添加定义
    private int RegisterComponentIndex(Type compType) {
        int index;
        if (_compIndexMap.TryGetValue(compType, out index))
            return index;
        _compTypes.Add(compType);

        index = _compTypes.Count - 1;
        _compIndexMap[compType] = index; 

        return index;
    }

    //component在entity数组中的下标
    public int GetComponentIndex(Type compType) {
        int index;
        if (!_compIndexMap.TryGetValue(compType, out index)) {
            index = -1;
        }
        return index;
    }
}

// 获取所有拥有该组件的实体
EntityManager.ForEachEntity<Comp.FallDown>((entity, fall) => {
    if (fall.IsFalling) {
        _toTickList.Add(entity);
        return;
    }
});

public void ForEachEntity<T>(Action<ElementEntity, T> callback) 
    where T: class, IElementComponent 
{
    int compIndex = _factory.GetComponentIndex(typeof(T));
    if (compIndex < 0)
        return;
    for (int i = 0; i < _entities.Count; i++) {
        var entity = _entities[i];
        if (entity.Comps[compIndex] != null)
            callback(entity, entity.Comps[compIndex] as T);
    }
}