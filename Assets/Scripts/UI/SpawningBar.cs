using UnityEngine;

public class SpawningBar : MonoBehaviour
{
    private Timer _spawningTimer;
    private ProgressBar _progressBar;
    private void Awake()
    {
        _progressBar = GetComponentInChildren<ProgressBar>();
    }
    private void Update()
    {
        if(_spawningTimer == null)
        {
            _spawningTimer = StickmenSpawner.SpawningTimer;
            _progressBar.UpdateValue(0);
        }
        else
        {
            _progressBar.UpdateValue(_spawningTimer.Elapsed / _spawningTimer.Duration);
        }
    }
}