using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class TestEnemyBoostMove : EnemyMove {

    private bool IS_MOVE_ATTACK = false;

    [SerializeField]
    private int m_speedIncreaseValue = 3;

    void Start()
    {
        SetIsMoveAttack(IS_MOVE_ATTACK);
    }

    protected override void DoMoveEffect()
    {
        Debug.Log("Enemy doing boost");
        foreach (CombatPawn combatPawn in MoveTargets)
        {
            combatPawn.IncreasePawnSpeed(m_speedIncreaseValue);
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

    public override void ChooseTargets(CombatPawn[] possibleTargets)
    {
        base.ChooseTargets(possibleTargets);
        if (MoveTargets.Length > 0)
        {
            return;
        }

        List<CombatPawn> possibleTargetsList = new List<CombatPawn>(possibleTargets);
        List<CombatPawn> targets = new List<CombatPawn>();
        while (targets.Count < NumberOfTargets)
        {
            CombatPawn lowestSpeedPawn = null;
            for (int i = 0; i < possibleTargetsList.Count; i++)
            {
                CombatPawn currentPawn = possibleTargetsList[i];
                if (lowestSpeedPawn == null || currentPawn.Speed < lowestSpeedPawn.Speed)
                {
                    lowestSpeedPawn = currentPawn;
                }
            }
            targets.Add(lowestSpeedPawn);
            possibleTargetsList.Remove(lowestSpeedPawn);
        }
        SetMoveTargets(targets);
    }

}
