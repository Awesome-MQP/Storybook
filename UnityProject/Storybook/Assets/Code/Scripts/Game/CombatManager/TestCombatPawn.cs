using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TestCombatPawn : CombatPlayer
{

    // Waits for input of a move
    public override void OnThink()
    {
        // If the player hits the space bar and this pawn is the client's, submit a move
        if (Input.GetKeyDown(KeyCode.Space) && PhotonNetwork.player.ID == PawnId)
        {
            PhotonView m_scenePhotonView = GetComponent<PhotonView>();
            Debug.Log(m_scenePhotonView.viewID);
            Debug.Log("Space bar pressed");
            List<CombatPawn> targetList = new List<CombatPawn>();
            targetList.Add(GetPawnsOpposing()[0]);

            // TODO: Use the actual hand
            //PlayerMove chosenMove = TestHand[0];

            Page chosenPage = PlayerHand[0];
            PlayerMove chosenMove = chosenPage.PlayerCombatMove;
            RemovePageFromHand(chosenPage);

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

    /// <summary>
    /// Sends the player move over network to the corresponding pawn in all clients
    /// </summary>
    /// <param name="playerId">The PawnID of the player submitting the move</param>
    /// <param name="targetIds">The PawnID of the targets of the selected move</param>
    /// <param name="moveIndex">The index of the move in the player's hand of moves</param>
    [PunRPC]
    private void SendPlayerMoveOverNetwork(int playerId, int[] targetIds, int moveIndex)
    {
        Debug.Log("Other player submitted move");

        Page chosenPage = PlayerHand[moveIndex];
        PlayerMove chosenMove = chosenPage.PlayerCombatMove;

        List<CombatPawn> targets = new List<CombatPawn>();

        // Determine the targets of the move based on the list of target ids
        CombatPawn[] possibleTargetList = null;

        // If the move is an attack, the possible targets are the enemy list
        if (chosenMove.IsMoveAttack)
        {
            Debug.Log("Move is an attack");
            possibleTargetList = GetPawnsOpposing();
        }

        // If the move is a support move, the possible targets are the player list
        else
        {
            Debug.Log("Move is not an attack");
            possibleTargetList = GetPawnsOnTeam();
        }

        // Iterate through the possible targets and find the targets based on the targetIds
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