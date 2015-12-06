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

            int[] targetIds = new int[4];
            for (int i = 0; i < chosenMove.MoveTargets.Length; i++)
            {
                CombatPawn target = chosenMove.MoveTargets[i];
                targetIds[i] = target.PawnId;
            }
            int moveIndex = 0;

            GetComponent<PhotonView>().RPC("SendPlayerMoveOverNetwork", PhotonTargets.All, PawnId, targetIds, moveIndex);
        }
    }

}