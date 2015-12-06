﻿using UnityEngine;
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

            PlayerMove chosenMove = PlayerHand[0];
            chosenMove.SetMoveOwner(this);
            chosenMove.SetMoveTargets(targetList);
            chosenMove.InitializeMove(); 
            SetMoveForTurn(chosenMove);
            SetHasSubmittedMove(true);

            Page p = new Page();
            p.Rarity = chosenMove.MoveRarity;
            p.PageGenre = chosenMove.MoveGenre;
            p.PageLevel = chosenMove.MoveLevel;
            p.PageOwner = this;
            PageMove pm = new PageMoveObject() as PageMove; // I had to create a child class that inherits PageMove in order to do stuff with it.
            pm.construct(p);

            int[] targetIds = new int[4];
            for (int i = 0; i < pm.NumberOfTargets; i++)
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
        PlayerMove chosenMove = PlayerHand[moveIndex];
        List<CombatPawn> targets = new List<CombatPawn>();

        // Determine the targets of the move based on the list of target ids
        CombatPawn[] possibleTargetList = null;

        // If the move is an attack or status, the possible targets are the enemy list
        if ((chosenMove.MoveType == MoveType.Attack) || (chosenMove.MoveType == MoveType.Status))
        {
            Debug.Log("Move is an attack/status");
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