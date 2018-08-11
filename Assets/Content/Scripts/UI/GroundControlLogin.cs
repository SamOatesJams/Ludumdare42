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
        int accessCode;
        if (!int.TryParse(AccessCodeField.text, out accessCode))
        {
            AccessCodeField.text = "";
            InvalidCodeText.enabled = true;
            return;
        }

        GameSession.GetInstance().Seed = accessCode;
        GameSession.GetInstance().GameMode = GameSession.GameModeType.GroundControl;

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
