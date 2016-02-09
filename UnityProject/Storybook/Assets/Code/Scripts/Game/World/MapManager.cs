using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

public class MapManager : Photon.PunBehaviour {

    struct Point
    {
        public bool Equals(Point other)
        {
            return x == other.x && y == other.y;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            return obj is Point && Equals((Point) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (x * 397) ^ y;
            }
        }

        public int x, y;
        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return ("(" + x.ToString() + ", " + y.ToString() + ")");
        }

        public static bool operator ==(Point p1, Point p2)
        {
            return p1.x == p2.x && p1.y == p2.y;
        }

        public static bool operator !=(Point p1, Point p2)
        {
            return !(p1 == p2);
        }
    }

    struct Pair
    {
        public Point pointA, pointB;
        public Pair(Point pointA, Point pointB)
        {
            this.pointA = pointA;
            this.pointB = pointB;
        }
    }

    private static MapManager s_instance;

    public static MapManager Instance
    {
        get { return s_instance; }
    }

    [SerializeField]
    private int m_worldMaxXSize = 4;

    [SerializeField]
    private int m_worldMaxYSize = 4;

    private RoomObject[,] m_worldGrid; // Creates a 2D array to place rooms

    [SerializeField]
    private RoomObject m_combatRoomPrefab;

    [SerializeField]
    private RoomObject m_startRoomPrefab;

    [SerializeField]
    private RoomObject m_exitRoomPrefab;

    [SerializeField]
    private RoomObject m_shopRoomPrefab;

    [SerializeField]
    private RoomObject m_emptyRoomPrefab;

    [SerializeField]
    private int m_minRoomsStartToExit = 4;

    [SerializeField]
    private int m_additionalHallsMax = 4;

    [SerializeField]
    private int m_additionalHallsMin = 2;

    private RoomData[,] m_worldMapData;

    [SerializeField]
    private int m_defaultRoomSize = 50; // Default room size (in blocks in Unity editor)

    public enum RoomType { None = 0, Start, Combat, Exit, Shop };

    private List<Point> m_pathFromStartToExit = new List<Point>();

    private Point m_startPoint;
    private Point m_exitPoint;
    private Point m_shopPoint;
    private int m_roomDataReceived = 0;

    private HashSet<RoomObject> m_registeredRooms = new HashSet<RoomObject>(); 

    // Initialize
    protected override void Awake()
    {
        base.Awake();

        s_instance = this;
    }

    /*
    void Start()
    {
            for (int i = 0; i < 100; i++)
            {
                GenerateMap();
            }
    }    
    */

    // Place a new room in the world.
    // The MapMgr is not concerned with the contents of the room, just that it can be placed.
    // 
    public RoomObject PlaceRoom(Location gridPosition, PageData pageToUseData)
    {
        int placeX = gridPosition.X;
        int placeY = gridPosition.Y;

        // Cannot have a piece hanging off
        Assert.IsFalse((placeX < 0 || placeX >= m_worldMaxXSize) ||
                       (placeY < 0 || placeY >= m_worldMaxYSize));

        // Now check to make sure there isn't a room already in the spot.
        // The player cannot overwrite rooms that have already been placed.
        if (m_worldGrid[placeX, placeY] != null)
        {
            Debug.Log("Returning null");
            return null;
        }
        Vector3 roomGridLocation = _getWorldLocationFromGrid(placeX, placeY);

        RoomData currentRoomData = m_worldMapData[placeX, placeY];
        RoomObject roomPrefab = _getRoomPrefab(currentRoomData);

        GameObject roomGameObject = PhotonNetwork.Instantiate(roomPrefab.name, roomGridLocation, new Quaternion(), 0);
        RoomObject room = roomGameObject.GetComponent<RoomObject>();
        room.RoomPageData = pageToUseData;
        room.Construct(currentRoomData);
        PhotonNetwork.Spawn(roomGameObject.GetComponent<PhotonView>());
        return room;
    }

    /// <summary>
    /// Registers an existing room with a grid location.
    /// </summary>
    /// <param name="room">The room to register.</param>
    public void RegisterRoom(RoomObject room)
    {
        Location gridPosition = room.RoomLocation;

        int placeX = gridPosition.X;
        int placeY = gridPosition.Y;

        // Cannot register a room twice
        Assert.IsFalse(ContainsRoom(room));

        // Cannot have a piece hanging off
        Assert.IsFalse((placeX < 0 || placeX >= m_worldMaxXSize) ||
                       (placeY < 0 || placeY >= m_worldMaxYSize));

        // Cannot register over a room that already exists.
        Assert.IsNull(m_worldGrid[placeX, placeY]);

        m_worldGrid[placeX, placeY] = room;
    }

    /// <summary>
    /// Checks to see if a room is contained in the grid.
    /// </summary>
    /// <param name="room">The room to check for.</param>
    /// <returns>True if the room is in the grid, otherwise false.</returns>
    public bool ContainsRoom(RoomObject room)
    {
        return m_registeredRooms.Contains(room);
    }

    /// <summary>
    /// Get a room from a grid location.
    /// </summary>
    /// <param name="roomLoc">The grid location to get from.</param>
    /// <returns>The room at that location, null if there is none.</returns>
    public RoomObject GetRoom(Location roomLoc)
    {
        if (roomLoc.X < 0 || roomLoc.X > m_worldGrid.GetLength(0))
            return null;

        if (roomLoc.Y < 0 || roomLoc.Y > m_worldGrid.GetLength(1))
            return null;

        return m_worldGrid[roomLoc.X, roomLoc.Y];
    }

    // Tests the Map Manager code. Called once on startup.

    void TestMapMgr()
    {
        Location OOB_X = new Location(-1, 0);
        Location OOB_Y = new Location(0, 13);
        Location OOB_Both = new Location(25, 25);
        Location Good_1 = new Location(5, 5);
        Location Good_2 = new Location(5, 6);
        PlaceRoom(OOB_X, new PageData()); // try placing a room out of bounds in X (expect fail)
        PlaceRoom(OOB_Y, new PageData()); // try placing a room OOB in Y (expect fail)
        PlaceRoom(OOB_Both, new PageData()); // try placing a room OOB in both directions (expect fail)
        PlaceRoom(Good_1, new PageData()); // try placing a room in a good location (expect pass)
        PlaceRoom(Good_2, new PageData()); // try placing a room in another good location (expect pass)
        PlaceRoom(Good_1, new PageData()); // try placing a room where another room has already been placed (expect fail)
    }

    void TestMapMgrPlacement()
    {
        Location Good_1 = new Location(5, 5);
        Location Good_2 = new Location(5, 6);
        GameObject aBlankRoom = GameObject.Find("TestRoom");
    }

    private Vector3 _getWorldLocationFromGrid(int placeX, int placeY)
    {
        // If we got here, then the location is assumed to be valid.
        // Place the room.
        return new Vector3(m_defaultRoomSize * placeY, 0, -m_defaultRoomSize * placeX);
    }

    /// <summary>
    /// Checks to see if the given location is occupied by a room
    /// </summary>
    /// <param name="locToCheck">The location in the grid to check</param>
    /// <returns>True if the room is occupied, false otherwise</returns>
    private bool _isLocationOccupied(Location locToCheck)
    {
        if (locToCheck.X >= 0 && locToCheck.X < m_worldMaxXSize && locToCheck.Y >= 0 && locToCheck.Y < m_worldMaxYSize)
        {
            RoomObject roomAtLoc = m_worldGrid[locToCheck.X, locToCheck.Y];
            return (roomAtLoc != null);
        }
        return false;
    }

    /// <summary>
    /// Takes in a door and returns the door that is paired with it
    /// </summary>
    /// <param name="currentLoc">The location in the grid of the given door</param>
    /// <param name="entryDoor">The door to find the pairing of</param>
    /// <returns>The door the is linked to the given door</returns>
    public Door GetDoorPartner(Location currentLoc, Door entryDoor)
    {
        RoomObject currentRoom = m_worldGrid[currentLoc.X, currentLoc.Y];
        Door[] roomDoors = currentRoom.AllDoors;
        int doorIndex;

        // Iterate through all of the doors in the room to determine its index
        for (doorIndex = 0; doorIndex < roomDoors.Length; doorIndex++)
        {
            if (roomDoors[doorIndex] == entryDoor)
            {
                break;
            }
        }

        Door exitDoor = null;

        if (doorIndex == (int)Door.Direction.North)
        {
            Location exitDoorLoc = new Location(currentLoc.X - 1, currentLoc.Y);
            RoomObject exitRoom = m_worldGrid[exitDoorLoc.X, exitDoorLoc.Y];
            exitDoor = exitRoom.SouthDoor;
        }

        else if (doorIndex == (int)Door.Direction.East)
        {
            Location exitDoorLoc = new Location(currentLoc.X, currentLoc.Y + 1);
            RoomObject exitRoom = m_worldGrid[exitDoorLoc.X, exitDoorLoc.Y];
            exitDoor = exitRoom.WestDoor;
        }

        else if (doorIndex == (int)Door.Direction.South)
        {
            Location exitDoorLoc = new Location(currentLoc.X + 1, currentLoc.Y);
            RoomObject exitRoom = m_worldGrid[exitDoorLoc.X, exitDoorLoc.Y];
            exitDoor = exitRoom.NorthDoor;
        }

        else if (doorIndex == (int)Door.Direction.West)
        {
            Location exitDoorLoc = new Location(currentLoc.X, currentLoc.Y - 1);
            RoomObject exitRoom = m_worldGrid[exitDoorLoc.X, exitDoorLoc.Y];
            exitDoor = exitRoom.EastDoor;
        }

        return exitDoor;
    }

    //TODO: Move this to the player entity
    /// <summary>
    /// Moves the camera to the given location, called when a player switches rooms
    /// </summary>
    /// <param name="newCameraLoc">The grid location to move the camera to</param>
    public void MoveCamera(Location newCameraLoc)
    {
        RoomObject newRoom = m_worldGrid[newCameraLoc.X, newCameraLoc.Y];
        Transform roomCamNode = newRoom.CameraNode;
        Camera.main.transform.position = roomCamNode.transform.position;
    }

    /// <summary>
    /// Enables or disables all of the room objects currently in the game
    /// </summary>
    /// <param name="isLoad">Enables all room objects if this is true, disables if it is false</param>
    public void LoadMap(bool isLoad)
    {
        for (int i = 0; i < m_worldMaxXSize; i++)
        {
            for (int j = 0; j < m_worldMaxYSize; j++)
            {
                RoomObject room = m_worldGrid[i, j];
                if (room != null)
                {
                    room.enabled = isLoad;
                    room.gameObject.SetActive(isLoad);
                }
            }
        }
    }

    /// <summary>
    /// Generates the map for the floor, which includes the start, exit, doors that will be active and the placement of special rooms
    /// </summary>
    public void GenerateMap()
    {
        m_worldGrid = new RoomObject[m_worldMaxXSize, m_worldMaxYSize];
        m_worldMapData = new RoomData[m_worldMaxXSize, m_worldMaxYSize];

        // In case the map fails to generate properly, it will just try again
        try {
            _placeStart();
            _placeExit();
            _createPathFromStartToExit();
            _addAdditionalDoors();
            _placeSpecialRooms();
            _printMap();
        }
        catch
        {
            GenerateMap();
        }      
    }

    /// <summary>
    /// Prints out the map data to the console
    /// </summary>
    private void _printMap()
    {
#if false
        Debug.Log("Start = " + m_startPoint.ToString());
        Debug.Log("Exit = " + m_exitPoint.ToString());
        Debug.Log("Shop = " + m_shopPoint.ToString());
        Debug.Log("Is Actually exit = " + m_worldMapData[m_exitPoint.x, m_exitPoint.y].RoomType);

        for (int i = 0; i < m_worldMaxXSize; i++)
        {
            for (int j = 0; j < m_worldMaxYSize; j++)
            {
                Point currentPoint = new Point(i, j);
                RoomData currentRoom = m_worldMapData[i, j];
                Debug.Log("Position = " + currentPoint.ToString());
                Debug.Log("Current Room: NorthDoor = " + currentRoom.IsNorthDoorActive + "| East Door = " + currentRoom.IsEastDoorActive +
                    "| South Door = " + currentRoom.IsSouthDoorActive + "| West Door = " + currentRoom.IsWestDoorActive);
            }
        } 
#endif
    }

    /// <summary>
    /// Randomly chooses locations for a shop or other special room types
    /// </summary>
    private void _placeSpecialRooms()
    {
        bool isSpotCombat = true;
        int shopX = 0;
        int shopY = 0;
        while (isSpotCombat)
        {
            shopX = UnityEngine.Random.Range(0, m_worldMaxXSize);
            shopY = UnityEngine.Random.Range(0, m_worldMaxYSize);
            if (m_worldMapData[shopX, shopY].RoomType == RoomType.Combat)
            {
                isSpotCombat = false;
            }
        }
        RoomData currentData = m_worldMapData[shopX, shopY];
        RoomData shopRoom = new RoomData(shopX, shopY, currentData.IsNorthDoorActive, currentData.IsEastDoorActive, currentData.IsSouthDoorActive, currentData.IsWestDoorActive, RoomType.Shop);
        m_worldMapData[shopX, shopY] = shopRoom;
        m_shopPoint = new Point(shopX, shopY);
    }

    /// <summary>
    /// Randomly chooses a point on the map to put the starting room
    /// </summary>
    private void _placeStart()
    {
        bool isSpotOccupied = true;
        int startX = 0;
        int startY = 0;
        while (isSpotOccupied)
        {
            startX = UnityEngine.Random.Range(0, m_worldMaxXSize);
            startY = UnityEngine.Random.Range(0, m_worldMaxYSize);
            if (m_worldMapData[startX, startY].RoomType == RoomType.None)
            {
                isSpotOccupied = false;
            }
        }

        RoomData startingRoom = new RoomData(startX, startY, false, false, false, false, RoomType.Start);
        m_worldMapData[startX, startY] = startingRoom;
        m_startPoint = new Point(startX, startY);
    }

    /// <summary>
    /// Randomly chooses a place on the map to place the exit
    /// </summary>
    private void _placeExit()
    {
        bool isSpotOccupied = true;
        int exitX = 0;
        int exitY = 0;
        while (isSpotOccupied)
        {
            exitX = UnityEngine.Random.Range(0, m_worldMaxXSize);
            exitY = UnityEngine.Random.Range(0, m_worldMaxYSize);
            if (m_worldMapData[exitX, exitY].RoomType == RoomType.None)
            {
                isSpotOccupied = false;
            }
        }

        RoomData exitRoom = new RoomData(exitX, exitY, false, false, false, false, RoomType.Exit);
        m_worldMapData[exitX, exitY] = exitRoom;
        m_exitPoint = new Point(exitX, exitY);
    }

    /// <summary>
    /// Creates the path from the start to the exit as well as the other doors that will be active on the floor
    /// </summary>
    private void _createPathFromStartToExit()
    {
        _depthBranch(0, m_startPoint);
    }

    /// <summary>
    /// Uses recursion to generate a map that connects all of the rooms in the dungeon
    /// </summary>
    /// <param name="depth">The current depth that the function is at</param>
    /// <param name="position">The current position in the map that it is looking at</param>
    private void _depthBranch(int depth, Point position)
    {
        List<Point> validPositions = _getSurroundingPositions(position);
        while (validPositions.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, validPositions.Count - 1);
            Point newPos = validPositions[randomIndex];
            if (_positionCheck(depth, newPos))
            {
                RoomData currentNewPosRoomData = m_worldMapData[newPos.x, newPos.y];
                bool isEmptyRoom = false;
                if (currentNewPosRoomData.RoomType == RoomType.None)
                {
                    isEmptyRoom = true;
                }
                
                if (isEmptyRoom)
                {
                    RoomData newPosRoomData = new RoomData(newPos.x, newPos.y, false, false, false, false, RoomType.Combat);
                    m_worldMapData[newPos.x, newPos.y] = newPosRoomData;
                }

                _connectRooms(position, newPos);

                if (isEmptyRoom) { 
                    _depthBranch(depth + 1, newPos);
                }
            }
            validPositions.Remove(newPos);
        }
    }

    /// <summary>
    /// Connects the rooms at the two given points by setting the corresponding doors to true in their RoomData
    /// </summary>
    /// <param name="currentRoomPos">The position of the current room in the map</param>
    /// <param name="newRoomPos">The position of the new room in the map</param>
    private void _connectRooms(Point currentRoomPos, Point newRoomPos)
    {
        RoomData currentRoom = m_worldMapData[currentRoomPos.x, currentRoomPos.y];
        RoomData newRoom = m_worldMapData[newRoomPos.x, newRoomPos.y];
        if (newRoomPos.y < currentRoomPos.y)
        {
            currentRoom.IsWestDoorActive = true;
            newRoom.IsEastDoorActive = true;
        }
        else if (newRoomPos.y > currentRoomPos.y)
        {
            currentRoom.IsEastDoorActive = true;
            newRoom.IsWestDoorActive = true;
        }
        else if (newRoomPos.x < currentRoomPos.x)
        {
            currentRoom.IsNorthDoorActive = true;
            newRoom.IsSouthDoorActive = true;
        }
        else if (newRoomPos.x > currentRoomPos.x)
        {
            currentRoom.IsSouthDoorActive = true;
            newRoom.IsNorthDoorActive = true;
        }
        m_worldMapData[currentRoomPos.x, currentRoomPos.y] = currentRoom;
        m_worldMapData[newRoomPos.x, newRoomPos.y] = newRoom;
    }

    /// <summary>
    /// Disconnects the rooms at the two given points by setting the corresponding doors to false in their RoomData
    /// </summary>
    /// <param name="currentRoomPos">The position of the current room in the map</param>
    /// <param name="newRoomPos">The position of the new room in the map</param>
    private void _disconnectRooms(Point currentRoomPos, Point newRoomPos)
    {
        RoomData currentRoom = m_worldMapData[currentRoomPos.x, currentRoomPos.y];
        RoomData newRoom = m_worldMapData[newRoomPos.x, newRoomPos.y];
        if (newRoomPos.y < currentRoomPos.y)
        {
            currentRoom.IsWestDoorActive = false;
            newRoom.IsEastDoorActive = false;
        }
        else if (newRoomPos.y > currentRoomPos.y)
        {
            currentRoom.IsEastDoorActive = false;
            newRoom.IsWestDoorActive = false;
        }
        else if (newRoomPos.x < currentRoomPos.x)
        {
            currentRoom.IsNorthDoorActive = false;
            newRoom.IsSouthDoorActive = false;
        }
        else if (newRoomPos.x > currentRoomPos.x)
        {
            currentRoom.IsSouthDoorActive = false;
            newRoom.IsNorthDoorActive = false;
        }
        m_worldMapData[currentRoomPos.x, currentRoomPos.y] = currentRoom;
        m_worldMapData[newRoomPos.x, newRoomPos.y] = newRoom;
    }

    /// <summary>
    /// Checks to see if the position that the maze-generation algorithm is trying to add is valid
    /// </summary>
    /// <param name="depth">The current depth of the algorithm</param>
    /// <param name="position">The position that the algorithm is trying to add</param>
    /// <returns>True if the position is valid, false otherwise</returns>
    private bool _positionCheck(int depth, Point position)
    {
        bool doesPositionPass = true;

        // If the position is off the map, return false
        if (position.x >= m_worldMaxXSize || position.x < 0 || position.y >= m_worldMaxYSize || position.y < 0)
        {
            doesPositionPass = false;
        }

        // If the position is the exit and the minimum distance from start to exit has not been met, return false
        RoomData roomAtPos = m_worldMapData[position.x, position.y];
        if (roomAtPos.RoomType == RoomType.Exit && depth < m_minRoomsStartToExit)
        {
            doesPositionPass = false;
        }

        // If the room type at the position is not 'None' or 'Exit', return false
        if (roomAtPos.RoomType != RoomType.None && roomAtPos.RoomType != RoomType.Exit)
        {
            doesPositionPass = false;
        }
        return doesPositionPass;
    }

    /// <summary>
    /// Returns a list of all the valid positions surrounding the given point
    /// </summary>
    /// <param name="position">The position to get the surrounding points</param>
    /// <returns>A list of all the valid positions around the given point</returns>
    private List<Point> _getSurroundingPositions(Point position)
    {
        List<Point> surroundingPositions = new List<Point>();
        if (position.y + 1 < m_worldMaxYSize)
        {
            Point validPos = new Point(position.x, position.y + 1);
            surroundingPositions.Add(validPos);
        }
        if (position.y - 1 >= 0)
        {
            Point validPos = new Point(position.x, position.y - 1);
            surroundingPositions.Add(validPos);
        }
        if (position.x + 1 < m_worldMaxXSize)
        {
            Point validPos = new Point(position.x + 1, position.y);
            surroundingPositions.Add(validPos);
        }
        if (position.x - 1 >= 0)
        {
            Point validPos = new Point(position.x - 1, position.y);
            surroundingPositions.Add(validPos);
        }
        return surroundingPositions;
    }

    /// <summary>
    /// Called after the map is generated, it adds additional connections in the map 
    /// </summary>
    private void _addAdditionalDoors()
    {
        int addedHalls = 0;
        int additionalHalls = UnityEngine.Random.Range(m_additionalHallsMin, m_additionalHallsMax + 1);
        while (addedHalls < additionalHalls)
        {
            Pair roomPair = _getPairOfAdjacentRooms();
            if (!_isPairConnected(roomPair))
            {
                _connectRooms(roomPair.pointA, roomPair.pointB);
                List<Point> shortestPath = _aStarSearch(m_startPoint, m_exitPoint);

                // If adding this connection causes the minimum distance from start to exit to go below the set distance, disconnect
                // these two rooms and try again
                if (shortestPath.Count < m_minRoomsStartToExit)
                {
                    _disconnectRooms(roomPair.pointA, roomPair.pointB);
                }

                // Otherwise, keep these rooms connected and increment the number of added halls
                else
                {
                    addedHalls++;
                }
                _resetRoomDataAStar();
            }
        }
    }

    /// <summary>
    /// Randomly selects a pair of adjacent rooms in the map
    /// </summary>
    /// <returns>A Pair containing two adjacent rooms</returns>
    private Pair _getPairOfAdjacentRooms()
    {
        int pointX = UnityEngine.Random.Range(0, m_worldMaxXSize);
        int pointY = UnityEngine.Random.Range(0, m_worldMaxYSize);
        Point pointA = new Point(pointX, pointY);

        List<Point> adjacentRooms = new List<Point>();
        int northPointX = pointX - 1;
        int eastPointY = pointY + 1;
        int southPointX = pointX + 1;
        int westPointY = pointY - 1;

        if (northPointX >= 0)
        {
            Point northPoint = new Point(northPointX, pointY);
            adjacentRooms.Add(northPoint);
        }

        if (eastPointY < m_worldMaxYSize)
        {
            Point eastPoint = new Point(pointX, eastPointY);
            adjacentRooms.Add(eastPoint);
        }

        if (southPointX < m_worldMaxXSize)
        {
            Point southPoint = new Point(southPointX, pointY);
            adjacentRooms.Add(southPoint);
        }

        if (westPointY >= 0)
        {
            Point westPoint = new Point(pointX, westPointY);
            adjacentRooms.Add(westPoint);
        }

        int otherRoomIndex = UnityEngine.Random.Range(0, adjacentRooms.Count);
        Point otherRoomPoint = adjacentRooms[otherRoomIndex];
        Pair roomPair = new Pair(pointA, otherRoomPoint);
        return roomPair;
    }

    /// <summary>
    /// Checks to see if the given pair of rooms is connected
    /// </summary>
    /// <param name="roomPair">The pair of rooms to check</param>
    /// <returns>True if the given pair is connected, false otherwise</returns>
    private bool _isPairConnected(Pair roomPair)
    {
        Point roomAPoint = roomPair.pointA;
        Point roomBPoint = roomPair.pointB;
        RoomData roomA = m_worldMapData[roomAPoint.x, roomAPoint.y];
        RoomData roomB = m_worldMapData[roomBPoint.x, roomBPoint.y];

        if (roomAPoint.y < roomBPoint.y && roomA.IsEastDoorActive && roomB.IsWestDoorActive)
        {
            return true;
        }
        else if (roomAPoint.y > roomBPoint.y && roomA.IsWestDoorActive && roomB.IsEastDoorActive)
        {
            return true;
        }
        else if (roomAPoint.x < roomBPoint.x && roomA.IsSouthDoorActive && roomB.IsNorthDoorActive)
        {
            return true;
        }
        else if (roomAPoint.x > roomBPoint.x && roomA.IsNorthDoorActive && roomB.IsSouthDoorActive)
        {
            return true;
        }
        return false;
    }

    public void GoToNewFloor()
    {

    }

    // Finds a path between the start node and the destination node using A* pathfinding algorithm
    private List<Point> _aStarSearch(Point start, Point destination)
    {
        // Initialize the necessary lists and variables for A* pathfinding
        List<Point> all = new List<Point>();
        all.Add(start);
        List<Point> closed = new List<Point>();
        List<Point> open = new List<Point>(all);
        List<Point> path = new List<Point>();

        while (!(open[0].x == destination.x && open[0].y == destination.y))
        {   
            Point lowest = open[0];
            RoomData lowestData = m_worldMapData[lowest.x, lowest.y];

            // Find the node with the lowest value in open
            int openCount = open.Count;
            for (int i = 0; i < openCount; i++)
            {
                Point currentNode = open[i];
                RoomData currentData = m_worldMapData[currentNode.x, currentNode.y];

                if (currentData.CostToHere < lowestData.CostToHere)
                {
                    lowest = currentNode;
                }
            }

            // Remove the current lowest from open and add it to the closed since it is being visited
            open.Remove(lowest);
            closed.Add(lowest);

            // Iterate through the neighbors of the selected point
            int lowestNeighborCount = _getConnectedRooms(lowest).Count;
            for (int i = 0; i < lowestNeighborCount; i++)
            {
                Point currentNeighbor = _getConnectedRooms(lowest)[i];
                RoomData currentNeighborData = m_worldMapData[currentNeighbor.x, currentNeighbor.y];

                // Calculate the cost as the distance between the current point and the current neighbor
                float cost = 1 + lowestData.CostToHere;
                if (_doesListContainPoint(open, currentNeighbor) != -1 && cost < currentNeighborData.CostToHere)
                {
                    open.RemoveAt(_doesListContainPoint(open, currentNeighbor));
                }
                if (_doesListContainPoint(closed, currentNeighbor) != -1 && cost < currentNeighborData.CostToHere)
                {
                    closed.RemoveAt(_doesListContainPoint(closed, currentNeighbor));
                }
                if (_doesListContainPoint(open, currentNeighbor) == -1 && _doesListContainPoint(closed, currentNeighbor) == -1)
                {
                    // If the open list is empty, add the current neighbor to the open list
                    if (open.Count == 0)
                    {
                        open.Add(currentNeighbor);
                    }
                    // Otherwise, place the node in the open list at the appropriate position according to its cost
                    else
                    {
                        bool addedToOpen = false;
                        int count = open.Count;
                        for (int j = 0; j < count; j++)
                        {
                            if (cost < m_worldMapData[open[j].x, open[j].y].CostToHere)
                            {
                                open.Insert(j, currentNeighbor);
                                addedToOpen = true;
                                break;
                            }
                        }
                        if (!addedToOpen)
                        {
                            open.Insert(open.Count, currentNeighbor);
                        }
                    }

                    // Set the cost and parent of the current neighbor
                    currentNeighborData.CostToHere = cost;
                    currentNeighborData.ParentX = lowest.x;
                    currentNeighborData.ParentY = lowest.y;
                    m_worldMapData[currentNeighbor.x, currentNeighbor.y] = currentNeighborData;
                }
            }
        }

        // Once finished, determine what the path is by looking at the parent nodes and insert them into the path in the proper order
        Point node = destination;
        while (!(node.x == start.x && node.y == start.y))
        {
            RoomData nodeData = m_worldMapData[node.x, node.y];
            path.Insert(0, node);
            node = new Point(nodeData.ParentX, nodeData.ParentY);
        }
        path.Insert(0, node);
        return path;
    }

    /// <summary>
    /// Gets the list of rooms that are connected to the room at the given point
    /// </summary>
    /// <param name="roomLoc">The location of the room to check for</param>
    /// <returns>The list of all the points that the given room is connected to</returns>
    private List<Point> _getConnectedRooms(Point roomLoc)
    {
        RoomData room = m_worldMapData[roomLoc.x, roomLoc.y];
        List<Point> connectedRooms = new List<Point>();

        if (room.IsNorthDoorActive)
        {
            Point northRoomPoint = new Point(roomLoc.x - 1, roomLoc.y);
            connectedRooms.Add(northRoomPoint);
        }

        if (room.IsEastDoorActive)
        {
            Point eastRoomPoint = new Point(roomLoc.x, roomLoc.y + 1);
            connectedRooms.Add(eastRoomPoint);
        }

        if (room.IsSouthDoorActive)
        {
            Point southRoomPoint = new Point(roomLoc.x + 1, roomLoc.y);
            connectedRooms.Add(southRoomPoint);
        }

        if (room.IsWestDoorActive)
        {
            Point westRoomPoint = new Point(roomLoc.x, roomLoc.y - 1);
            connectedRooms.Add(westRoomPoint);
        }

        return connectedRooms;
    }

    /// <summary>
    /// Checks to see if the given list contains the given point
    /// </summary>
    /// <param name="listToSearch">The list to search through</param>
    /// <param name="pointToSearch">The point to check for</param>
    /// <returns>True if the given point is in the list, false otherwise</returns>
    private int _doesListContainPoint(List<Point> listToSearch, Point pointToSearch)
    {
        int i = 0;
        foreach (Point p in listToSearch)
        {
            if (p.x == pointToSearch.x && p.y == pointToSearch.y)
            {
                return i;
            }
            i++;
        }
        return -1;
    }

    /// <summary>
    /// Resets all the AStar data in the RoomData for all the RoomData in the map
    /// </summary>
    private void _resetRoomDataAStar()
    {
        for(int i = 0; i < m_worldMaxXSize; i++)
        {
            for (int j = 0; j < m_worldMaxYSize; j++)
            {
                RoomData currentRoom = m_worldMapData[i, j];
                currentRoom.ResetAStarData();
                m_worldMapData[i, j] = currentRoom;
            }
        }
    }

    public RoomObject PlaceStartRoom()
    {
        Location start = new Location(m_startPoint.x, m_startPoint.y);
        return PlaceRoom(start, new PageData(-1, Genre.None));
    }

    public bool isAllDataReceived()
    {
        if (m_roomDataReceived >= (m_worldMaxXSize * m_worldMaxYSize))
        {
            return true;
        }
        return false;
    }

    public RoomData GetRoomData(int roomDataX, int roomDataY)
    {
        return m_worldMapData[roomDataX, roomDataY];
    }

    private RoomObject _getRoomPrefab(RoomData roomToPlace)
    {
        RoomObject roomPrefab;
        switch (roomToPlace.RoomType)
        {
            case RoomType.Combat:
                roomPrefab = m_combatRoomPrefab;
                break;

            case RoomType.Exit:
                roomPrefab = m_exitRoomPrefab;
                break;

            case RoomType.Start:
                roomPrefab = m_startRoomPrefab;
                break;

            case RoomType.Shop:
                roomPrefab = m_shopRoomPrefab;
                break;

            default:
                roomPrefab = m_emptyRoomPrefab;
                break;
        }
        return roomPrefab;
    }
}
