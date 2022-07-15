using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolsPool : MonoBehaviour
{
    [SerializeField]
    private FlagsPool flagsPool;

    [SerializeField]
    private MarkersPool markersPool;

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
