using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    [SerializeField]
    [Range(0.5f, 2f)]
    private float spawnCellSize = 0.5f;

    [SerializeField]
    private GameObject worldEdge;

    private Camera _camera;

    private KdTree<Flag>[] _tree;

    private int _playersCount;

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
    void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(this);
            return;
        }
        _instance = this;
        _camera = Camera.main;
        _playersCount = DataHolder.Instance.Players.Count;
        _tree = new KdTree<Flag>[_playersCount];
        for (int i = 0; i < _playersCount; i++)
        {
            _tree[i] = new KdTree<Flag>();
        }

        void RemoveFlag(Flag flag)
        {
            for (int i = 0; i < _playersCount; i++)
            {
                if (i != flag.PlayerOwner.number)
                    _tree[i].RemoveAll((x) => x.Equals(flag));
            }
        }
        void AddFlag(Flag flag)
        {
            for (int i = 0; i < _playersCount; i++)
            {
                if (i != flag.PlayerOwner.number)
                    _tree[i].Add(flag);
            }
        }
        EventsPool.FlagRemovedEvent.AddListener(RemoveFlag);
        EventsPool.FlagPlacedEvent.AddListener(AddFlag);

        SetEdges();
    }
    private void Start()
    {
    }
    public Flag ClosestFlag(Stickman stickman)
    {
        return _tree[stickman.PlayerOwner.number].FindClosest(stickman.transform.position);
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
