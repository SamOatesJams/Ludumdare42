using UnityEngine;

public class DoorConnector : BaseConnector
{
    public enum DoorType
    {
        Normal,
        Puzzle
    }

    /// <summary>
    /// 
    /// </summary>
    public MeshRenderer Marker;

    /// <summary>
    /// 
    /// </summary>
    public Light UnlockLight;

    /// <summary>
    /// 
    /// </summary>
    public DoorType Type;

    /// <summary>
    /// 
    /// </summary>
    public bool IsLocked { get; private set; }

    /// <summary>
    /// The animator component of the door
    /// </summary>
    private Animator m_animator;

    /// <summary>
    /// 
    /// </summary>
    private readonly int m_parameterHash = Animator.StringToHash("isOpen");

    /// <summary>
    /// 
    /// </summary>
    private BasePuzzle m_puzzle;

    /// <summary>
    /// 
    /// </summary>
    private void Awake()
    {
        m_animator = GetComponentInChildren<Animator>();
        if (m_animator == null)
        {
            Debug.LogError($"Failed to find an animator component on the door '{name}'.");
            return;
        }

        if (Type == DoorType.Puzzle)
        {
            m_puzzle = GetComponentInChildren<BasePuzzle>();
            if (m_puzzle == null)
            {
                Debug.LogError($"Failed to find puzzle on the puzzle door '{name}'.");
                return;
            }

            m_puzzle.gameObject.SetActive(false);
        }

        Marker.sharedMaterial = new Material(Marker.sharedMaterial);
        UnlockDoor(false);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="unlock"></param>
    public void UnlockDoor(bool unlock)
    {
        IsLocked = !unlock;

        var color = unlock ? Color.green : Color.red;
        Marker.sharedMaterial.color = color;
        Marker.sharedMaterial.SetColor("_EmissionColor", color);
        UnlockLight.color = color;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="open"></param>
    /// <param name="playerController"></param>
    public void OpenDoor(bool open, PlayerController playerController)
    {
        m_animator.SetBool(m_parameterHash, open);
        if (open)
        {
            GameSession.GetInstance().DoorOpenAudio?.Play();

            if (m_puzzle != null)
            {
                playerController.UnlockMouse(false);
                m_puzzle.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    public void OnTriggerEnter(Collider other)
    {
        if (m_animator == null)
        {
            return;
        }

        var playerController = other.GetComponentInParent<PlayerController>();
        if (playerController == null)
        {
            return;
        }

        var gameSession = GameSession.GetInstance();
        if (gameSession.GetTimeRemaining() == null)
        {
            gameSession.StartTimer();
            var playerUi = playerController.GetComponentInChildren<SpaceStationPlayerUI>();
            if (playerUi?.TimerText != null)
            {
                playerUi.TimerPanel.gameObject.SetActive(true);
            }
        }
        
        if (!IsLocked)
        {
            OpenDoor(true, playerController);
            return;
        }

        if (m_puzzle != null)
        {
            playerController.UnlockMouse(true);
            m_puzzle.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    public void OnTriggerExit(Collider other)
    {
        if (m_animator == null)
        {
            return;
        }

        var playerController = other.GetComponentInParent<PlayerController>();
        if (playerController == null)
        {
            return;
        }

        OpenDoor(false, playerController);

        if (m_puzzle != null)
        {
            playerController.UnlockMouse(false);
            m_puzzle.gameObject.SetActive(false);
        }
    }
}
