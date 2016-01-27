using UnityEngine;
using System.Collections;
using System;

public class TestPageAttackMove : PageMove {

    private const bool IS_MOVE_ATTACK = true;

    private bool m_isMoveStarted = false;

    void Awake()
    {
        Debug.Log("Setting is move attack");
        SetIsMoveAttack(IS_MOVE_ATTACK);
    }

    /// <summary>
    /// Deals damage to all the combat pawns in the target list
    /// </summary>
    protected override void DoMoveEffect()
    {
        foreach (CombatPawn enemyPawn in MoveTargets)
        {
            int moveDamage = StatsManager.CalcDamage(MoveOwner.PawnGenre, enemyPawn.PawnGenre, MoveGenre, MoveLevel, MoveOwner.Attack, enemyPawn.Defense);
            enemyPawn.DealDamageToPawn(moveDamage);
        }
    }
}
