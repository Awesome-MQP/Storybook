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

    public void MovePlayersToNode(Vector3 nodeForPlayers)
    {
        foreach(PlayerWorldPawn playerPawn in m_playerWorldPawns)
        {
            playerPawn.TargetPosition = nodeForPlayers;
        }
    }

    public void RegisterPlayerWorldPawn(PlayerWorldPawn pawnToRegister)
    {
        m_playerWorldPawns.Add(pawnToRegister);
        pawnToRegister.transform.parent = transform;
    }

    public IEnumerator MoveToDoor(Location newRoomLoc)
    {
        _playWalkAnimations();
        while (!IsAtTarget)
        {
            yield return new WaitForFixedUpdate();
        }

        MoveToNextRoom(newRoomLoc);
    }

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

    public void OpenPageForRoomMenu()
    {
        Object loadedObject = Resources.Load("UIPrefabs/ChoosePageForRoomCanvas");
        m_canvas = (GameObject)Instantiate(loadedObject);
        m_UIHandler = m_canvas.GetComponent<PageForRoomUIHandler>();
        m_UIHandler.RegisterPlayerMover(this);
        m_UIHandler.PopulateMenu();
        m_isMenuOpen = true;
    }

    public void OpenDeckManagementMenu()
    {
        Object loadedObject = Resources.Load("UIPrefabs/DeckManagementCanvas");
        GameObject canvas = (GameObject) Instantiate(loadedObject);
        DeckManagementUIHandler uiHandler = canvas.GetComponent<DeckManagementUIHandler>();
        uiHandler.PopulateMenu();
    }

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
