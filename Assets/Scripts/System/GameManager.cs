using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Serialized

    #endregion

    #region Private

    private static GameManager _instance;

    private static float _timeSinceStarted = 0;

    private static bool _started;
    private static bool _finished;
    private static Camera _camera;

    #endregion

    // Sound effect stuff
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip[] audioClips = new AudioClip[4];

    static public GameManager Instance
    {
        get { return _instance; }
    }
    public float TimeSinceStarted
    {
        get { return _timeSinceStarted; }
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
            _finished = false;
            _started = false;

            Application.targetFrameRate = 60;
            EventsPool.ClearPoolsEvent.Invoke();
            EventsPool.GameStartedEvent.AddListener(StartGame);
            EventsPool.GameFinishedEvent.AddListener(FinishGame);
        }
    }
    private void Start()
    {
        EventsPool.UpdateUIEvent.Invoke();
        audioSource = GetComponent<AudioSource>();
    }
    private void StartGame()
    {
        _started = true;
    }
    private void FinishGame(bool w)
    {
        _finished = true;
        EventsPool.ClearPoolsEvent.Invoke();
    }

    public void PlayDeathSound()
    {
        int rand = UnityEngine.Random.Range(0, 4);
        AudioClip clip = audioClips[rand];
        audioSource.PlayOneShot(clip);
    }
}