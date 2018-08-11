using UnityEngine;

public class DoorConnector : BaseConnector
{
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
    private void Start()
    {
        m_animator = GetComponent<Animator>();
        if (m_animator == null)
        {
            Debug.LogError($"Failed to find an animator component on the door '{name}'.");
            return;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    public void OnTriggerEnter(Collider other)
    {
        // TODO: Validate 'other' is the player

        m_animator.SetBool(m_parameterHash, true);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    public void OnTriggerExit(Collider other)
    {
        // TODO: Validate 'other' is the player

        m_animator.SetBool(m_parameterHash, false);
    }
}
