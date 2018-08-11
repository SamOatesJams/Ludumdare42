using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    /// <summary>
    /// The size of a room in meters
    /// </summary>
    private const float c_roomSize = 10.0f;

    /// <summary>
    /// The size of the map to generator
    /// </summary>
    [Range(3, 7)]
    public int GridSize = 3;

    /// <summary>
    /// All potential start rooms we can use
    /// </summary>
    public StartRoom[] StartRoomPrefabs;

    /// <summary>
    /// All potential exit rooms we can use
    /// </summary>
    public ExitRoom[] ExitRoomPrefabs;

    /// <summary>
    /// All potential puzzle rooms we can use
    /// </summary>
    public PuzzleRoom[] PuzzleRoomPrefabs;

    /// <summary>
    /// 
    /// </summary>
    private BaseRoom[,] m_tiles;

    /// <summary>
    /// 
    /// </summary>
    public void SpawnMap()
    {
        if (m_tiles != null)
        {
            DeleteMap();
        }
        
        Random.InitState(314159);

        m_tiles = new BaseRoom[GridSize, GridSize];

        // Spawn a start point.
        var startTile = StartRoomPrefabs[Random.Range(0, StartRoomPrefabs.Length)];
        SpawnRoom(startTile, 0, Random.Range(0, GridSize));

        // Spawn an end point.
        var exitTile = ExitRoomPrefabs[Random.Range(0, ExitRoomPrefabs.Length)];
        SpawnRoom(exitTile, GridSize - 1, Random.Range(0, GridSize));

        // Fill in empty tiles with puzzles.
        for (var x = 0; x < GridSize; ++x)
        {
            for (var y = 0; y < GridSize; ++y)
            {
                if (m_tiles[x, y] != null)
                {
                    continue;
                }

                var puzzleTile = PuzzleRoomPrefabs[Random.Range(0, PuzzleRoomPrefabs.Length)];
                SpawnRoom(puzzleTile, x, y);
            }
        }

        // Resolve connections.
    }

    /// <summary>
    /// 
    /// </summary>
    public void DeleteMap()
    {
        for (var x = 0; x < m_tiles.GetLength(0); ++x)
        {
            for (var y = 0; y < m_tiles.GetLength(1); ++y)
            {
                Destroy(m_tiles[x, y].gameObject);
            }
        }

        m_tiles = null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="template"></param>
    /// <param name="row"></param>
    /// <param name="column"></param>
    private void SpawnRoom(BaseRoom template, int row, int column)
    {
        var newRoom = Instantiate(template, transform);
        newRoom.transform.localPosition = new Vector3(column * c_roomSize, 0.0f, row * c_roomSize);
        newRoom.transform.localRotation = Quaternion.identity;
        newRoom.transform.localScale = Vector3.one;
        m_tiles[row, column] = newRoom;
    }

#region Editor Debug

#if UNITY_EDITOR

    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 120, 30), "Spawn Map"))
        {
            SpawnMap();
        }
    }

#endif

#endregion
}
