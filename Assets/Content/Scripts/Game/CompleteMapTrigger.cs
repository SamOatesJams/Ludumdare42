using UnityEngine;
using UnityEngine.SceneManagement;

public class CompleteMapTrigger : MonoBehaviour
{
    /// <summary>
    /// 
    /// </summary>
    public Animator EscapePodAnimator;

    /// <summary>
    /// 
    /// </summary>
    public Animator ExitHatchAnimator;

    /// <summary>
    /// 
    /// </summary>
    public Transform ExitCameraMount;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    public void OnTriggerEnter(Collider other)
    {
        var playerController = other.GetComponentInParent<PlayerController>();
        if (playerController == null)
        {
            return;
        }

        EscapePodAnimator.SetBool("flyAway", true);
        ExitHatchAnimator.SetBool("isOpen", true);

        var playerCamera = playerController.GetComponentInChildren<Camera>();
        if (playerCamera == null)
        {
            Debug.LogError($"Failed to find a camera in the children of the player '{playerController.name}'.");
            return;
        }

        playerCamera.transform.SetParent(ExitCameraMount);
        playerCamera.transform.localPosition = Vector3.zero;
        playerCamera.transform.localRotation = Quaternion.identity;
        playerCamera.transform.localScale = Vector3.one;

        var playerUi = playerController.GetComponentInChildren<SpaceStationPlayerUI>();
        if (playerUi == null)
        {
            Debug.LogError($"Failed to located the player ui on the player '{playerController.name}'.");
            return;
        }

        playerUi.FadeOut(3.0f, () =>
        {
            playerController.UnlockMouse(true);
            SceneManager.LoadScene("MainMenu");
        });
    }
}
