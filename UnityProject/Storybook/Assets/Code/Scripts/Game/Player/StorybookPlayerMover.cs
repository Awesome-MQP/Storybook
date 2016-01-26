using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StorybookPlayerMover : BasePlayerMover {

    List<PlayerWorldPawn> m_playerWorldPawns = new List<PlayerWorldPawn>();

    [SerializeField]
    List<PlayerNode> m_playerPositions = new List<PlayerNode>();

    [SerializeField]
    private PageForRoomUIHandler m_UIHandler;

    private bool m_isInCombat = false;
    private bool m_isMenuOpen = false;

    private GameObject m_canvas;

    public PlayerNode[] PlayerPositions
    {
        get { return m_playerPositions.ToArray(); }
    }

    public void Start()
    {
        OpenDeckManagementMenu();
    }

    public void OnGUI()
    {
        if (!m_isInCombat && !m_isMenuOpen)
        {
            if (CurrentRoom.NorthDoor.IsDoorEnabled && GUILayout.Button("Go North"))
            {
                MoveInDirection(Door.Direction.North);
                if (!CurrentRoom.NorthDoor.IsConnectedRoomMade)
                {
                    OpenPageForRoomMenu();
                }
                else
                {
                    TargetNode = CurrentRoom.NorthDoor;
                    StartCoroutine(MoveToDoor(CurrentRoom.NorthDoor.NextRoomLocation));
                }
            }
            if (CurrentRoom.EastDoor.IsDoorEnabled && GUILayout.Button("Go East"))
            {
                MoveInDirection(Door.Direction.East);
                if (!CurrentRoom.EastDoor.IsConnectedRoomMade)
                {
                    OpenPageForRoomMenu();
                }
                else
                {
                    TargetNode = CurrentRoom.EastDoor;
                    StartCoroutine(MoveToDoor(CurrentRoom.EastDoor.NextRoomLocation));
                }
            }
            if (CurrentRoom.SouthDoor.IsDoorEnabled && GUILayout.Button("Go South"))
            {
                MoveInDirection(Door.Direction.South);
                if (!CurrentRoom.SouthDoor.IsConnectedRoomMade)
                {
                    OpenPageForRoomMenu();
                }
                else
                {
                    TargetNode = CurrentRoom.SouthDoor;
                    StartCoroutine(MoveToDoor(CurrentRoom.SouthDoor.NextRoomLocation));
                }
            }
            if (CurrentRoom.WestDoor.IsDoorEnabled && GUILayout.Button("Go West"))
            {
                MoveInDirection(Door.Direction.West);
                if (!CurrentRoom.WestDoor.IsConnectedRoomMade)
                {
                    OpenPageForRoomMenu();
                }
                else
                {
                    TargetNode = CurrentRoom.WestDoor;
                    StartCoroutine(MoveToDoor(CurrentRoom.WestDoor.NextRoomLocation));
                }
            }
        }
    }

    /// <summary>
    /// Sets the target position for all the players in the world pawns list
    /// </summary>
    /// <param name="nodeForPlayers">The position to be used as the target</param>
    public void MovePlayersToNode(Vector3 nodeForPlayers)
    {
        foreach(PlayerWorldPawn playerPawn in m_playerWorldPawns)
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
        CombatRoom currentCombatRoom = (CombatRoom)CurrentRoom;
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
        m_canvas = (GameObject)Instantiate(loadedObject);
        m_UIHandler = m_canvas.GetComponent<PageForRoomUIHandler>();
        m_UIHandler.RegisterPlayerMover(this);
        m_UIHandler.PopulateMenu();
        m_isMenuOpen = true;
    }

    /// <summary>
    /// Opens the menu for managing the player's deck
    /// </summary>
    public void OpenDeckManagementMenu()
    {
        Object loadedObject = Resources.Load("UIPrefabs/DeckManagementCanvas");
        GameObject canvas = (GameObject) Instantiate(loadedObject);
        DeckManagementUIHandler uiHandler = canvas.GetComponent<DeckManagementUIHandler>();
        uiHandler.PopulateMenu();
    }

    /// <summary>
    /// Called when a player has selected a page to use to create a room
    /// Uses the selected page data to create the next room
    /// </summary>
    /// <param name="pageToUseData">The page data from the page that the player selected for the room</param>
    public void SubmitPageForRoom(PageData pageToUseData)
    {
        Destroy(m_canvas.gameObject);
        m_isMenuOpen = false;
        Door selectedDoor = CurrentRoom.GetDoorByDirection(MoveDirection);
        CreateRoom(selectedDoor.NextRoomLocation, pageToUseData);
        selectedDoor.IsConnectedRoomMade = true;
        selectedDoor.LinkedDoor.IsConnectedRoomMade = true;
        TargetNode = selectedDoor;
        StartCoroutine(MoveToDoor(selectedDoor.NextRoomLocation));
    }

    private void _playWalkAnimations()
    {
        foreach(PlayerWorldPawn pawn in m_playerWorldPawns)
        {
            pawn.SwitchCharacterToWalking();
        }
    }

    private void _playIdleAnimations()
    {
        foreach(PlayerWorldPawn pawn in m_playerWorldPawns)
        {
            pawn.SwitchCharacterToIdle();
        }
    }
}
