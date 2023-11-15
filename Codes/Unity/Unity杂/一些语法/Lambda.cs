
public bool Fire => FireCooldown <= 0.0 && math.length(Shoot) > 0.5f;
 
// 等价于
public bool Fire
{
    get { return FireCooldown <= 0.0 && math.length(Shoot) > 0.5f; }
}
  
 
//------------------------------------------------------
 
protected ElementEntityManager EntityManager => EcsWorld.EntityManager;


// ??
diagonalFall.Speed = fallDown?.Speed ?? EraseDefines.FallingStartSpeed;