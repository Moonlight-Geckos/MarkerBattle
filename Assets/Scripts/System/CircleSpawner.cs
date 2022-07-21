using UnityEngine;

public class CircleSpawner : MonoBehaviour
{
    private CirclesPool _circlesPool;
    private MarkersPool _markersPool;
    private DataHolder _dataHolder;
    private Circle _onGoingCircle;
    private Marker _marker;

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
        if(_dataHolder == null) 

        if(_circlesPool == null)
        {
            _markersPool = PoolsPool.Instance.MarkersPool;
            _circlesPool = PoolsPool.Instance.CirclesPool;
            _dataHolder = DataHolder.Instance;
        }

        _onGoingCircle = _circlesPool.Pool.Get();
        _onGoingCircle.Initialize(_dataHolder.Players[playerNumber], position);

        _marker = _markersPool.Pool.Get();
        _marker.Initialize(_onGoingCircle);
        EventsPool.PlayerDrawingEvent.Invoke();
    }
    public void StopGrowing()
    {
        _marker?.Dispose();
        _onGoingCircle?.SettleDown();
        _onGoingCircle = null;
        EventsPool.PlayerStoppedDrawingEvent.Invoke();
    }
}