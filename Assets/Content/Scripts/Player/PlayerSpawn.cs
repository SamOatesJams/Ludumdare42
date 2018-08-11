using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
#region Editor Debug

#if UNITY_EDITOR

    /// <summary>
    /// 
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.5f);
        Gizmos.DrawSphere(transform.position + (transform.forward * 0.5f), 0.1f);
    }

#endif

#endregion
}
