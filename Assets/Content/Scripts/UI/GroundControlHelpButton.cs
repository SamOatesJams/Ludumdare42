
using UnityEngine;

public class GroundControlHelpButton : MonoBehaviour
{
    /// <summary>
    /// 
    /// </summary>
    public UnityEngine.UI.Image HelpPanel;

    /// <summary>
    /// 
    /// </summary>
    public void Start()
    {
        HelpPanel.gameObject.SetActive(false);
    }

    /// <summary>
    /// 
    /// </summary>
    public void OnClick()
    {
        HelpPanel.gameObject.SetActive(true);
    }

    /// <summary>
    /// 
    /// </summary>
    public void OnCloseHelp()
    {
        HelpPanel.gameObject.SetActive(false);
    }
}
