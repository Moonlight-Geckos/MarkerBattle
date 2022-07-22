using UnityEngine;
using System.Collections.Generic;
public class Observer : MonoBehaviour
{
    private static bool _started;
    private static bool _finished;
    private static int _someoneDrawing;

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
            _someoneDrawing = 0;
            void StartGame()
            {
                _worldManager = WorldManager.Instance;
                for (int i = 0; i < _worldManager.Trees.Count; i++)
                {
                    if (_worldManager.Trees[i].Count == 0)
                    {
                        return;
                    }
                }
                _started = true;
                EventsPool.PlayerStoppedDrawingEvent.RemoveListener(StartGame);
            }
            void FinishGame(bool w)
            {
                _finished = true;
                _started = false;
            }
            void PlayerDrawing()
            {
                _someoneDrawing++;
            }
            void PlayerStoppedDrawing()
            {
                if(_someoneDrawing > 0)
                    _someoneDrawing--;
            }
            EventsPool.GameFinishedEvent.AddListener(FinishGame);
            EventsPool.PlayerDrawingEvent.AddListener(PlayerDrawing);
            EventsPool.PlayerStoppedDrawingEvent.AddListener(PlayerStoppedDrawing);
            EventsPool.PlayerStoppedDrawingEvent.AddListener(StartGame);
        }
    }
    private void CheckForFinish()
    {
        if (!_started)
            return;
        if (_someoneDrawing > 0)
            return;
        for (int i = 0; i < _worldManager.Trees.Count; i++)
        {
            if (_worldManager.Trees[i].Count == 0)
            {
                if (i == 0)
                    EventsPool.GameFinishedEvent.Invoke(true);
                else
                    EventsPool.GameFinishedEvent.Invoke(false);
                _started = false;
                _finished = true;
            }
        }
    }
    private void LateUpdate()
    {
        CheckForFinish();
    }
}