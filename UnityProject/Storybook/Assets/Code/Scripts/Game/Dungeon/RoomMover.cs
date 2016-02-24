using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Assertions;

/// <summary>
/// Class for handling movement between rooms.
/// </summary>
public abstract class RoomMover : NetworkNodeMover, IConstructable<RoomObject>
{
    protected delegate IEnumerable<StateDelegate> StateDelegate();

    private bool m_isAtRoomCenter;
    private bool m_lockMovement;

    private Door.Direction m_nextMoveDirection = Door.Direction.Unknown;

    private StateDelegate m_currentState;

    private bool m_atNode;

    /// <summary>
    /// Gets the current room that this mover is considered in.
    /// </summary>
    public RoomObject CurrentRoom
    {
        get
        {
            MovementNode targetNode = TargetNode;
            if (targetNode)
                return targetNode.GetComponentInParent<RoomObject>();

            return null;
        }
    }

    /// <summary>
    /// If true then the mover is able to select a new direction to move in.
    /// </summary>
    public bool CanMove
    {
        get { return m_isAtRoomCenter && !m_lockMovement;}
    }

    /// <summary>
    /// If true, then a new movement direction cannot be chosen.
    /// </summary>
    [SyncProperty]
    protected bool IsMovementLocked
    {
        get { return m_lockMovement; }
        set
        {
            Assert.IsTrue(ShouldBeChanging);
            m_lockMovement = value;
            PropertyChanged();
        }
    }

    public virtual void Construct(RoomObject room)
    {
        m_currentState = OnWaitingForInput;

        SpawnInRoom(room);
    }

    /// <summary>
    /// Spawns all the players in the given room object
    /// Called only to place the players in the starting room of the floor
    /// </summary>
    /// <param name="room">The room to spawn the players in</param>
    public void SpawnInRoom(RoomObject room)
    {
        Assert.IsTrue(IsMine);
        
        TargetNode = room.CenterNode;
        Position = room.CenterNode.transform.position;

        StartCoroutine(_stateMachine());
    }
    /// <summary>
    /// Tells the mover to move in a certain direction.
    /// </summary>
    /// <param name="direction">The direction to move in.</param>
    public void MoveInDirection(Door.Direction direction)
    {
        if (!CanMove)
            return;

        if (IsMine)
        {
            m_nextMoveDirection = direction;
        }
        else if (IsController)
        {
            photonView.RpcOwner(nameof(_rpcMoveInDirection), (byte)direction);
        }
        else
        {
            Assert.IsTrue(false);
        }
    }

    /// <summary>
    /// Calls PlaceRoom in MapManager using the given location and page data to create the room
    /// </summary>
    /// <param name="roomLoc"></param>
    /// <param name="pageToUseData"></param>
    public void CreateRoom(Location roomLoc, PageData pageToUseData)
    {
        MapManager mapManager = FindObjectOfType<MapManager>();
        mapManager.PlaceRoom(roomLoc, pageToUseData);
    }

    /// <summary>
    /// Moves all the players to the next room
    /// </summary>
    /// <param name="newRoomLoc">The location of the room to move to</param>
    [Obsolete]
    public void MoveToNextRoom(Location newRoomLoc)
    {
        MapManager mapManager = FindObjectOfType<MapManager>();
        RoomObject newRoom = mapManager.GetRoom(newRoomLoc);

        //Camera.main.transform.position = newRoom.CameraNode.position;
        Camera.main.GetComponent<GameCamera>().trackObject(Camera.main, newRoom.CameraNode);
        Camera.main.transform.rotation = newRoom.CameraNode.rotation;

        SpawnInRoom(newRoom);
    }

    protected sealed override void OnArriveAtNode(MovementNode node)
    {
        if (node == CurrentRoom.CenterNode)
        {
            m_isAtRoomCenter = true;
        }

        m_atNode = true;
    }

    protected sealed override void OnLeaveNode(MovementNode node)
    {
        if (node == CurrentRoom.CenterNode)
            m_isAtRoomCenter = false;

        m_atNode = false;
    }

    /// <summary>
    /// State when the room mover is waiting for input on which direction to move in.
    /// </summary>
    /// <returns>The next state to go to.</returns>
    protected virtual IEnumerable<StateDelegate> OnWaitingForInput()
    {
        m_nextMoveDirection = Door.Direction.Unknown;

        while (m_nextMoveDirection == Door.Direction.Unknown || !CanMove)
        {
            yield return null;
        }

        yield return OnMoveInDirection;
    }

    /// <summary>
    /// State when the mover try to move in a direction.
    /// </summary>
    /// <returns>The next state to go to.</returns>
    protected virtual IEnumerable<StateDelegate> OnMoveInDirection()
    {
        Door nextDoor = CurrentRoom.GetDoorByDirection(m_nextMoveDirection);

        if (nextDoor == null || !nextDoor.IsDoorEnabled || !nextDoor.IsDoorOpen)
        {
            yield return OnFailMoveInDirection;
        }
        else
        {
            yield return OnLeavingRoom;
        }
    }

    /// <summary>
    /// State when the mover fail to move in a direction.
    /// </summary>
    /// <returns>The next state to go to.</returns>
    protected virtual IEnumerable<StateDelegate> OnFailMoveInDirection()
    {
        yield return OnWaitingForInput;
    }

    /// <summary>
    /// State when the mover starts to leave through a door.
    /// </summary>
    /// <returns>The next state to go to.</returns>
    protected virtual IEnumerable<StateDelegate> OnLeavingRoom()
    {
        TargetNode = CurrentRoom.GetDoorByDirection(m_nextMoveDirection);

        while (!m_atNode)
        {
            yield return null;
        }

        CurrentRoom.RoomExit(this);

        yield return OnMoveBetweenRooms;
    }

    /// <summary>
    /// State when the mover is moving between rooms.
    /// </summary>
    /// <returns>The next state to go to.</returns>
    protected virtual IEnumerable<StateDelegate> OnMoveBetweenRooms()
    {
        TargetNode = CurrentRoom.GetDoorByDirection(m_nextMoveDirection).LinkedDoor;

        while (!m_atNode)
        {
            yield return null;
        }

        yield return OnEnterRoom;
    }

    /// <summary>
    /// State when the mover enters a room and should move into position. This state will also by default wait for the room event to end.
    /// </summary>
    /// <returns>The next state to go to.</returns>
    protected virtual IEnumerable<StateDelegate> OnEnterRoom()
    {
        CurrentRoom.RoomEntered(this);
        TargetNode = CurrentRoom.CenterNode;

        while (!m_atNode)
        {
            yield return null;
        }

        foreach (var v in CurrentRoom.RoomEvent(this))
        {
            yield return null;
        }

        yield return OnWaitingForInput;
    }

    [PunRPC]
    private void _rpcMoveInDirection(byte directionIndex)
    {
        MoveInDirection((Door.Direction) directionIndex);
    }

    private IEnumerator _stateMachine()
    {
        yield return null;//wait for one full frame to pass.

        while (m_currentState != null)
        {
            StateDelegate next = null;

            foreach (StateDelegate stateDelegate in m_currentState())
            {
                if (stateDelegate != null)
                    next = stateDelegate;

                yield return null;
            }

            if (next != null)
                m_currentState = next;
            else
                yield break;
        }
    }

    protected Door.Direction MoveDirection
    {
        get { return m_nextMoveDirection; }
        set { m_nextMoveDirection = value; }
    }
}
