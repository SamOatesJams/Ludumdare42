using System.Collections.Generic;
using UnityEngine;

public class MuteAudioButton : MonoBehaviour
{
    /// <summary>
    /// 
    /// </summary>
    public UnityEngine.UI.Image SoundEnabledImage;

    /// <summary>
    /// 
    /// </summary>
    public UnityEngine.UI.Image SoundDisabledImage;

    /// <summary>
    /// 
    /// </summary>
    private static readonly Dictionary<string, float> OriginalVolumes = new Dictionary<string, float>();

    /// <summary>
    /// 
    /// </summary>
    private static bool s_isAudioEnabled = true;

    /// <summary>
    /// 
    /// </summary>
    public void OnEnable()
    {
        if (s_isAudioEnabled)
        {
            SoundEnabledImage.enabled = true;
            SoundDisabledImage.enabled = false;
        }
        else
        {
            SoundEnabledImage.enabled = false;
            SoundDisabledImage.enabled = true;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="audioName"></param>
    /// <param name="audioSource"></param>
    private void MuteAudio(string audioName, AudioSource audioSource)
    {
        if (audioSource == null)
        {
            return;
        }

        OriginalVolumes[audioName] = audioSource.volume;
        audioSource.volume = 0.0f;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="audioName"></param>
    /// <param name="audioSource"></param>
    private void EnableAudio(string audioName, AudioSource audioSource)
    {
        if (audioSource == null)
        {
            return;
        }

        float originalVolume;
        if (OriginalVolumes.TryGetValue(audioName, out originalVolume))
        {
            audioSource.volume = originalVolume;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void OnClick()
    {
        var gameSession = GameSession.GetInstance();
        if (gameSession == null)
        {
            return;
        }

        if (SoundEnabledImage.enabled)
        {
            SoundEnabledImage.enabled = false;
            SoundDisabledImage.enabled = true;
            s_isAudioEnabled = false;

            MuteAudio("BackgroundMusic", gameSession.BackgroundMusic);
            MuteAudio("ButtonClickAudio", gameSession.ButtonClickAudio);
            MuteAudio("ErrorAudio", gameSession.ErrorAudio);
            MuteAudio("MenuMusic", gameSession.MenuMusic);
            MuteAudio("DoorOpenAudio", gameSession.DoorOpenAudio);
        }
        else
        {
            SoundEnabledImage.enabled = true;
            SoundDisabledImage.enabled = false;
            s_isAudioEnabled = true;

            EnableAudio("BackgroundMusic", gameSession.BackgroundMusic);
            EnableAudio("ButtonClickAudio", gameSession.ButtonClickAudio);
            EnableAudio("ErrorAudio", gameSession.ErrorAudio);
            EnableAudio("MenuMusic", gameSession.MenuMusic);
            EnableAudio("DoorOpenAudio", gameSession.DoorOpenAudio);
        }
    }
}
