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
        // TODO - Change back to player hitting space bar to select the first move
        if (combatUI.IsMoveChosen)
        {
            Debug.Log("Player move submitted");
            List<CombatPawn> targetList = new List<CombatPawn>();
            PlayerMove chosenMove = PlayerHand[combatUI.ChosenIndex];
            Debug.Log(chosenMove.NumberOfTargets);
            chosenMove.InitializeIsMoveAttack();
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