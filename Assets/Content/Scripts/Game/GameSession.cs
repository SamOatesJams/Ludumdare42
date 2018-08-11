using UnityEngine;

public class GameSession : MonoBehaviour
{
    public enum GameModeType
    {
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
    private static GameSession s_instance;

    /// <summary>
    /// 
    /// </summary>
    public int Seed { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool SpawnMap { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public GameModeType GameMode { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int GridSize { get; set; }

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
}
