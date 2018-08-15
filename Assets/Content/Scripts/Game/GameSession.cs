using UnityEngine;
using Random = UnityEngine.Random;

public class GameSession : MonoBehaviour
{
    public enum GameModeType
    {
        Unknown,
        SpaceStation,
        GroundControl
    }

    /// <summary>
    /// 
    /// </summary>
    public AudioSource ButtonClickAudio;

    /// <summary>
    /// 
    /// </summary>
    public AudioSource ErrorAudio;

    /// <summary>
    /// 
    /// </summary>
    public AudioSource BackgroundMusic;

    /// <summary>
    /// 
    /// </summary>
    public AudioSource MenuMusic;

    /// <summary>
    /// 
    /// </summary>
    public AudioSource DoorOpenAudio;

    /// <summary>
    /// 
    /// </summary>
    private static GameSession s_instance;

    /// <summary>
    /// 
    /// </summary>
    public int Seed { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public GameModeType GameMode { get; set; } = GameModeType.Unknown;

    /// <summary>
    /// 
    /// </summary>
    public int GridSize { get; set; }

    /// <summary>
    /// 
    /// </summary>
    private float? m_timerStartTime;

    /// <summary>
    /// 
    /// </summary>
    public float TimerLength { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    public void Awake()
    {
        if (s_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        s_instance = this;
        DontDestroyOnLoad(gameObject);
        Seed = Random.Range(0, int.MaxValue);
        GridSize = 4;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static GameSession GetInstance()
    {
        if (s_instance == null)
        {
            var instanceGo = new GameObject("GameSession_Singleton");
            return instanceGo.AddComponent<GameSession>();
        }
        return s_instance;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="seconds"></param>
    public void SetTimerLength(int seconds)
    {
        TimerLength = seconds;
        m_timerStartTime = null;
    }

    /// <summary>
    /// 
    /// </summary>
    public void StartTimer()
    {
        m_timerStartTime = Time.time;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public float? GetTimeRemaining()
    {
        if (m_timerStartTime == null)
        {
            return null;
        }

        return TimerLength - (Time.time - m_timerStartTime.Value);
    }
}
