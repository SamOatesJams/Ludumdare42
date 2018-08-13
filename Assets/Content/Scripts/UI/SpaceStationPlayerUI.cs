using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpaceStationPlayerUI : MonoBehaviour
{
    /// <summary>
    /// 
    /// </summary>
    public UnityEngine.UI.Image FadePanel;
    
    /// <summary>
    /// 
    /// </summary>
    public UnityEngine.UI.Image TimerPanel;

    /// <summary>
    /// 
    /// </summary>
    public UnityEngine.UI.Text TimerText;

    /// <summary>
    /// 
    /// </summary>
    private bool m_fadingOut;

    /// <summary>
    /// 
    /// </summary>
    public void FixedUpdate()
    {
        if (TimerText != null)
        {
            var gameSession = GameSession.GetInstance();
            var secondsRemaining = gameSession.GetTimeRemaining();
            if (secondsRemaining == null)
            {
                gameSession.BackgroundMusic.pitch = 1.0f;
                return;
            }

            var timeRemaining = TimeSpan.FromSeconds(Math.Max(secondsRemaining.Value, 0.0f));
            TimerText.text = $"{timeRemaining.Minutes:D2}:{timeRemaining.Seconds:D2}:{timeRemaining.Milliseconds.ToString("D2").Substring(0, 2)}";

            if (secondsRemaining <= 30.0f)
            {
                var musicSpeed = Mathf.Lerp(1.0f, 4.0f, 1.0f - (secondsRemaining.Value / 30.0f));
                gameSession.BackgroundMusic.pitch = musicSpeed;
            }

            if (secondsRemaining <= 0)
            {
                FadeOut(0.0f, () =>
                {
                    GetComponentInParent<PlayerController>()?.UnlockMouse(true);
                    gameSession.BackgroundMusic.pitch = 1.0f;
                    SceneManager.LoadScene("MainMenu");
                });
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void FadeOut(float delay, Action callback)
    {
        if (m_fadingOut)
        {
            return;
        }

        m_fadingOut = true;
        StartCoroutine(FadeOutCoroutine(delay, callback));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator FadeOutCoroutine(float delay, Action callback)
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

        m_fadingOut = false;
        callback?.Invoke();
    }
}
