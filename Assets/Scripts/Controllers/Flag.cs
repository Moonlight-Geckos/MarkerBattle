using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Flag : Targetable
{
    private Material _material;
    private IDisposable _disposable;
    private HashSet<Circle> _circles;
    override public Player PlayerOwner
    {
        get { return _circles.Count > 0 ? _circles.First().OwnerPlayer : null; }
    }
    public void Initialize(Circle circle)
    {
        if( _material == null)
        {
            foreach (var mat in GetComponentInChildren<Renderer>().materials)
            {
                if (mat.name.Contains("Cust")) {
                    _material = mat;
                    break;
                }
            }

            _circles = new HashSet<Circle>();
        }
        _circles?.Clear();
        _circles.Add(circle);

        SetColor(circle.OwnerPlayer.mainColor);
        SettlePosition();

        EventsPool.FlagPlacedEvent.Invoke(this);
    }
    public void SettlePosition()
    {
        Vector3 pos = Vector3.zero;
        foreach(var c in _circles)
        {
            pos += c.transform.position;
        }
        pos /= _circles.Count;

        //Calculate closest circle
        float minDis = float.MaxValue;
        Vector3 desiredPos = Vector3.zero;
        foreach(var c in _circles)
        {
            float d = Vector3.Distance(c.transform.position, pos);
            if(d < minDis)
            {
                minDis = d;
                desiredPos = c.transform.position + (pos - c.transform.position).normalized * c.Radius / 2f;
            }
        }
        transform.position = desiredPos;
    }
    public void AddCircle(Circle circle)
    {
        int cnt = _circles.Count;
        _circles.Add(circle);
    }
    public void Dispose()
    {
        if (_disposable == null)
            _disposable = GetComponent<IDisposable>();
        if (gameObject.activeSelf)
        {
            _disposable.Dispose();
            EventsPool.FlagRemovedEvent.Invoke(this);
        }
    }
    public void Occupy(Stickman stick)
    {
        EventsPool.FlagRemovedEvent.Invoke(this);
        foreach(var c in _circles)
        {
            c.Occupy(stick.PlayerOwner);
        }
        SetColor(stick.PlayerOwner.mainColor);
        EventsPool.FlagPlacedEvent.Invoke(this);
    }
    private void SetColor(Color cc)
    {
        _material.color = new Color(cc.r - 0.1f, cc.g - 0.1f, cc.b - 0.1f);
    }

    public override void GetHit(Stickman stick)
    {
        Occupy(stick);
    }
}
