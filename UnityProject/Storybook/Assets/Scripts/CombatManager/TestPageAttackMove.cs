using UnityEngine;
using System.Collections;
using System;

public class TestPageAttackMove : PageMove {

    [SerializeField]
    private int m_moveDamage = 3;

    private const bool IS_MOVE_ATTACK = true;

    void Awake()
    {
        SetIsMoveAttack(IS_MOVE_ATTACK);
    }

    /// <summary>
    /// Deals damage to all the combat pawns in the target list
    /// </summary>
    protected override void DoMoveEffect()
    {
        foreach (CombatPawn enemyPawn in MoveTargets)
        {
            enemyPawn.DealDamageToPawn(m_moveDamage);
        }
    }

    public override void ExecuteMove()
    {
        SetTimeSinceMoveStarted(TimeSinceMoveStarted + Time.deltaTime);
        if (TimeSinceMoveStarted >= 0.5 && !IsMoveEffectCompleted)
        {
            DoMoveEffect();
            SetIsMoveEffectCompleted(true);
        }
        else if (TimeSinceMoveStarted >= 1)
        {
            Debug.Log("Page move is complete");
            SetIsMoveComplete(true);
        }
    }

    public override void InitializeIsMoveAttack()
    {
        SetIsMoveAttack(IS_MOVE_ATTACK);
    }
}
