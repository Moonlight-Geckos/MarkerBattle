using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIParent : MonoBehaviour
{
    [SerializeField]
    private float turnCooldown = 3f;

    private int _lastAI = 0;
    private List<AIPlayer> _aiPlayers;
    private Timer _timer;

    private void Awake()
    {
        EventsPool.PlayerStoppedDrawingEvent.AddListener(SetupAIPlayer);
        EventsPool.NoMoreTargetsEvent.AddListener((int u) => _timer.Stop());
    }
    private void SetupAIPlayer()
    {
        _timer = TimersPool.Instance.Pool.Get();
        _timer.Duration = turnCooldown;
        _timer.AddTimerFinishedEventListener(AITurn);
        List<Player> players = DataHolder.Instance.Players;
        _aiPlayers = new List<AIPlayer>();
        AIPool aiPool = PoolsPool.Instance.AIPool;
        for (int i = 1; i < players.Count; i++)
        {
            var ai = aiPool.Pool.Get();
            ai.Initialize(players[i]);
            _aiPlayers.Add(ai);
        }
        EventsPool.PlayerStoppedDrawingEvent.RemoveListener(SetupAIPlayer);
        _timer.Run();
    }
    private void AITurn()
    {
        if(_lastAI >= _aiPlayers.Count)
            _lastAI = 0;

        IEnumerator playturn()
        {
            yield return null;
            _aiPlayers[_lastAI++].PlayTurn();
            yield return new WaitForSeconds(2f);
            _timer.Run();
        }
        StartCoroutine(playturn());
    }
}
