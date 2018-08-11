
using UnityEngine;

public class StartRoom : BaseRoom
{
    /// <summary>
    /// 
    /// </summary>
    private PlayerSpawn m_playerSpawn;

    /// <summary>
    /// 
    /// </summary>
    public override void Awake()
    {
        base.Awake();
        m_playerSpawn = GetComponentInChildren<PlayerSpawn>();
        if (m_playerSpawn == null)
        {
            Debug.LogError($"Failed to find a player spawn within the start room '{name}'.");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="playerPrefab"></param>
    public PlayerController SpawnPlayer(PlayerController playerPrefab)
    {
        var player = Instantiate(playerPrefab);
        player.transform.position = m_playerSpawn.transform.position;
        player.transform.rotation = m_playerSpawn.transform.rotation;
        player.transform.localScale = Vector3.one;
        return player;
    }
}
