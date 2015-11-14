using UnityEngine;
using System.Collections;

public class TestPageSupportMove : PageMove {

    [SerializeField]
    private int m_speedIncrease = 3;

    private const bool IS_MOVE_ATTACK = false;

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
}
