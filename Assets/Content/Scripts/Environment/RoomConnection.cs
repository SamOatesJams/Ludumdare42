using UnityEngine;

public class RoomConnection : MonoBehaviour
{
    public enum DirectionType
    {
        North,
        East,
        South,
        West
    }

    /// <summary>
    /// The direction of the connector.
    /// </summary>
    public DirectionType Direction;
    
    #region Editor Debug

#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.5f);
        Gizmos.DrawSphere(transform.position + (transform.forward * 0.5f), 0.1f);
    }
#endif

    #endregion
}
