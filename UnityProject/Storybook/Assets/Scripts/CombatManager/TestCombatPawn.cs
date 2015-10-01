using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TestCombatPawn : CombatPlayer
{
    // Give the player one move for testing that is triggered when the space bar is pressed
    private PlayerMove m_testMove;

    // Use this for initialization
    void Start()
    {
        SetSpeed(6);
        SetHealth(10);
        m_testMove = new TestPageMove();
    }

    // Waits for input of a move
    public override void OnThink()
    {
        // If the space bar is pressed, submit the move to the CombatManager, and exit the OnThink() function
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space bar pressed");
            // TODO - Way for player to select targets
            List<CombatPawn> targetList = new List<CombatPawn>(CManager.EnemyList);
            m_testMove.SetMoveTargets(targetList);
            m_testMove.InitializeMove();
            SetMoveForTurn(m_testMove);
            SetHasSubmittedMove(true);
        }
    }
}