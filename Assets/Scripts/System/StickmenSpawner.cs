using UnityEngine;

public class StickmenSpawner : MonoBehaviour
{
    [SerializeField]
    private StickmenPool stickmenPool;

    private Timer _spawnTimer;
    private WorldManager _worldManager;
    private void Start()
    {
        _worldManager = WorldManager.Instance;
        _spawnTimer = TimersPool.Instance.Pool.Get();
        _spawnTimer.Duration = 7f;
        _spawnTimer.AddTimerFinishedEventListener(Spawn);
        _spawnTimer.Run();
    }
    private void Spawn()
    {
        Vector3 position = Vector3.zero;
        position.y = 0.1f;
        Collider[] results = new Collider[2];
        int num;
        for(float x = WorldManager.LeftTopCorner.x + _worldManager.SpawnCellSize; x < WorldManager.RightTopCorner.x; x += _worldManager.SpawnCellSize)
        {
            for (float z = WorldManager.LeftBottomCorner.z + _worldManager.SpawnCellSize; z < WorldManager.RightTopCorner.z; z += _worldManager.SpawnCellSize)
            {
                position.x = x;
                position.z = z;
                num = Physics.OverlapSphereNonAlloc(position, 0.01f, results, StaticValues.SpawnerLayer);
                if(num > 0)
                {
                    var circle = results[0].GetComponent<Circle>();
                    if (!circle.Active)
                        continue;
                    var stickamn = stickmenPool.Pool.Get();
                    stickamn.transform.position = position;
                    stickamn.Initialize(circle.OwnerPlayer, 6, Vector3.zero);
                }
            }
        }

        _spawnTimer.Run();
    }
}
