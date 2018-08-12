
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class AlphaHuePuzzle : BasePuzzle
{
    private class DisplayInformation
    {
        public Color Color;
        public string Character;
    }

    /// <summary>
    /// 
    /// </summary>
    public UnityEngine.UI.Image[] DisplayPanels;

    /// <summary>
    /// 
    /// </summary>
    private MapGenerator m_mapGenerator;

    /// <summary>
    /// 
    /// </summary>
    private DoorConnector m_door;

    /// <summary>
    /// 
    /// </summary>
    private DisplayInformation[] m_displayCode;

    /// <summary>
    /// 
    /// </summary>
    private int[] m_unlockCode;

    /// <summary>
    /// 
    /// </summary>
    private int m_codeIndex;

    /// <summary>
    /// 
    /// </summary>
    public void Awake()
    {
        m_mapGenerator = GetComponentInParent<MapGenerator>();
        if (m_mapGenerator == null)
        {
            Debug.LogError($"Failed to find the owning map generator for the puzzle '{name}'.");
            return;
        }

        m_door = GetComponentInParent<DoorConnector>();
        if (m_door == null)
        {
            Debug.LogError($"Failed to find the owning door for the puzzle '{name}'.");
            return;
        }

        m_displayCode = new DisplayInformation[DisplayPanels.Length];
        m_unlockCode = new int[DisplayPanels.Length];
        m_codeIndex = 0;
    }

    /// <summary>
    /// 
    /// </summary>
    public void Start()
    {
        SetupPuzzle();
        SetupDisplayPanel();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="button"></param>
    public void OnButtonPress(int button)
    {
        if (button == m_unlockCode[m_codeIndex])
        {
            m_codeIndex++;
            GameSession.GetInstance().ButtonClickAudio.Play();

            if (m_codeIndex >= m_unlockCode.Length)
            {
                var room = m_door.GetComponentInParent<BaseRoom>();
                if (room == null)
                {
                    Debug.LogError($"Failed to find owning room for the door '{m_door.name}'.");
                    return;
                }

                var connection = m_door.GetComponentInParent<RoomConnection>();
                if (connection == null)
                {
                    Debug.LogError($"Failed to find owning room connection for the door '{m_door.name}'.");
                    return;
                }

                m_mapGenerator.UnlockDoor(room, connection.Direction);
                m_mapGenerator.OpenDoor(room, connection.Direction);
            }
            return;
        }

        m_codeIndex = 0;
        GameSession.GetInstance().ErrorAudio.Play();
    }

    /// <summary>
    /// 
    /// </summary>
    private void SetupPuzzle()
    {
        var validColors = new[]
        {
            Color.red,
            Color.green,
            Color.blue,
            Color.yellow
        };

        var validCharacters = new List<string>();
        for (var c = 'A'; c != ('Z' + 1); ++c)
        {
            validCharacters.Add(c.ToString());
        }

        for (var index = 0; index < m_displayCode.Length; ++index)
        {
            m_displayCode[index] = new DisplayInformation
            {
                Color = validColors[Random.Range(0, validColors.Length)],
                Character = validCharacters[Random.Range(0, validCharacters.Count)]
            };

            m_unlockCode[index] = SolveCode(m_displayCode[index]);
        }

#if UNITY_EDITOR
        Debug.Log($"Unlock code for door '{m_door.name}' is: {string.Join("", m_unlockCode)}");
#endif
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    private int SolveCode(DisplayInformation info)
    {
        var vowels = new [] {"A", "E", "I", "O", "U"};
        var isVowel = vowels.Any(vowel => vowel == info.Character);

        var oddNumbers = new[] {1, 3, 5, 7, 9};
        var evenNumbers = new[] { 2, 4, 6, 8 };

        var potentialNumbers = isVowel ? oddNumbers : evenNumbers;

        int potentialCode;
        if (info.Color == Color.red)
        {
            potentialCode = potentialNumbers[0];
        }
        else if (info.Color == Color.green)
        {
            potentialCode = potentialNumbers[potentialNumbers.Length - 1];
        }
        else if (info.Color == Color.blue)
        {
            potentialCode = potentialNumbers[1];
        }
        else if (info.Color == Color.yellow)
        {
            potentialCode = potentialNumbers[potentialNumbers.Length - 1] + 1;
            if (potentialCode > 9)
            {
                potentialCode = 1;
            }
        }
        else
        {
            Debug.LogError($"Unhandled display color '{info.Color}' in the puzzle '{name}'.");
            return 1;
        }

        if (new[] {"S", "A", "M"}.Any(x => x == info.Character))
        {
            potentialCode = potentialCode - 1;
            if (potentialCode < 1)
            {
                potentialCode = 9;
            }
        }

        return potentialCode;
    }

    /// <summary>
    /// 
    /// </summary>
    private void SetupDisplayPanel()
    {
        for (var index = 0; index < m_displayCode.Length; ++index)
        {
            var displayPanelImage = DisplayPanels[index];
            var displayPanelText = displayPanelImage.GetComponentInChildren<UnityEngine.UI.Text>();
            if (displayPanelText == null)
            {
                Debug.LogError($"Failed to find a text component in the children on the display panel '{name}'.");
                continue;
            }

            displayPanelImage.color = m_displayCode[index].Color;
            displayPanelText.text = m_displayCode[index].Character;
        }
    }
}
