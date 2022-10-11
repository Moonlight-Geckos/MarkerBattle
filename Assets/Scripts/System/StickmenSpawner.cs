using UnityEngine;

public class StickmenSpawner : MonoBehaviour
{
    // CREATED SECOND POOL HERE
    // Changed both to public to access them in Stickman script
    public StickmenPool stickmenPool;
    public StickmenPool stickmenPoolEnemy;
    public StickmenPool stickmenPoolEnemy2;

    [SerializeField]
    private float spawnCooldown = 7f;

    private static Timer _spawnTimer;
    private WorldManager _worldManager;
    private int _spawnerLayerMask;

    public static Timer SpawningTimer
    {
        get { return _spawnTimer; }
    }
    private void Start()
    {
        _spawnerLayerMask = (1 << StaticValues.CircleLayer);
        _worldManager = WorldManager.Instance;
        _spawnTimer = TimersPool.Instance.Pool.Get();
        _spawnTimer.Duration = spawnCooldown;
        _spawnTimer.AddTimerFinishedEventListener(Spawn);
        EventsPool.PlayerStoppedDrawingEvent.AddListener(StartSpawning);
        EventsPool.GameFinishedEvent.AddListener(StopSpawning);
    }
    private void StartSpawning()
    {
        var _worldManager = WorldManager.Instance;
        for (int i = 0; i < _worldManager.Trees.Count; i++)
        {
            if (_worldManager.Trees[i].Count == 0)
            {
                return;
            }
        }
        _spawnTimer.Run();
        EventsPool.PlayerStoppedDrawingEvent.RemoveListener(StartSpawning);
    }
    private void StopSpawning(bool w)
    {
        _spawnTimer.Stop();
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
                num = Physics.OverlapSphereNonAlloc(position, 0.01f, results, _spawnerLayerMask);
                if(num > 0)
                {
                    var circle = results[0].GetComponent<Circle>();
                    if (!circle.Active)
                        continue;

                    // CUSTOM CODE HERE TO SPAWN DIFFERENT MODELS
                    if (circle.OwnerPlayer.number == 0)
                    {
                        var stickman = stickmenPool.Pool.Get();
                        stickman.Initialize(circle.OwnerPlayer, 4, position, Vector3.zero);
                    }
                    else if (circle.OwnerPlayer.number == 1)
                    {
                        var stickman = stickmenPoolEnemy.Pool.Get();
                        stickman.Initialize(circle.OwnerPlayer, 4, position, Vector3.zero);
                    }
                    else if(circle.OwnerPlayer.number == 1 && stickmenPoolEnemy2 != null)
                    {
                        var stickman = stickmenPoolEnemy2.Pool.Get();
                        stickman.Initialize(circle.OwnerPlayer, 4, position, Vector3.zero);
                    }

                    //var stickman = stickmenPool.Pool.Get();
                    //stickman.Initialize(circle.OwnerPlayer, 4, position, Vector3.zero);
                }
            }
        }
        _spawnTimer.Run();
    }
}
