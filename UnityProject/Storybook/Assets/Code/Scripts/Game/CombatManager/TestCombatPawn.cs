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
            
            Page chosenPage = PlayerHand[0];
            PlayerMove chosenMove = chosenPage.PlayerCombatMove;
            RemovePageFromHand(chosenPage);
            
            chosenMove.SetMoveOwner(this);
            chosenMove.SetMoveTargets(targetList);
            chosenMove.InitializeMove(); 
            SetMoveForTurn(chosenMove);
            SetHasSubmittedMove(true);
            
            PageMove pm = new PageMoveObject() as PageMove; // I had to create a child class that inherits PageMove in order to do stuff with it.
            pm.construct(chosenPage);

            int[] targetIds = new int[4];
            for (int i = 0; i < pm.NumberOfTargets; i++)
            {
                CombatPawn target = chosenMove.MoveTargets[i];
                targetIds[i] = target.PawnId;
            }
            int moveIndex = 0;

            GetComponent<PhotonView>().RPC("SendPlayerMoveOverNetwork", PhotonTargets.All, PawnId, targetIds, moveIndex);
        }
    }
}