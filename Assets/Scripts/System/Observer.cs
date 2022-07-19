using UnityEngine;
using System.Collections.Generic;
public class Observer : MonoBehaviour
{
    private static bool _started;
    private static bool _finished;

    private static Observer _instance;
    private static WorldManager _worldManager;

    public bool Finished
    {
        get { return _finished; }
        set { _finished = value; }
    }
    public bool Started
    {
        get { return _started; }
        set { _started = value; }
    }
    public static Observer Instance
    {
        get { return _instance; }
    }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this);
        }
        else
        {
            _instance = this;
            _started = false;
            _finished = false;
            void StartGame()
            {
                _started = true;
                _worldManager = WorldManager.Instance;
            }
            void FinishGame(bool w)
            {
                _finished = true;
            }
            EventsPool.GameStartedEvent.AddListener(StartGame);
            EventsPool.GameFinishedEvent.AddListener(FinishGame);
        }
    }
}
