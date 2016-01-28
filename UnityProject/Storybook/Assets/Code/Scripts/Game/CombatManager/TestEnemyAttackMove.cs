using UnityEngine;
using System.Collections.Generic;
using System;

public class TestEnemyAttackMove : AIMove {

    private bool IS_MOVE_ATTACK = true;

    void Start()
    {
        SetIsMoveAttack(IS_MOVE_ATTACK);
    }

    /// <summary>
    /// Deals combat damage to all the player pawns in the target list
    /// </summary>
    protected override void DoMoveEffect()
    {
        foreach (CombatPawn combatPawn in MoveTargets)
        {
            int moveDamage = StatsManager.CalcDamage(MoveOwner.PawnGenre, combatPawn.PawnGenre, MoveGenre, MoveLevel, MoveOwner.Attack, combatPawn.Defense);
            Debug.Log("Enemy dealing " + moveDamage + " to the player");
            combatPawn.DealDamageToPawn(moveDamage);
        }
    }

    /// <summary>
    /// Chooses the targets for the move, currently chooses the pawns that have the lowest health
    /// </summary>
    /// <param name="possibleTargets">The list of CombatPawn that are the possible targets for the move</param>
    public override void ChooseTargets(HashSet<CombatPawn> possibleTargets)
    {
        // Reset the targets list before starting
        SetMoveTargets(new List<CombatPawn>());

        base.ChooseTargets(possibleTargets);
        if (MoveTargets.Length > 0)
        {
            return;
        }

        List<CombatPawn> possibleTargetsList = new List<CombatPawn>(possibleTargets);
        List<CombatPawn> targets = new List<CombatPawn>();
        while (targets.Count < NumberOfTargets)
        {
            CombatPawn lowestHealthPawn = null;
            for (int i = 0; i < possibleTargetsList.Count; i++)
            {
                CombatPawn currentPawn = possibleTargetsList[i];
                if (lowestHealthPawn == null || currentPawn.Health < lowestHealthPawn.Health)
                {
                    lowestHealthPawn = currentPawn;
                }
            }
            targets.Add(lowestHealthPawn);
            possibleTargetsList.Remove(lowestHealthPawn);
        }
        SetMoveTargets(targets);
    }

    public override void InitializeIsMoveAttack()
    {
        SetIsMoveAttack(IS_MOVE_ATTACK);
    }
}
