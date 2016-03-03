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
    public enum NetState
    {
        None,
        WaitingForInput,
        FailedToMove,
        WaitingForRoomEvent,
        MovingBetweenRooms
    }

    protected delegate IEnumerable<StateDelegate> StateDelegate();

    private bool m_isAtRoomCenter;
    private bool m_lockMovement;

    private Door.Direction m_nextMoveDirection = Door.Direction.Unknown;

    private StateDelegate m_currentState;
    private NetState m_netState;

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

    /// <summary>
    /// Variable that is synced over the network to allow information about the current mover state on other clients.
    /// </summary>
    [SyncProperty]
    protected NetState NetworkedMovementState
    {
        get { return m_netState; }
        set
        {
            Assert.IsTrue(ShouldBeChanging);
            m_netState = value;
            _onNetStateChanged();
            PropertyChanged();
        }
    }

    public virtual void Construct(RoomObject room)
    {
        m_currentState = StateWaitingForInput;

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
    }

    public override void OnStartOwner(bool wasSpawn)
    {
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

    protected virtual void OnWaitForInput()
    {
        
    }

    protected virtual void OnFailToMove()
    {
        
    }

    protected virtual void OnMovingBetweenRooms()
    {
        
    }

    protected virtual void OnWaitingForRoomEvent()
    {
        
    }

    /// <summary>
    /// State when the room mover is waiting for input on which direction to move in.
    /// </summary>
    /// <returns>The next state to go to.</returns>
    protected virtual IEnumerable<StateDelegate> StateWaitingForInput()
    {
        m_nextMoveDirection = Door.Direction.Unknown;

        if(NetworkedMovementState != NetState.FailedToMove)
            NetworkedMovementState = NetState.WaitingForInput;

        while (m_nextMoveDirection == Door.Direction.Unknown || !CanMove)
        {
            yield return null;
        }

        yield return StateMoveInDirection;
    }

    /// <summary>
    /// State when the mover try to move in a direction.
    /// </summary>
    /// <returns>The next state to go to.</returns>
    protected virtual IEnumerable<StateDelegate> StateMoveInDirection()
    {
        Door nextDoor = CurrentRoom.GetDoorByDirection(m_nextMoveDirection);

        if (nextDoor == null || !nextDoor.IsDoorEnabled || !nextDoor.IsDoorOpen)
        {
            yield return StateFailMoveInDirection;
        }
        else
        {
            yield return StateLeavingRoom;
        }
    }

    /// <summary>
    /// State when the mover fail to move in a direction.
    /// </summary>
    /// <returns>The next state to go to.</returns>
    protected virtual IEnumerable<StateDelegate> StateFailMoveInDirection()
    {
        NetworkedMovementState = NetState.FailedToMove;
        
        yield return StateWaitingForInput;
    }

    /// <summary>
    /// State when the mover starts to leave through a door.
    /// </summary>
    /// <returns>The next state to go to.</returns>
    protected virtual IEnumerable<StateDelegate> StateLeavingRoom()
    {
        TargetNode = CurrentRoom.GetDoorByDirection(m_nextMoveDirection);
        NetworkedMovementState = NetState.MovingBetweenRooms;

        while (!m_atNode)
        {
            yield return null;
        }

        CurrentRoom.RoomExit(this);

        yield return StateMoveBetweenRooms;
    }

    /// <summary>
    /// State when the mover is moving between rooms.
    /// </summary>
    /// <returns>The next state to go to.</returns>
    protected virtual IEnumerable<StateDelegate> StateMoveBetweenRooms()
    {
        TargetNode = CurrentRoom.GetDoorByDirection(m_nextMoveDirection).LinkedDoor;

        while (!m_atNode)
        {
            yield return null;
        }

        yield return StateEnterRoom;
    }

    /// <summary>
    /// State when the mover enters a room and should move into position. This state will also by default wait for the room event to end.
    /// </summary>
    /// <returns>The next state to go to.</returns>
    protected virtual IEnumerable<StateDelegate> StateEnterRoom()
    {
        CurrentRoom.RoomEntered(this);
        TargetNode = CurrentRoom.CenterNode;

        while (!m_atNode)
        {
            yield return null;
        }

        NetworkedMovementState = NetState.WaitingForRoomEvent;

        foreach (var v in CurrentRoom.RoomEvent(this))
        {
            yield return null;
        }

        yield return StateWaitingForInput;
    }

    [PunRPC]
    protected void _rpcMoveInDirection(byte directionIndex)
    {
        MoveInDirection((Door.Direction) directionIndex);
    }

    protected Door.Direction MoveDirection
    {
        get { return m_nextMoveDirection; }
        set { m_nextMoveDirection = value; }
    }

    private IEnumerator _stateMachine()
    {
        yield return null;//wait for one full frame to pass.

        while (m_currentState != null && gameObject.activeSelf)
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

    private void _onNetStateChanged()
    {
        switch (m_netState)
        {
            case NetState.None:
                break;
            case NetState.WaitingForInput:
                OnWaitForInput();
                break;
            case NetState.FailedToMove:
                OnFailToMove();
                break;
            case NetState.WaitingForRoomEvent:
                OnWaitingForRoomEvent();
                break;
            case NetState.MovingBetweenRooms:
                OnMovingBetweenRooms();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
