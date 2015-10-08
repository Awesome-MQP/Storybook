using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TestCombatPawn : CombatPlayer
{

    CombatDemoUIHandler combatUI;

    void Start()
    {
        combatUI = FindObjectOfType<CombatDemoUIHandler>();
    }

    // Waits for input of a move
    public override void OnThink()
    {
        // If the space bar is pressed, submit the move to the CombatManager, and exit the OnThink() function
        if (combatUI.IsMoveChosen)
        {
            Debug.Log("Space bar pressed");
            // TODO - Way for player to select targets
            List<CombatPawn> targetList = new List<CombatPawn>();
            PlayerMove chosenMove = PlayerHand[combatUI.ChosenIndex];
            if (chosenMove.IsMoveAttack)
            {
                targetList.Add(CManager.EnemyList[0]);
            }
            else
            {
                targetList = new List<CombatPawn>(CManager.PlayerPawnList);
            }
            chosenMove.SetMoveTargets(targetList);
            chosenMove.InitializeMove();
            SetMoveForTurn(chosenMove);
            SetHasSubmittedMove(true);
        }
    }
}