using UnityEngine;
using System.Collections;

public class TestPageSupportMove : PageMove {

    [SerializeField]
    private int m_speedIncrease = 3;

    private const bool IS_MOVE_ATTACK = false;

    private bool m_isMoveStarted = false;

    // Use this for initialization
    void Start () {
        SetIsMoveAttack(IS_MOVE_ATTACK);
    }

    /// <summary>
    /// Increases the speed stat of all pawns in the target list
    /// </summary>
    protected override void DoMoveEffect()
    {
        foreach (CombatPawn playerPawn in MoveTargets)
        {
            playerPawn.IncreasePawnSpeed(m_speedIncrease);
        }
    }

    public override void ExecuteMove()
    {
        Animator playerAnimator = MoveOwner.GetComponent<Animator>();
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
        else if (TimeSinceMoveStarted >= 1.2f)
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
