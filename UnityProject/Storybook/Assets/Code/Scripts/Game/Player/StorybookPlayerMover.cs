using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StorybookPlayerMover : BasePlayerMover {

    List<PlayerWorldPawn> m_playerWorldPawns = new List<PlayerWorldPawn>();

    [SerializeField]
    List<PlayerNode> m_playerPositions = new List<PlayerNode>();

    private bool m_isInCombat = false;

    public PlayerNode[] PlayerPositions
    {
        get { return m_playerPositions.ToArray(); }
    }

    public void OnGUI()
    {
        if (!m_isInCombat)
        {
            if (CurrentRoom.NorthDoor.IsDoorEnabled && GUILayout.Button("Go North"))
            {
                Debug.Log("Going north");
                MoveInDirection(Door.Direction.North);
                CreateRoom(CurrentRoom.NorthDoor.NextRoomLocation);
                TargetNode = CurrentRoom.NorthDoor;
                StartCoroutine(MoveToDoor(CurrentRoom.NorthDoor.NextRoomLocation));
            }
            if (CurrentRoom.EastDoor.IsDoorEnabled && GUILayout.Button("Go East"))
            {
                Debug.Log("Going east");
                MoveInDirection(Door.Direction.East);
                CreateRoom(CurrentRoom.EastDoor.NextRoomLocation);
                TargetNode = CurrentRoom.EastDoor;
                StartCoroutine(MoveToDoor(CurrentRoom.EastDoor.NextRoomLocation));
            }
            if (CurrentRoom.SouthDoor.IsDoorEnabled && GUILayout.Button("Go South"))
            {
                Debug.Log("Going south");
                MoveInDirection(Door.Direction.South);
                CreateRoom(CurrentRoom.SouthDoor.NextRoomLocation);
                TargetNode = CurrentRoom.SouthDoor;
                StartCoroutine(MoveToDoor(CurrentRoom.SouthDoor.NextRoomLocation));
            }
            if (CurrentRoom.WestDoor.IsDoorEnabled && GUILayout.Button("Go West"))
            {
                Debug.Log("Going west");
                MoveInDirection(Door.Direction.West);
                CreateRoom(CurrentRoom.WestDoor.NextRoomLocation);
                TargetNode = CurrentRoom.WestDoor;
                StartCoroutine(MoveToDoor(CurrentRoom.WestDoor.NextRoomLocation));
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
        while (!IsAtTarget)
        {
            yield return new WaitForFixedUpdate();
        }

        MoveToNextRoom(newRoomLoc);
    }

    public void EnterCombat()
    {
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
        foreach (PlayerWorldPawn pawn in m_playerWorldPawns)
        {
            pawn.enabled = true;
            pawn.gameObject.SetActive(true);
        }
        TransitionFromCombat();
    }

}
