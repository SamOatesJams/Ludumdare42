using UnityEngine;

public class TriggerAnimation : MonoBehaviour
{
    /// <summary>
    /// The animator of which the entering this trigger will effect.
    /// </summary>
    public Animator AnimatorToAnimate;

    /// <summary>
    /// The name of the boolean parameter on the animator we wish to toggle.
    /// </summary>
    public string ParameterName;

    /// <summary>
    /// 
    /// </summary>
    private int m_parameterHash;

    private void Start()
    {
        m_parameterHash = Animator.StringToHash(ParameterName);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    public void OnTriggerEnter(Collider other)
    {
        // TODO: Validate 'other' is the player

        AnimatorToAnimate.SetBool(m_parameterHash, true);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    public void OnTriggerExit(Collider other)
    {
        // TODO: Validate 'other' is the player

        AnimatorToAnimate.SetBool(m_parameterHash, false);
    }
}
