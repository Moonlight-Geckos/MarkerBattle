using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleSystemController : MonoBehaviour
{
    protected ParticleSystem.MainModule _ps;
    protected IDisposable _disposable;
    public void Initialize(Color color) => _ps.startColor = color;
    private void Awake()
    {
        var system = GetComponent<ParticleSystem>().main;
        system.stopAction = ParticleSystemStopAction.Callback;
        _ps = GetComponent<ParticleSystem>().main;
        _disposable = GetComponent<IDisposable>();
    }
    void OnParticleSystemStopped()
    {
        if(gameObject.activeSelf)
            _disposable?.Dispose();
    }
}