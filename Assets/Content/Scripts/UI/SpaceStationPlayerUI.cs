using System.Collections;
using UnityEngine;

public class SpaceStationPlayerUI : MonoBehaviour
{
    /// <summary>
    /// 
    /// </summary>
    public UnityEngine.UI.Image FadePanel;

    /// <summary>
    /// 
    /// </summary>
    public void FadeOut(float delay, System.Action callback)
    {
        StartCoroutine(FadeOutCoroutine(delay, callback));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator FadeOutCoroutine(float delay, System.Action callback)
    {
        yield return new WaitForSeconds(delay);

        var startTime = Time.time;
        var color = FadePanel.color;

        while (Time.time - startTime <= 1.0f)
        {
            color.a = Mathf.Lerp(0.0f, 1.0f, Time.time - startTime);
            FadePanel.color = color;
            yield return new WaitForFixedUpdate();
        }

        callback?.Invoke();
    }
}
