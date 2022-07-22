using UnityEngine;

public class StickmenSpawner : MonoBehaviour
{
    [SerializeField]
    private StickmenPool stickmenPool;

    [SerializeField]
    private float spawnCooldown = 7f;

    private Timer _spawnTimer;
    private WorldManager _worldManager;
    private int _spawnerLayerMask;
    private void Start()
    {
        _spawnerLayerMask = (1 << StaticValues.CircleLayer);
        _worldManager = WorldManager.Instance;
        _spawnTimer = TimersPool.Instance.Pool.Get();
        _spawnTimer.Duration = spawnCooldown;
        _spawnTimer.AddTimerFinishedEventListener(Spawn);
        EventsPool.PlayerStoppedDrawingEvent.AddListener(StartSpawning);
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
                    var stickman = stickmenPool.Pool.Get();
                    stickman.Initialize(circle.OwnerPlayer, 4, position, Vector3.zero);
                }
            }
        }
        _spawnTimer.Run();
    }
}
