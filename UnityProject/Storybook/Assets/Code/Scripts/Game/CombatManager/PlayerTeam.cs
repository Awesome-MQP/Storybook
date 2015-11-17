using UnityEngine;
using System.Collections;
using System;

public class PlayerTeam : CombatTeam {

    public override void SpawnTeam()
    {
        int i = 0;
        foreach (CombatPawn pawn in PawnsOnTeam)
        {
            pawn.transform.position = PawnPositions[i];
        }
    }

    public override void RemovePawnFromTeam(CombatPawn pawnToRemove)
    {
        base.RemovePawnFromTeam(pawnToRemove);
    }

    public override void StartCombat()
    {
        Debug.Log("Player team starting combat");
    }

    public override void StartNewTurn()
    {
        foreach(CombatPawn pawn in PawnsOnTeam)
        {
            pawn.DecrementBoosts();
        }
    }
}
