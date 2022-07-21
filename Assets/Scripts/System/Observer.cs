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
                _started = true;
                _worldManager = WorldManager.Instance;
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
                _someoneDrawing--;
            }
            EventsPool.GameStartedEvent.AddListener(StartGame);
            EventsPool.GameFinishedEvent.AddListener(FinishGame);
            EventsPool.NoMoreTargetsEvent.AddListener(CheckForFinish);
            EventsPool.PlayerDrawingEvent.AddListener(PlayerDrawing);
            EventsPool.PlayerStoppedDrawingEvent.AddListener(PlayerStoppedDrawing);
        }
    }
    private void CheckForFinish(int player)
    {
        if (!_started)
            return;
        if (_someoneDrawing > 0)
            return;
        else
        {
            if (player == 0)
                EventsPool.GameFinishedEvent.Invoke(true);
            else
                EventsPool.GameFinishedEvent.Invoke(false);
            _started = false;
            _finished = true;
        }
    }
}
