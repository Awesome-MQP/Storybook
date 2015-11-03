using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatNetworker : Photon.PunBehaviour {

    private CombatManager m_cManager;
    private static PhotonView m_scenePhotonView;

    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    // Use this for initialization
    void Start () {
        Debug.Log("Starting Networker");
        m_cManager = FindObjectOfType<CombatManager>();
        m_scenePhotonView = this.GetComponent<PhotonView>();
    }
	
	// Update is called once per frame
	void Update () {

	}

    public void SubmitMoveForPlayer(int playerId, CombatMove playerMove)
    {
        // Determine the enemy ids to send over network
        int[] targetIds = new int[4];
        for (int i = 0; i < playerMove.MoveTargets.Length; i++)
        {
            CombatPawn target = playerMove.MoveTargets[i];
            targetIds[i] = target.PlayerId;
        }

        // Find the move index of the chosen move
        GameManager gm = FindObjectOfType<GameManager>();
        int moveIndex = 0;
        foreach (PlayerMove pm in gm.AllPlayerMoves)
        {
            if (playerMove == pm)
            {
                break;
            }
            moveIndex += 1;
        }

        m_scenePhotonView.RPC("SendMoveOverNetwork", PhotonTargets.Others, playerId, targetIds, moveIndex);
    }

    [PunRPC]
    private void SendMoveOverNetwork(int playerId, int[] targetIds, int moveIndex)
    {
        Debug.Log("Other player submitted move");
        GameManager gm = FindObjectOfType<GameManager>();
        PlayerMove chosenMove = gm.AllPlayerMoves[moveIndex];
        List<CombatPawn> targets = new List<CombatPawn>();

        // Determine the targets of the move based on the list of target ids
        CombatPawn[] possibleTargetList = null;
        if (chosenMove.IsMoveAttack)
        {
            possibleTargetList = m_cManager.EnemyList;
        }
        else
        {
            possibleTargetList = m_cManager.PlayerPawnList;
        }
        foreach (CombatPawn pawn in m_cManager.EnemyList)
        {
            if (targetIds.Contains(pawn.PlayerId))
            {
                targets.Add(pawn);
            }
        }

        // Find the player that submitted the move
        CombatPawn submittedPlayer = null;
        foreach (CombatPawn playerPawn in m_cManager.PlayerPawnList)
        {
            if (playerPawn.PlayerId == playerId)
            {
                submittedPlayer = playerPawn;
                break;
            }
        }

        chosenMove.SetMoveOwner(submittedPlayer);
        chosenMove.SetMoveTargets(targets);
        chosenMove.InitializeMove();
        submittedPlayer.SetMoveForTurn(chosenMove);
        submittedPlayer.SetHasSubmittedMove(true);
    }
}
