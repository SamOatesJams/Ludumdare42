using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

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
    public DoorConnector[] DoorConnectionPrefabs;

    /// <summary>
    /// 
    /// </summary>
    public WallConnector[] WallConnectionPrefabs;

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
        var startTile = RandomArrayItem(StartRoomPrefabs);
        SpawnRoom(startTile, 0, Random.Range(0, GridSize));

        // Spawn an end point.
        var exitTile = RandomArrayItem(ExitRoomPrefabs);
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

                var puzzleTile = RandomArrayItem(PuzzleRoomPrefabs);
                SpawnRoom(puzzleTile, x, y);
            }
        }

        // Resolve connections.
        ResolveRoomConnections(startTile, exitTile);
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
    /// <param name="startRoom"></param>
    /// <param name="exitRoom"></param>
    private void ResolveRoomConnections(StartRoom startRoom, ExitRoom exitRoom)
    {
        var directions = Enum.GetValues(typeof(RoomConnection.DirectionType))
            .Cast<RoomConnection.DirectionType>().ToArray();

        // Put walls around the edge of the map
        for (var x = 0; x < GridSize; ++x)
        {
            for (var y = 0; y < GridSize; ++y)
            {
                foreach (var direction in directions)
                {
                    var room = GetRoom(x, y);
                    if (!room.HasConnection(direction))
                    {
                        continue;
                    }

                    var adjacentRoom = GetAdjacentRoom(x, y, direction);

                    // Adjacent room has no connection point at the opposite side, set to a wall.
                    if (adjacentRoom != null && !adjacentRoom.HasConnection(direction.GetOpposite()))
                    {
                        room.SetConnection(direction, RandomArrayItem(WallConnectionPrefabs));
                        continue;
                    }

                    // adjacent it out of bounds, set to a wall.
                    if (adjacentRoom == null)
                    {
                        room.SetConnection(direction, RandomArrayItem(WallConnectionPrefabs));
                        continue;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <param name="items"></param>
    /// <returns></returns>
    private TItem RandomArrayItem<TItem>(TItem[] items)
    {
        return items[Random.Range(0, items.Length)];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="room"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    private BaseRoom GetAdjacentRoom(BaseRoom room, RoomConnection.DirectionType direction)
    {
        for (var x = 0; x < GridSize; ++x)
        {
            for (var y = 0; y < GridSize; ++y)
            {
                if (m_tiles[x, y] == room)
                {
                    return GetAdjacentRoom(x, y, direction);
                }
            }
        }

        return null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="row"></param>
    /// <param name="column"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    private BaseRoom GetAdjacentRoom(int row, int column, RoomConnection.DirectionType direction)
    {
        switch (direction)
        {
            case RoomConnection.DirectionType.North:
                return GetRoom(row + 1, column);
            case RoomConnection.DirectionType.East:
                return GetRoom(row, column + 1);
            case RoomConnection.DirectionType.South:
                return GetRoom(row - 1, column);
            case RoomConnection.DirectionType.West:
                return GetRoom(row, column - 1);
            default:
                Debug.LogError($"Failed to located adjacent room for the direction '{direction}'.");
                return null;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="row"></param>
    /// <param name="column"></param>
    /// <returns></returns>
    private BaseRoom GetRoom(int row, int column)
    {
        if (row < 0 || row >= GridSize)
        {
            return null;
        }

        if (column < 0 || column >= GridSize)
        {
            return null;
        }

        return m_tiles[row, column];
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

    private bool m_requestMapSpawn;

    private void OnGUI()
    {
        if (!m_requestMapSpawn && GUI.Button(new Rect(10, 10, 120, 30), "Spawn Map"))
        {
            m_requestMapSpawn = true;
        }
    }

    private void FixedUpdate()
    {
        if (!m_requestMapSpawn)
        {
            return;
        }

        SpawnMap();
        m_requestMapSpawn = false;
    }

#endif

#endregion
}
