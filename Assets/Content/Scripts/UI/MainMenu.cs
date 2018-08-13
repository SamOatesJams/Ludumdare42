using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    /// <summary>
    /// 
    /// </summary>
    public void OnEnable()
    {
        var gameSession = GameSession.GetInstance();
        gameSession.BackgroundMusic?.Stop();
        gameSession.MenuMusic?.Play();
    }

    /// <summary>
    /// 
    /// </summary>
    public void FixedUpdate()
    {
        transform.localEulerAngles = new Vector3(Mathf.Sin(Time.time), Time.time, 0.0f);
    }

    /// <summary>
    /// 
    /// </summary>
    public void OnEnterSpaceStationClick()
    {
        var gameSession = GameSession.GetInstance();
        gameSession.Seed = Random.Range(0, int.MaxValue);
        gameSession.GameMode = GameSession.GameModeType.SpaceStation;
        gameSession.ButtonClickAudio?.Play();
        gameSession.MenuMusic?.Stop();

        SceneManager.LoadScene("SpaceStationWorld");
    }

    /// <summary>
    /// 
    /// </summary>
    public void OnEnterJoinGroundClick()
    {
        var gameSession = GameSession.GetInstance();
        gameSession.ButtonClickAudio?.Play();
        gameSession.MenuMusic?.Stop();
        gameSession.GameMode = GameSession.GameModeType.GroundControl;

        SceneManager.LoadScene("GroundControlWorld");
    }
}
