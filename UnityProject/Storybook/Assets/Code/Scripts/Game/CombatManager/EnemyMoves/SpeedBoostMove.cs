using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class SpeedBoostMove : AIMove
{

    private bool IS_MOVE_ATTACK = false;

    [SerializeField]
    private int m_speedIncreaseValue = 3;

    protected override void DoMoveEffect()
    {
        foreach (CombatPawn combatPawn in MoveTargets)
        {
            combatPawn.IncreasePawnSpeed(m_speedIncreaseValue);
        }
    }

    /// <summary>
    /// Chooses the targets for the move, currently chooses the pawns that have the lowest speed
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

    public override void InitializeIsMoveAttack()
    {
        SetIsMoveAttack(IS_MOVE_ATTACK);
    }
}
