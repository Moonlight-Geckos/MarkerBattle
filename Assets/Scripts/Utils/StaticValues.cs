using UnityEngine;

public static class StaticValues
{
    public readonly static int EnemyLayer = LayerMask.NameToLayer("Enemy");
    public readonly static int PlayerLayer = LayerMask.NameToLayer("Player");
    public readonly static int CircleLayer = LayerMask.NameToLayer("Circle");
    public readonly static int SpawnerLayer = LayerMask.NameToLayer("Spawner");
}