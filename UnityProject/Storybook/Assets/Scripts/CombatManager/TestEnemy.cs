using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TestEnemy : CombatEnemy
{

    public override void OnThink()
    {
        PhotonView m_scenePhontonView = GetComponent<PhotonView>();
        // Randomly select a player pawn to attack
        EnemyMove moveSelected = CreateMove();
        SetMoveForTurn(moveSelected);
        SetHasSubmittedMove(true);
        Debug.Log("Enemy submitted move");

        int[] targetIds = new int[4];
        for (int i = 0; i < moveSelected.MoveTargets.Length; i++)
        {
            CombatPawn target = moveSelected.MoveTargets[i];
            targetIds[i] = target.PawnId;
        }
        int moveIndex;
        for (moveIndex = 0; moveIndex < EnemyMoves.Length; moveIndex++)
        {
            if (EnemyMoves[moveIndex] == moveSelected)
            {
                break;
            }
        }

        m_scenePhontonView.RPC("SendEnemyMoveOverNetwork", PhotonTargets.Others, PawnId, targetIds, moveIndex);
    }

    [PunRPC]
    private void SendEnemyMoveOverNetwork(int playerId, int[] targetIds, int moveIndex)
    {
        Debug.Log("Other enemy submitted move");
        EnemyMove chosenMove = EnemyMoves[moveIndex];
        List<CombatPawn> targets = new List<CombatPawn>();

        // Determine the targets of the move based on the list of target ids
        CombatPawn[] possibleTargetList = null;
        if (chosenMove.IsMoveAttack)
        {
            possibleTargetList = CManager.EnemyList;
        }
        else
        {
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
