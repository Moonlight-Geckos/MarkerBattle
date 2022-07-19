using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Circle : MonoBehaviour
{
    public bool Active;
    public int PlayerOwnerNumber;

    private Player _ownerPlayer;
    private Material _material;
    private IDisposable _disposable;

    private Color tranColor;
    private Flag _flag;

    HashSet<Circle> _newIntersectedCircles;
    HashSet<Circle> _intersectedCircles;
    public Player OwnerPlayer
    {
        get { return _ownerPlayer; }
    }
    public float Radius
    {
        get { return 0.5f * transform.localScale.x; }
    }
    private void Awake()
    {
        _material = GetComponent<Renderer>().material;
        _newIntersectedCircles = new HashSet<Circle>();
        _intersectedCircles = new HashSet<Circle>();
    }
    private void Start()
    {
        if (Active)
        {
            var scale = transform.localScale;
            Initialize(DataHolder.Instance.Players[PlayerOwnerNumber], transform.position);
            transform.localScale = scale;
            SettleDown();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (Active)
            return;
        Circle circle;
        Stickman stickman;
        if (other.TryGetComponent(out circle))
        { 
            if(circle.OwnerPlayer.number == OwnerPlayer.number)
            {
                if (!Active)
                {
                    _newIntersectedCircles.Add(circle);
                }
            }
            else
            {
                Dispose();
            }
        }
        else if(other.TryGetComponent(out stickman))
        {
            if(stickman.PlayerOwner.number != OwnerPlayer.number)
            {
                Dispose();
            }
        }
        else
        {
            SettleDown();
        }
    }
    public void Initialize(Player owner, Vector3 position)
    {
        Active = false;
        _ownerPlayer = owner;
        transform.position = position;
        transform.localScale = Vector3.one * 0.1f;

        tranColor = _ownerPlayer.mainColor;
        tranColor.a = 0.7f;
        SetColor(tranColor);

        _newIntersectedCircles.Clear();
        _intersectedCircles.Clear();
    }
    public void Dispose()
    {
        if (_disposable == null)
            _disposable = GetComponent<IDisposable>();

        _flag = null;
        _disposable.Dispose();
    }
    public void Occupy(Player owner)
    {
        _ownerPlayer = owner;
        SetColor(owner.mainColor);
    }
    public bool Grow()
    {
        if (Active)
            return false;
        transform.localScale = transform.localScale + (Vector3.one * 0.02f);
        return true;
    }
    public void SettleDown()
    {
        if (!gameObject.activeSelf || Active)
            return;

        if (transform.localScale.x <= 0.7f)
        {
            Dispose();
            return;
        }
        Active = true;
        SetColor(_ownerPlayer.mainColor);
        _flag = PoolsPool.Instance.FlagsPool.Pool.Get();
        _flag.Initialize(this);
        if (_newIntersectedCircles.Count > 0)
            SetupIntersections();
    }
    private void SetColor(Color color)
    {
        _material.color = color;
    }
    private void AddIntersection(Circle circle)
    {
        _intersectedCircles.Add(circle);
    }
    private void SetupIntersections()
    {
        //BFS traverse to circles 
        foreach (Circle circle in _newIntersectedCircles)
        {
            _intersectedCircles.Add(circle);
        }
        Queue<Circle> nodes = new Queue<Circle>();
        HashSet<Circle> _visited = new HashSet<Circle>();

        foreach(Circle circle in _intersectedCircles)
            nodes.Enqueue(circle);
        while (nodes.Count > 0)
        {
            Circle circle = nodes.Dequeue();
            if (_visited.Contains(circle))
                continue;

            _visited.Add(circle);

            _flag.AddCircle(circle);
            if(_flag != circle._flag)
                circle._flag.Dispose();
            circle._flag = _flag;

            foreach (var child in circle._intersectedCircles)
                if (!_visited.Contains(child))
                    nodes.Enqueue(child);
        }
        foreach (Circle circle in _newIntersectedCircles)
        {
            circle.AddIntersection(this);
        }
        _flag.SettlePosition();
        _newIntersectedCircles.Clear();
    }
    public override int GetHashCode()
    {
        var u = Math.Round(transform.position.x, 2).ToString() + Math.Round(transform.position.z, 2).ToString();
        u = u.Replace("-", "");
        return int.Parse(u.Replace(".", ""));
    }
    public override bool Equals(object other)
    {
        return other.GetHashCode() == GetHashCode();
    }
}