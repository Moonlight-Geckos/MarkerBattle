using UnityEngine;

public class CircleSpawner : MonoBehaviour
{
    private static CirclesPool circlesPool;
    private DataHolder _dataHolder;
    private Circle _onGoingCircle;
    private void Update()
    {
        if (_onGoingCircle == null)
            return;

        if (!_onGoingCircle.Grow())
        {
            _onGoingCircle = null;
        }
    }
    public void GrowCircle(int playerNumber, Vector3 position)
    {
        if(_dataHolder == null) _dataHolder = DataHolder.Instance;
        if(circlesPool == null) circlesPool = DataHolder.Instance.CirclesPool;

        _onGoingCircle = circlesPool.Pool.Get();
        _onGoingCircle.Initialize(_dataHolder.Players[playerNumber], position);
    }
    public void StopGrowing()
    {
        _onGoingCircle?.SettleDown();
        _onGoingCircle = null;
    }
    private static void Initialize()
    {
    }
}