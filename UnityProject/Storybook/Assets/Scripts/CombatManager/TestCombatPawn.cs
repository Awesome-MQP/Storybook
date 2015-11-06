using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TestCombatPawn : CombatPlayer
{

    // Waits for input of a move
    public override void OnThink()
    {
        // TODO - Change back to player hitting space bar to select the first move
        if (Input.GetKeyDown(KeyCode.Space) && PhotonNetwork.player.ID == PawnId)
        {
            PhotonView m_scenePhotonView = GetComponent<PhotonView>();
            Debug.Log(m_scenePhotonView.viewID);
            Debug.Log("Space bar pressed");
            List<CombatPawn> targetList = new List<CombatPawn>();
            targetList.Add(CManager.EnemyList[0]);
            PlayerMove chosenMove = PlayerHand[0];
            chosenMove.SetMoveOwner(this);
            chosenMove.SetMoveTargets(targetList);
            chosenMove.InitializeMove();
            SetMoveForTurn(chosenMove);
            SetHasSubmittedMove(true);

            int[] targetIds = new int[4];
            for (int i = 0; i < chosenMove.MoveTargets.Length; i++)
            {
                CombatPawn target = chosenMove.MoveTargets[i];
                targetIds[i] = target.PawnId;
            }
            int moveIndex = 0;

            m_scenePhotonView.RPC("SendPlayerMoveOverNetwork", PhotonTargets.All, PawnId, targetIds, moveIndex);
        }
    }

    [PunRPC]
    private void SendPlayerMoveOverNetwork(int playerId, int[] targetIds, int moveIndex)
    {
        Debug.Log("Other player submitted move");
        PlayerMove chosenMove = PlayerHand[moveIndex];
        List<CombatPawn> targets = new List<CombatPawn>();

        // Determine the targets of the move based on the list of target ids
        CombatPawn[] possibleTargetList = null;
        if (chosenMove.IsMoveAttack)
        {
            Debug.Log("Move is an attack");
            possibleTargetList = CManager.EnemyList;
        }
        else
        {
            Debug.Log("Move is not an attack");
            possibleTargetList = CManager.PlayerPawnList;
        }
        foreach (CombatPawn pawn in possibleTargetList)
        {
            if (targetIds.Contains(pawn.PawnId))
            {
                targets.Add(pawn);
            }
        }

        chosenMove.SetMoveOwner(this);
        chosenMove.SetMoveTargets(targets);
        chosenMove.InitializeMove();
        SetMoveForTurn(chosenMove);
        SetHasSubmittedMove(true);
    }
}