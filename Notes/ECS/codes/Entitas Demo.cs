public class PositionComponent : IComponent {
    public int x;
    public int y;
}

public class GameBoardElementComponent : IComponent {}

// Entity can Add, Repleace, Remove;
public static Entity CreateBlocker(this Pool pool, int x, int y) {
    return pool.CreateEntity()
               .IsGameBoardElement(true)
               .AddPosition(x, y)
               .AddResource(Res.Blocker)
}

// Pool is the place where all entities live
// Can Create, Destroy, Getall, Getgroup

_pool.GetGroup(){
    Matcher.AllOf(
        Matcher.GameBoardElementComponent,
        Matcher.Position
    )
};

// Move system

public void Execute() {
    var movables = _pool.GetGroup(
        Matcher.AllOf(
            Matcher.Velocity,
            Matcher.Position
        )
    );

    foreach(var e in movables.GetEntities()){
        var pos = e.position;
        e.ReplacePosition(pos+e.Velocity.value);
    }
}