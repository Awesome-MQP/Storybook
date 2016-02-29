using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

//[RequireComponent(typeof (Animator))]
public class StorybookPlayerMover : BasePlayerMover,
    PageForRoomEventDispatcher.IPageForRoomEventListener,
    DeckManagementEventDispatcher.IDeckManagementEventListener,
    OverworldEventDispatcher.IOverworldEventListener, 
    RoomEventEventDispatcher.IRoomEventListener
{
    List<PlayerWorldPawn> m_playerWorldPawns = new List<PlayerWorldPawn>();

    [SerializeField]
    List<PlayerNode> m_playerPositions = new List<PlayerNode>();

    [SerializeField]
    private PageForRoomUIHandler m_UIHandler;

    private bool m_isInCombat = false;
    private bool m_isMenuOpen = false;

    private GameObject m_canvas;

    private Door.Direction m_lastMoveDirection = Door.Direction.Unknown;

    //Stores the animator for this player mover.
    private Animator m_animator;

    public PlayerNode[] PlayerPositions
    {
        get { return m_playerPositions.ToArray(); }
    }

    public EventDispatcher Dispatcher { get { return EventDispatcher.GetDispatcher<PageForRoomEventDispatcher>(); } }
    public EventDispatcher DeckMgmtDispatcher { get { return EventDispatcher.GetDispatcher<DeckManagementEventDispatcher>(); } }
    public EventDispatcher OverworldEventDispatcher { get { return EventDispatcher.GetDispatcher<OverworldEventDispatcher>(); } }
    public EventDispatcher RoomEventEventDispatcher { get { return EventDispatcher.GetDispatcher<RoomEventEventDispatcher>(); } }

    [SyncProperty]
    private int NetPlayerCountEvent
    {
        get { return m_playerWorldPawns.Count; }
        set
        {
            Assert.IsTrue(ShouldBeChanging);
            if (IsMine)
                OwnerOnPlayerCountChanged(value);
            else
                PeerOnPlayerCountChanged(value);
            PropertyChanged();
        }
    }

    protected override void Awake()
    {
        EventDispatcher.GetDispatcher<PageForRoomEventDispatcher>().RegisterEventListener(this);
        EventDispatcher.GetDispatcher<DeckManagementEventDispatcher>().RegisterEventListener(this);
        EventDispatcher.GetDispatcher<OverworldEventDispatcher>().RegisterEventListener(this);
        EventDispatcher.GetDispatcher<RoomEventEventDispatcher>().RegisterEventListener(this);

        m_animator = GetComponent<Animator>();

        base.Awake();
    }

    protected override void OnRegistered()
    {
        OpenDeckManagementMenu();
    }

    // I copied this in from Dev, don't know how recent it is, but I couldn't find another place to have the listeners be destroyed.
    void OnDestroy()
    {
        EventDispatcher.GetDispatcher<PageForRoomEventDispatcher>().RemoveListener(this);
        EventDispatcher.GetDispatcher<DeckManagementEventDispatcher>().RemoveListener(this);
        EventDispatcher.GetDispatcher<OverworldEventDispatcher>().RemoveListener(this);
        EventDispatcher.GetDispatcher<RoomEventEventDispatcher>().RemoveListener(this);

        Debug.Log("Mover destroyed");
    }

    /// <summary>
    /// Sets the target position for all the players in the world pawns list
    /// </summary>
    /// <param name="nodeForPlayers">The position to be used as the target</param>
    public void MovePlayersToNode(Vector3 nodeForPlayers)
    {
        foreach (PlayerWorldPawn playerPawn in m_playerWorldPawns)
        {
            playerPawn.TargetPosition = nodeForPlayers;
        }
    }

    /// <summary>
    /// Adds the given PlayerWorldPawn to the list of world pawns
    /// </summary>
    /// <param name="pawnToRegister">The pawn to add to the list</param>
    public void RegisterPlayerWorldPawn(PlayerWorldPawn pawnToRegister)
    {
        m_playerWorldPawns.Add(pawnToRegister);
        pawnToRegister.transform.parent = transform;
    }

    /// <summary>
    /// Waits until the players have moved to the door
    /// When the players reach the door, calls the MoveToNextRoom function to move the players to the room that the door connects
    /// </summary>
    /// <param name="newRoomLoc">The location of the room to move to</param>
    /// <returns></returns>
    public IEnumerator MoveToDoor(Location newRoomLoc)
    {
        _playWalkAnimations();
        while (!IsAtTarget)
        {
            yield return new WaitForFixedUpdate();
        }

        MoveToNextRoom(newRoomLoc);
    }

    /// <summary>
    /// Called when the game is transitioning to combat from the dungeon
    /// Disables all of the world pawns
    /// </summary>
    public void EnterCombat()
    {
        _playIdleAnimations();
        m_isInCombat = true;
        foreach (PlayerWorldPawn pawn in m_playerWorldPawns)
        {
            pawn.enabled = false;
            pawn.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Called when the game is transitioning to the dungeon from combat
    /// Destroys the enemy pawns that are in the center of the room, and enables the PlayerWorldPawns
    /// </summary>
    public void ExitCombat()
    {
        m_isInCombat = false;
        CombatRoom currentCombatRoom = (CombatRoom) CurrentRoom;
        currentCombatRoom.DestroyEnemyWorldPawns();
        foreach (PlayerWorldPawn pawn in m_playerWorldPawns)
        {
            pawn.enabled = true;
            pawn.gameObject.SetActive(true);
        }
        TransitionFromCombat();
        OpenDeckManagementMenu();
    }

    /// <summary>
    /// Opens the menu for selecting a page to create a room
    /// </summary>
    public void OpenPageForRoomMenu()
    {
        Object loadedObject = Resources.Load("UIPrefabs/ChoosePageForRoomCanvas");
        m_canvas = (GameObject) Instantiate(loadedObject);
        m_UIHandler = m_canvas.GetComponent<PageForRoomUIHandler>();
        m_UIHandler.PopulateMenu();
        m_isMenuOpen = true;
    }

    /// <summary>
    /// Opens the menu for managing the player's deck
    /// </summary>
    public void OpenDeckManagementMenu()
    {
        UnityEngine.Object loadedObject = Resources.Load("UIPrefabs/DeckManagementCanvas");
        GameObject canvas = (GameObject) Instantiate(loadedObject);
        DeckManagementUIHandler uiHandler = canvas.GetComponent<DeckManagementUIHandler>();
        uiHandler.PopulateMenu();
    }

    /// <summary>
    /// Opens the menu for selecting a direction
    /// </summary>
    public void OpenOverworldMenu()
    {
        Object loadedObject = Resources.Load("UIPrefabs/OverworldCanvas");
        GameObject canvas = (GameObject) Instantiate(loadedObject);
        OverworldUIHandler uiHandler = canvas.GetComponent<OverworldUIHandler>();
        uiHandler.PopulateMenu(CurrentRoom);
    }

    /// <summary>
    /// Called when a player has selected a page to use to create a room
    /// Uses the selected page data to create the next room
    /// </summary>
    /// <param name="pageToUseData">The page data from the page that the player selected for the room</param>
    public void SubmitPageForRoom(PageData pageToUseData)
    {
        m_isMenuOpen = false;
        Door selectedDoor = CurrentRoom.GetDoorByDirection(m_lastMoveDirection);
        CreateRoom(selectedDoor.NextRoomLocation, pageToUseData);
        selectedDoor.IsConnectedRoomMade = true;
        selectedDoor.LinkedDoor.IsConnectedRoomMade = true;
        selectedDoor.OpenDoor();
        MoveInDirection(m_lastMoveDirection);
    }

    /// <summary>
    /// Called by the overworld menu when a direction has been selected
    /// </summary>
    /// <param name="moveDirection"></param>
    public void SubmitDirection(Door.Direction moveDirection)
    {
        Debug.Log("MoveDirection = " + moveDirection);
        MoveInDirection(moveDirection);
    }

    protected override IEnumerable<StateDelegate> OnWaitingForInput()
    {
        _playIdleAnimations();

        return base.OnWaitingForInput();
    }

    protected override IEnumerable<StateDelegate> OnFailMoveInDirection()
    {
        m_lastMoveDirection = MoveDirection;
        OpenPageForRoomMenu();

        return base.OnFailMoveInDirection();
    }

    protected override IEnumerable<StateDelegate> OnLeavingRoom()
    {
        _playWalkAnimations();

        return base.OnLeavingRoom();
    }

    protected override IEnumerable<StateDelegate> OnMoveBetweenRooms()
    {
        return base.OnMoveBetweenRooms();
    }

    private void _playWalkAnimations()
    {
        foreach (PlayerWorldPawn pawn in m_playerWorldPawns)
        {
            pawn.SwitchCharacterToWalking();
        }
    }

    private void _playIdleAnimations()
    {
        foreach (PlayerWorldPawn pawn in m_playerWorldPawns)
        {
            pawn.SwitchCharacterToIdle();
        }
    }

    /// <summary>
    /// Called by the DeckManagement menu when the finish button is pressed
    /// Opens the OverworldMenu
    /// </summary>
    public void OnDeckManagementClosed()
    {
        OpenOverworldMenu();
    }

    public void OnRoomCleared()
    {
        OpenDeckManagementMenu();
    }

    public void TransitionFromCombat()
    {
        Camera.main.transform.position = CurrentRoom.CameraNode.position;
        Camera.main.transform.rotation = CurrentRoom.CameraNode.rotation;
    }

    protected override bool OnRegisterPlayer(PlayerObject player)
    {
        PlayerEntity playerEntity = player as PlayerEntity;
        if (!playerEntity)
            return false;

        ResourceAsset asset = GameManager.GetInstance<BaseStorybookGame>().GetWorldPawnForGenre(playerEntity.Genre);
        PlayerWorldPawn pawn = PhotonNetwork.Instantiate<PlayerWorldPawn>(asset, Vector3.zero, Quaternion.identity, 0);
        m_playerWorldPawns.Add(pawn);
        pawn.Construct(playerEntity);

        PhotonNetwork.Spawn(pawn.photonView);

        NetPlayerCountEvent = m_playerWorldPawns.Count;

        return true;
    }

    protected virtual void OwnerOnPlayerCountChanged(int newPlayerCount)
    {
        //m_animator.SetInteger("PlayerCount", newPlayerCount);

        for (int i = 0; i < newPlayerCount; i++)
        {
            m_playerWorldPawns[i].Position = m_playerPositions[i].transform.position;
            m_playerWorldPawns[i].TargetNode = m_playerPositions[i];
        }
    }

    protected virtual void PeerOnPlayerCountChanged(int newPlayerCount)
    {
        //m_animator.SetInteger("PlayerCount", newPlayerCount);
    }
}
