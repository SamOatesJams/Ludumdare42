using System.Collections.Generic;
using UnityEngine;

public abstract class BaseRoom : MonoBehaviour
{
    /// <summary>
    /// 
    /// </summary>
    private readonly Dictionary<RoomConnection.DirectionType, RoomConnection> m_connections
        = new Dictionary<RoomConnection.DirectionType, RoomConnection>();

    /// <summary>
    /// 
    /// </summary>
    public void Awake()
    {
        foreach (var connection in GetComponentsInChildren<RoomConnection>())
        {
            if (m_connections.ContainsKey(connection.Direction))
            {
                Debug.LogError($"The room '{name}' has multiple room connections for the direction '{connection.Direction}'.");
            }
            m_connections[connection.Direction] = connection;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public bool HasConnection(RoomConnection.DirectionType direction)
    {
        return m_connections.ContainsKey(direction);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="connector"></param>
    public void SetConnection(RoomConnection.DirectionType direction, BaseConnector connector)
    {
        RoomConnection connection;
        if (!m_connections.TryGetValue(direction, out connection))
        {
            Debug.LogError($"Failed to find a room connection for the direction '{direction}' within the room '{name}'.");
            return;
        }

        if (connection.transform.childCount > 0)
        {
            Debug.LogError($"The connection for direction '{direction}' within the room '{name}' already has a connector.");
            return;
        }

        var newConnection = Instantiate(connector.gameObject, connection.transform);
        newConnection.transform.localPosition = Vector3.zero;
        newConnection.transform.localRotation = Quaternion.identity;
        newConnection.transform.localScale = Vector3.one;
    }
}
