using System.Collections.Generic;
using UnityEngine;

public class Circle : MonoBehaviour
{
    private Player _ownerPlayer;
    private Material _material;
    private IDisposable _disposable;

    private bool _active;
    private float _dis;
    private Color tranColor;
    private Flag _flag;
    private Marker _marker;

    List<Circle> _newIntersectedCircles;
    public Player OwnerPlayer
    {
        get { return _ownerPlayer; }
    }
    public float Radius
    {
        get { return 0.5f * transform.localScale.x; }
    }
    public bool Settled
    {
        get { return _active; }
    }
    private void Awake()
    {
        _material = GetComponent<Renderer>().material;
        _newIntersectedCircles = new List<Circle>();
    }
    private void OnTriggerEnter(Collider other)
    {
        Circle circle;
        Stickman stickman;
        if (other.TryGetComponent(out circle))
        { 
            if(circle.OwnerPlayer.number == OwnerPlayer.number)
            {
                if (!_active)
                {
                    _newIntersectedCircles.Add(circle);
                }
            }
        }
        else if(other.TryGetComponent(out stickman))
        {

        }
        else
        {
            SettleDown();
        }
    }
    public void Initialize(Player owner, Vector3 position)
    {
        _active = false;
        _ownerPlayer = owner;
        transform.position = position;
        transform.localScale = Vector3.one * 0.1f;

        tranColor = _ownerPlayer.mainColor;
        tranColor.a = 0.7f;
        _material.color = tranColor;

        _newIntersectedCircles.Clear();
        _marker = PoolsPool.Instance.MarkersPool.Pool.Get();
        _marker.Initialize(this);
    }
    public void Dispose()
    {
        if (_disposable == null)
            _disposable = GetComponent<IDisposable>();

        _disposable.Dispose();
        _marker?.Dispose();
    }
    private void AddIntersection(Circle circle)
    {
        _dis = Vector3.Distance(circle.transform.position, transform.position);
        if (_dis < circle.Radius - Radius)
        {
            Dispose();
        }
    }
    private void SetupIntersections()
    {
        foreach (Circle circle in _newIntersectedCircles)
        {
            circle.AddIntersection(this);
            circle._flag.Initialize(this);
            _flag = circle._flag;
        }
        _newIntersectedCircles.Clear();
    }
    public bool Grow()
    {
        if (_active)
            return false;
        transform.localScale = transform.localScale + (Vector3.one * 0.02f);
        return true;
    }
    public void SettleDown()
    {
        if (!gameObject.activeSelf || _active)
            return;

        if (transform.localScale.x <= 0.7f)
        {
            Dispose();
            return;
        }
        _active = true;
        _marker?.Dispose();
        _material.color = _ownerPlayer.mainColor;
        if (_newIntersectedCircles.Count > 0)
            SetupIntersections();
        else
        {
            _flag = PoolsPool.Instance.FlagsPool.Pool.Get();
            _flag.Initialize(this);
        }
    }
}