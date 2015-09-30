using UnityEngine;
using System.Collections;

public class TestEnemyBoostMove : EnemyMove {

    private int MOVE_DAMAGE = 3;
    private int MOVE_TARGETS = 4;
    private bool IS_MOVE_ATTACK = false;
    private int SPEED_INCREASE_VALUE = 2;

    public TestEnemyBoostMove()
    {
        SetIsMoveAttack(IS_MOVE_ATTACK);
        SetNumberOfTargets(MOVE_TARGETS);
    }

    protected override void DoMoveEffect()
    {
        foreach (CombatPawn combatPawn in MoveTargets)
        {
            combatPawn.IncreasePawnSpeed(SPEED_INCREASE_VALUE);
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
            Debug.Log("Enemy move is complete");
            SetIsMoveComplete(true);
        }
    }

}
