using UnityEngine;

public class GroundControlLogin : MonoBehaviour
{
    /// <summary>
    /// 
    /// </summary>
    public UnityEngine.UI.InputField AccessCodeField;

    /// <summary>
    /// 
    /// </summary>
    public MapGenerator MapGenerator;

    /// <summary>
    /// 
    /// </summary>
    public UnityEngine.UI.Text InvalidCodeText;

    /// <summary>
    /// 
    /// </summary>
    public void OnConnectButtonClick()
    {
        var gameSession = GameSession.GetInstance();

        int accessCode;
        if (!int.TryParse(AccessCodeField.text, out accessCode))
        {
            AccessCodeField.text = "";
            InvalidCodeText.enabled = true;
            gameSession.ErrorAudio.Play();
            return;
        }

        gameSession.Seed = accessCode;
        gameSession.GameMode = GameSession.GameModeType.GroundControl;
        gameSession.ButtonClickAudio.Play();

        MapGenerator.SpawnMap();
        Destroy(gameObject);
    }

    /// <summary>
    /// 
    /// </summary>
    public void OnAccessCodeChanges()
    {
        InvalidCodeText.enabled = false;
    }
}
