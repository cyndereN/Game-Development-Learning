// Entity Model 发送给 Element View 的消息 

private readonly Queue<EntityMsg> _msgQueue = new Queue<EntityMsg>();

public struct EntityMsg {
    public EntityMsgId Id;
    public int Duration;
    public IEntityMsgData Data;
}


public interface IEntityMsgData {
    
}

public class EntityMsgAlignView : IEntityMsgData
{
    public int Row;
    public int Col;
}


public bool HasMsg() {
    return _msgQueue.Count > 0;
}

public EntityMsg ReceiveMsg() {
    return _msgQueue.Dequeue();
}

// ---------------
// 发送及调用

public void SendMsg(EntityMsgId id, int duration = 0, IEntityMsgData data = null) {
    _msgQueue.Enqueue(new EntityMsg(){ Id = id, Duration = duration, Data = data});
}



entity.SendMsg(EntityMsgId.FallStop, 0, new EntityMsgAlignView(){Row = entity.Row, Col = entity.Col});

//----------
// 处理消息
public abstract void HandleMsg(ElementView elementView, EntityMsg msg);

widget.HandleMsg(this, msg);

public override void HandleMsg(ElementView elementView, EntityMsg msg)
{
    switch (msg.Id)
    {
        case EntityMsgId.DiagonalFallStart:
        {
            DiagonalFallStart();
            break;
        }
        case EntityMsgId.DiagonalFallStop:
        {
            DiagonalFallStop();
            break;
        }
        case EntityMsgId.FallStop:
        {
            EntityMsgAlignView data = msg.Data as EntityMsgAlignView;
            if (data != null) PlayFallStop(data.Row, data.Col);
            break;
        }
        // ... 
    }
}