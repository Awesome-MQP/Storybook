using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TestEnemy : CombatEnemy
{

    void Awake()
    {
        EnemyMove testEnemyMove = new TestEnemyMove();
        List<EnemyMove> enemyMoveList = new List<EnemyMove>();
        enemyMoveList.Add(testEnemyMove);
        SetEnemyMoves(enemyMoveList);
    }

    void Start()
    {
        SetSpeed(5);
        SetHealth(10);
    }

    public override void OnThink()
    {
        // Randomly select a player pawn to attack
        EnemyMove moveSelected = ChooseMove();
        moveSelected.InitializeMove();
        SetMoveForTurn(moveSelected);
        SetHasSubmittedMove(true);
    }
}
