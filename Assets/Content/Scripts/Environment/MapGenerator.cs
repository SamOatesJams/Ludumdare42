using System.Collections.Generic;
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
    /// The seed to use for random map generation.
    /// </summary>
    public int GenerationSeed;

    /// <summary>
    /// The size of the map to generator
    /// </summary>
    [Range(3, 11)]
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
    public PlayerController FirstPersonPlayerPrefab;

#if UNITY_EDITOR
    /// <summary>
    /// 
    /// </summary>
    public GameSession.GameModeType DevMode;
#endif

    /// <summary>
    /// 
    /// </summary>
    private BaseRoom[,] m_tiles;

    /// <summary>
    /// 
    /// </summary>
    private PlayerController m_playerController;

    /// <summary>
    /// 
    /// </summary>
    public void Start()
    {
        var gameSession = GameSession.GetInstance();
        if (gameSession.GameMode == GameSession.GameModeType.Unknown)
        {
            gameSession.GameMode = DevMode;
        }

        if (gameSession.GameMode == GameSession.GameModeType.SpaceStation)
        {
            SpawnMap();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void FixedUpdate()
    {
        if (m_tiles == null || m_playerController == null)
        {
            return;
        }

        var gameSession = GameSession.GetInstance();
        if (gameSession.GameMode != GameSession.GameModeType.SpaceStation)
        {
            return;
        }

        var playerPosition = m_playerController.transform.position;
        var playerRow = (int) (playerPosition.z / c_roomSize);
        var playerColumn = (int)(playerPosition.x / c_roomSize);

        var playerRoom = GetRoom(playerRow, playerColumn);
        if (playerRoom == null)
        {
            return;
        }

        var closestDoor = float.MaxValue;
        var closestRoom = default(BaseRoom);

        foreach (var direction in RoomConnection.Directions)
        {
            if (!playerRoom.HasConnection(direction))
            {
                continue;
            }

            var door = playerRoom.GetConnector(direction) as DoorConnector;
            if (door == null)
            {
                continue;
            }

            var distanceToDoor = Vector3.Distance(door.transform.position, playerPosition);
            if (distanceToDoor >= closestDoor)
            {
                continue;
            }

            var room = GetAdjacentRoom(playerRow, playerColumn, direction);
            if (room == null)
            {
                continue;
            }

            closestDoor = distanceToDoor;
            closestRoom = room;
        }

        if (closestRoom == null)
        {
            return;
        }

        var enabledRooms = new HashSet<BaseRoom>()
        {
            playerRoom,
            closestRoom
        };

        for (var row = 0; row < m_tiles.GetLength(0); ++row)
        {
            for (var column = 0; column < m_tiles.GetLength(1); ++column)
            {
                var room = GetRoom(row, column);
                if (room != null)
                {
                    room.gameObject.SetActive(enabledRooms.Contains(room));
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void SpawnMap()
    {
        var gameSession = GameSession.GetInstance();
        GridSize = gameSession.GridSize;

        if (m_tiles != null)
        {
            DeleteMap();
        }

        Random.InitState(gameSession.Seed);

        m_tiles = new BaseRoom[GridSize, GridSize];

        // Spawn a start point.
        var startTilePrefab = RandomArrayItem(StartRoomPrefabs);
        var startRoom = SpawnRoom(startTilePrefab, 0, Random.Range(0, GridSize));

        // Spawn an end point.
        var exitTilePrefab = RandomArrayItem(ExitRoomPrefabs);
        var exitRoom = SpawnRoom(exitTilePrefab, GridSize - 1, Random.Range(0, GridSize));

        // Fill in empty tiles with puzzles.
        for (var x = 0; x < GridSize; ++x)
        {
            for (var y = 0; y < GridSize; ++y)
            {
                if (GetRoom(x, y) != null)
                {
                    continue;
                }

                var puzzleTile = RandomArrayItem(PuzzleRoomPrefabs);
                SpawnRoom(puzzleTile, x, y);
            }
        }

        // Resolve connections.
        ResolveRoomConnections(startRoom, exitRoom);

        if (gameSession.GameMode == GameSession.GameModeType.SpaceStation)
        {
            // Spawn Player.
            m_playerController = startRoom.SpawnPlayer(FirstPersonPlayerPrefab);
        }

        // Unlock all normal doors
        foreach (var room in m_tiles)
        {
            if (room == null)
            {
                continue;
            }

            foreach (var direction in RoomConnection.Directions)
            {
                if (!room.HasConnection(direction))
                {
                    continue;
                }

                var door = room.GetConnector(direction) as DoorConnector;
                if (door == null)
                {
                    continue;
                }

                if (door.Type == DoorConnector.DoorType.Normal
                    && door.IsLocked)
                {
                    UnlockDoor(room, direction);
                }
            }
        }
        
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
                var room = GetRoom(x, y);
                if (room == null)
                {
                    continue;
                }

                Destroy(room.gameObject);
            }
        }
        m_tiles = null;

        if (m_playerController != null)
        {
            Destroy(m_playerController.gameObject);
            m_playerController = null;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="room"></param>
    /// <param name="direction"></param>
    public void UnlockDoor(BaseRoom room, RoomConnection.DirectionType direction)
    {
        var door = room.GetConnector(direction) as DoorConnector;
        if (door == null)
        {
            return;
        }
        door.UnlockDoor(true);

        var adjecentRoom = GetAdjacentRoom(room, direction);
        var adjacnetDoor = adjecentRoom?.GetConnector(direction.GetOpposite()) as DoorConnector;
        adjacnetDoor?.UnlockDoor(true);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="room"></param>
    /// <param name="direction"></param>
    public void OpenDoor(BaseRoom room, RoomConnection.DirectionType direction)
    {
        var door = room.GetConnector(direction) as DoorConnector;
        if (door == null)
        {
            return;
        }
        door.OpenDoor(true, m_playerController);

        var adjecentRoom = GetAdjacentRoom(room, direction);
        var adjacnetDoor = adjecentRoom?.GetConnector(direction.GetOpposite()) as DoorConnector;
        adjacnetDoor?.OpenDoor(true, m_playerController);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="startRoom"></param>
    /// <param name="exitRoom"></param>
    private void ResolveRoomConnections(StartRoom startRoom, ExitRoom exitRoom)
    {
        // Put walls around the edge of the map
        for (var x = 0; x < GridSize; ++x)
        {
            for (var y = 0; y < GridSize; ++y)
            {
                foreach (var direction in RoomConnection.Directions)
                {
                    var room = GetRoom(x, y);
                    if (room?.HasConnection(direction) != true)
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
                    }
                }
            }
        }

        // Find a route from start to exit.
        var maxPathResolve = (GridSize * GridSize) * RoomConnection.Directions.Length;
        BaseRoom currentRoom = startRoom;
        var roomRoute = new Stack<BaseRoom>();

        while (currentRoom != exitRoom)
        {
            var openDirections = currentRoom.GetFreeConnectionDirections();
            if (!openDirections.Any())
            {
                if (roomRoute.Count == 0)
                {
                    Debug.LogError($"The room '{currentRoom.name}' has no open connections.");
                    break;
                }

                currentRoom = roomRoute.Pop();
                continue;
            }

            var moveDirection = openDirections[Random.Range(0, openDirections.Count)];
            var nextRoom = GetAdjacentRoom(currentRoom, moveDirection);

            var doorPrefabs = DoorConnectionPrefabs;
            if (currentRoom == startRoom)
            {
                doorPrefabs = doorPrefabs.Where(x => x.Type == DoorConnector.DoorType.Normal).ToArray();
            }

            var doorConnection = RandomArrayItem(doorPrefabs);
            currentRoom.SetConnection(moveDirection, doorConnection);
            nextRoom.SetConnection(moveDirection.GetOpposite(), doorConnection);

            roomRoute.Push(currentRoom);
            currentRoom = nextRoom;

            if (--maxPathResolve == 0)
            {
                Debug.LogError("Failed to generate a path from start to finish within the maximum number of moves.");
                break;
            }
        }

        // Fill all open connections with walls.
        for (var x = 0; x < GridSize; ++x)
        {
            for (var y = 0; y < GridSize; ++y)
            {
                var room = GetRoom(x, y);
                if (room == null)
                {
                    continue;
                }

                foreach (var direction in room.GetFreeConnectionDirections())
                {
                    var wallPrefab = RandomArrayItem(WallConnectionPrefabs);
                    room.SetConnection(direction, wallPrefab);

                    var adjacentRoom = GetAdjacentRoom(x, y, direction);
                    if (adjacentRoom?.HasConnection(direction.GetOpposite()) == true)
                    {
                        adjacentRoom.SetConnection(direction.GetOpposite(), wallPrefab);
                    }
                }
            }
        }

        // Remove completely walled rooms
        for (var x = 0; x < GridSize; ++x)
        {
            for (var y = 0; y < GridSize; ++y)
            {
                var room = GetRoom(x, y);
                if (room?.IsBoxedIn() == true)
                {
                    Destroy(room.gameObject);
                    m_tiles[x, y] = null;
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
                if (GetRoom(x, y) == room)
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
        if (row < 0 || row >= m_tiles.GetLength(0))
        {
            return null;
        }

        if (column < 0 || column >= m_tiles.GetLength(1))
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
    private TRoom SpawnRoom<TRoom>(TRoom template, int row, int column) where TRoom : BaseRoom
    {
        var newRoom = Instantiate(template, transform);
        newRoom.transform.localPosition = new Vector3(column * c_roomSize, 0.0f, row * c_roomSize);
        newRoom.transform.localRotation = Quaternion.identity;
        newRoom.transform.localScale = Vector3.one;
        newRoom.name = $"{template.name} [{row}, {column}]";

        var renderMesh = newRoom.GetComponent<MeshRenderer>();
        if (renderMesh != null)
        {
            var shader = GameSession.GetInstance().GameMode == GameSession.GameModeType.GroundControl
                ? Shader.Find("Unlit/Texture")
                : Shader.Find("Standard");
            renderMesh.sharedMaterial.shader = shader;
        }

        m_tiles[row, column] = newRoom;
        return newRoom;
    }
}
