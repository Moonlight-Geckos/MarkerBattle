using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    [SerializeField]
    [Range(0.5f, 2f)]
    private float spawnCellSize = 0.5f;

    [SerializeField]
    private GameObject worldEdge;

    private Camera _camera;
    [SerializeField]
    List<KdTree<Targetable>> _trees;
    List<Player> _players;

    private static WorldManager _instance;

    public static WorldManager Instance
    {
        get { return _instance; }
    }
    public static Vector3 RightBottomCorner
    {
        get;
        private set;
    }
    public static Vector3 LeftBottomCorner
    {
        get;
        private set;
    }
    public static Vector3 RightTopCorner
    {
        get;
        private set;
    }
    public static Vector3 LeftTopCorner
    {
        get;
        private set;
    }
    public float SpawnCellSize
    {
        get { return spawnCellSize; }
    }
    public List<KdTree<Targetable>> Trees
    {
        get { return _trees; }
    }
    void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(this);
            return;
        }
        _instance = this;
        _camera = Camera.main;
        _trees = new List<KdTree<Targetable>>();

        _players = DataHolder.Instance.Players;
        for (int i = 0; i < _players.Count; i++)
        {
            _players[i]._targetsTree = new KdTree<Targetable>();
            _trees.Add(_players[i]._targetsTree);
        }

        SetEdges();
    }
    public Targetable ClosestTarget(Stickman stickman)
    {
        var target = _trees[stickman.PlayerOwner.number].FindClosest(stickman.transform.position);
        int num = 5;
        while(num > 0 && target != null && !target.gameObject.activeSelf)
        {
            _trees[stickman.PlayerOwner.number].RemoveAll((x) => x.Equals(target));
            target = _trees[stickman.PlayerOwner.number].FindClosest(stickman.transform.position);
            num--;
        }
        return target;
    }
    public void AddTarget(Targetable target)
    {
        for (int i = 0; i < _players.Count; i++)
        {
            if (i != target.PlayerOwner.number)
                _trees[i].Add(target);
        }
    }
    public void RemoveTarget(Targetable target)
    {
        for (int i = 0; i < _players.Count; i++)
        {
            if (i != target.PlayerOwner.number)
            {
                _trees[i].RemoveAll((x) => x.Equals(target));
                if (_trees[i].Count == 0)
                {
                    EventsPool.NoMoreTargetsEvent.Invoke(i);
                }
            }
        }
    }
    private void Update()
    {
        foreach(var tree in _trees)
        {
            tree.UpdatePositions();
        }
    }
    private void SetEdges()
    {
        LeftBottomCorner = MathHelper.GetPointAtHeight(_camera.ViewportPointToRay(new Vector3(0, 0, 0)), 0);
        LeftTopCorner = MathHelper.GetPointAtHeight(_camera.ViewportPointToRay(new Vector3(0, 1, 0)), 0);
        RightTopCorner = MathHelper.GetPointAtHeight(_camera.ViewportPointToRay(new Vector3(1, 1, 0)), 0);
        RightBottomCorner = MathHelper.GetPointAtHeight(_camera.ViewportPointToRay(new Vector3(1, 0, 0)), 0);


        var edgesParent = new GameObject();

        var topEdge = Instantiate(worldEdge);
        topEdge.transform.position = (RightTopCorner + LeftTopCorner) / 2;
        topEdge.transform.localScale = new Vector3((RightTopCorner - LeftTopCorner).magnitude, 1, 0.01f);

        var bottomEdge = Instantiate(worldEdge);
        bottomEdge.transform.position = (LeftBottomCorner + RightBottomCorner) / 2;
        bottomEdge.transform.localScale = new Vector3((LeftBottomCorner - RightBottomCorner).magnitude, 1, 0.01f);

        var rightEdge = Instantiate(worldEdge);
        rightEdge.transform.position = (RightTopCorner + RightBottomCorner) / 2;
        rightEdge.transform.localScale = new Vector3(0.01f, 1, (RightTopCorner - RightBottomCorner).magnitude);
        rightEdge.transform.localRotation = Quaternion.LookRotation((RightTopCorner - RightBottomCorner).normalized, Vector3.up);

        var leftEdge = Instantiate(worldEdge);
        leftEdge.transform.position = (LeftBottomCorner + LeftTopCorner) / 2;
        leftEdge.transform.localScale = new Vector3(0.01f, 1, (LeftBottomCorner - LeftTopCorner).magnitude);
        leftEdge.transform.localRotation = Quaternion.LookRotation((LeftTopCorner - LeftBottomCorner).normalized, Vector3.up);

        leftEdge.transform.parent = edgesParent.transform;
        rightEdge.transform.parent = edgesParent.transform;
        bottomEdge.transform.parent = edgesParent.transform;
        topEdge.transform.parent = edgesParent.transform;
    }
}
