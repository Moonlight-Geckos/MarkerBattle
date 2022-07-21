using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolsPool : MonoBehaviour
{
    [SerializeField]
    private FlagsPool flagsPool;

    [SerializeField]
    private MarkersPool markersPool;

    [SerializeField]
    private CirclesPool circlesPool;

    [SerializeField]
    private AIPool aiPool;

    [SerializeField]
    private ParticlesPool splashPool;

    private static PoolsPool _instance;
    public static PoolsPool Instance
    {
        get { return _instance; }
    }
    public FlagsPool FlagsPool
    {
        get { return flagsPool; }
    }
    public MarkersPool MarkersPool
    {
        get { return markersPool; }
    }
    public CirclesPool CirclesPool
    {
        get { return circlesPool; }
    }
    public AIPool AIPool
    {
        get { return aiPool; }
    }
    public ParticlesPool SplashPool
    {
        get { return splashPool; }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this);
        else
        {
            _instance = this;
        }
    }
}
