using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMenuButton : MonoBehaviour
{
    /// <summary>
    /// 
    /// </summary>
    public void OnClick()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
