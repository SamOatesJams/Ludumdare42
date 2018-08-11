using UnityEngine;

public class DoorConnector : BaseConnector
{
    /// <summary>
    /// 
    /// </summary>
    public MeshRenderer Marker;

    /// <summary>
    /// 
    /// </summary>
    public Light UnlockLight;

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
    private bool m_isLocked;

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

        Marker.sharedMaterial = new Material(Marker.sharedMaterial);

        UnlockDoor(false);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="unlock"></param>
    public void UnlockDoor(bool unlock)
    {
        m_isLocked = !unlock;

        var color = unlock ? Color.green : Color.red;
        Marker.sharedMaterial.color = color;
        Marker.sharedMaterial.SetColor("_EmissionColor", color);
        UnlockLight.color = color;
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

        if (other.GetComponentInParent<PlayerController>() == null)
        {
            return;
        }

        m_animator.SetBool(m_parameterHash, !m_isLocked);
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

        if (other.GetComponentInParent<PlayerController>() == null)
        {
            return;
        }

        m_animator.SetBool(m_parameterHash, false);
    }
}
