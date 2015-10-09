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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space bar pressed");
            List<CombatPawn> targetList = new List<CombatPawn>();
            targetList.Add(CManager.EnemyList[0]);
            PlayerMove chosenMove = PlayerHand[0];
            chosenMove.SetMoveTargets(targetList);
            chosenMove.InitializeMove();
            SetMoveForTurn(chosenMove);
            SetHasSubmittedMove(true);
        }
    }
}