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

    public override void ExecuteMove()
    {
        NetExecuteState executeState = FindObjectOfType<NetExecuteState>();
        Animator playerAnimator = executeState.CurrentCombatPawn.GetComponent<Animator>();
        if (!m_isMoveStarted)
        {
            playerAnimator.SetBool("IdleToIdle", false);
            playerAnimator.SetBool("WalkToIdle", false);
            playerAnimator.SetBool("AttackToIdle", false);
            playerAnimator.SetBool("IdleToAttack", true);
            m_isMoveStarted = true;
        }
        SetTimeSinceMoveStarted(TimeSinceMoveStarted + Time.deltaTime);
        if (TimeSinceMoveStarted >= 0.5 && !IsMoveEffectCompleted)
        {
            DoMoveEffect();
            SetIsMoveEffectCompleted(true);
        }
        else if (TimeSinceMoveStarted >= 3.0f)
        {
            Debug.Log("Page move is complete");
            playerAnimator.SetBool("IdleToAttack", false);
            playerAnimator.SetBool("AttackToIdle", true);
            playerAnimator.SetBool("IdleToIdle", true);
            SetIsMoveComplete(true);
            m_isMoveStarted = false;
            SetTimeSinceMoveStarted(0);
        }
    }
}
