using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TestEnemy : CombatEnemy
{

    public override void OnThink()
    {
        if (PhotonNetwork.isMasterClient)
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
    }

    /// <summary>
    /// Sends the chosen enemy move to the enemy pawns on the other clients
    /// </summary>
    /// <param name="playerId">The id of the pawn whose move is being sent</param>
    /// <param name="targetIds">The pawn ids of the targets of the move</param>
    /// <param name="moveIndex">The index of the move in the enemy's move pool</param>
    [PunRPC]
    private void SendEnemyMoveOverNetwork(int playerId, int[] targetIds, int moveIndex)
    {
        Debug.Log("Other enemy submitted move");
        EnemyMove chosenMove = EnemyMoves[moveIndex];
        List<CombatPawn> targets = new List<CombatPawn>();

        // Determine the targets of the move based on the list of target ids
        CombatPawn[] possibleTargetList = null;

        // If the move is an attack, the players are the possible targets
        if (chosenMove.IsMoveAttack)
        {
            Debug.Log("Enemy move is an attack");
            possibleTargetList = CManager.PlayerPawnList;
        }

        // If the move is a support move, the other enemies are the possible targets
        else
        {
            Debug.Log("Enemy move is not an attack");
            possibleTargetList = CManager.EnemyList;
        }

        // Iterate through the pawns in the possibleTargetList and find the targets
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
