using System.Collections;
using UnityEngine;

public class AIPlayer : MonoBehaviour
{
    CircleSpawner _circleSpawner;
    Player _player;
    Timer _drawTimer;
    Vector3 _newPos;
    int _circleLayerMask;

    public void Initialize(Player player)
    {
        _player = player;
        _circleSpawner = gameObject.AddComponent<CircleSpawner>();
        _drawTimer = TimersPool.Instance.Pool.Get();
        _circleLayerMask = (1 << StaticValues.CircleLayer) | (1 << StaticValues.ObstacleLayer) | (1 << StaticValues.StickmanLayer);
    }
    public void PlayTurn()
    {
        IEnumerator draw()
        {
            
            yield return null;
            Collider[] results = new Collider[2];

            _newPos = GetPositionInWorld();
            float holdTime = Random.Range(0.7f, 1.75f);
            int num = Physics.OverlapSphereNonAlloc(_newPos, 1, results, _circleLayerMask);
            while(num > 0)
            {
                yield return new WaitForEndOfFrame();
                _newPos = GetPositionInWorld();
                num = Physics.OverlapSphereNonAlloc(_newPos, 1, results, _circleLayerMask);
            }
            _circleSpawner.GrowCircle(_player.number, _newPos);
            float normalizedTime = 0;
            while (normalizedTime <= 1f)
            {
                normalizedTime += Time.deltaTime / holdTime;
                yield return null;
            }
            _circleSpawner.StopGrowing();
            _drawTimer.Run();
        }
        StartCoroutine(draw());
    }
    private Vector3 GetPositionInWorld()
    {
        return new Vector3(
            Random.Range(WorldManager.LeftTopCorner.x + 2, WorldManager.RightBottomCorner.x - 2),
            -0.08f,
            Random.Range(WorldManager.RightBottomCorner.z + 2, WorldManager.LeftTopCorner.z - 2)
        );
    }
}