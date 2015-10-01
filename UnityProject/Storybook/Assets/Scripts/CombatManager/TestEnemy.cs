using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TestEnemy : CombatEnemy
{

    void Awake()
    {
        InitializeVariables();
    }

    void Start()
    {
        SetSpeed(5);
        SetHealth(10);
    }

    public override void OnThink()
    {
        // Randomly select a player pawn to attack
        EnemyMove moveSelected = CreateMove();
        SetMoveForTurn(moveSelected);
        SetHasSubmittedMove(true);
        Debug.Log("Enemy submitted move");
    }
}
