using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class AttackBoostMove : AIMove
{

    private bool IS_MOVE_ATTACK = false;

    [SerializeField]
    private int m_attackIncreaseValue = 3;

    protected override void DoMoveEffect()
    {
        foreach (CombatPawn combatPawn in MoveTargets)
        {
            combatPawn.IncreasePawnAttack(m_attackIncreaseValue);
        }
    }

    /// <summary>
    /// Chooses the targets for the move, currently chooses the pawns that have the lowest attack
    /// </summary>
    /// <param name="possibleTargets">The list of CombatPawn that are the possible targets for the move</param>
    public override void ChooseTargets(HashSet<CombatPawn> possibleTargets)
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
            CombatPawn lowestAttackPawn = null;
            for (int i = 0; i < possibleTargetsList.Count; i++)
            {
                CombatPawn currentPawn = possibleTargetsList[i];
                if (lowestAttackPawn == null || currentPawn.Attack < lowestAttackPawn.Attack)
                {
                    lowestAttackPawn = currentPawn;
                }
            }
            targets.Add(lowestAttackPawn);
            possibleTargetsList.Remove(lowestAttackPawn);
        }
        SetMoveTargets(targets);
    }

    public override void InitializeIsMoveAttack()
    {
        SetIsMoveAttack(IS_MOVE_ATTACK);
    }
}
