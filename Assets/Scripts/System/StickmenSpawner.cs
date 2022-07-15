using UnityEngine;

public class StickmenSpawner : MonoBehaviour
{
    [SerializeField]
    private StickmenPool stickmenPool;

    [SerializeField]
    [Range(0.5f, 2f)]
    private float spawnCellSize = 0.5f;

    private Timer _spawnTimer;
    private void Start()
    {
        _spawnTimer = TimersPool.Instance.Pool.Get();
        _spawnTimer.Duration = 7f;
        _spawnTimer.AddTimerFinishedEventListener(Spawn);
        _spawnTimer.Run();
    }
    private void Spawn()
    {
        Ray ray;
        Vector3 position = Vector3.zero;
        position.y = 0.1f;
        Collider[] results = new Collider[2];
        int num;
        for(float x = WorldManager.LeftTopCorner.x + spawnCellSize; x < WorldManager.RightTopCorner.x; x += spawnCellSize)
        {
            for (float z = WorldManager.LeftBottomCorner.z + spawnCellSize; z < WorldManager.RightTopCorner.z; z += spawnCellSize)
            {
                position.x = x;
                position.z = z;
                num = Physics.OverlapSphereNonAlloc(position, 0.01f, results, StaticValues.SpawnerLayer);
                if(num > 0)
                {
                    var circle = results[0].GetComponent<Circle>();
                    if (!circle.Settled)
                        continue;
                    var stickamn = stickmenPool.Pool.Get();
                    stickamn.transform.position = position;
                    stickamn.Initialize(circle.OwnerPlayer.mainColor, 4, Vector3.zero);
                }
            }
        }

        _spawnTimer.Run();
    }
}
