using UnityEngine;
using System.Collections;
using System;

public class PageSpeedBoost : PageMove {

    [SerializeField]
    private int m_speedIncrease;

    void Awake()
    {
        SetIsMoveAttack(false);
    }

    protected override void DoMoveEffect()
    {
        foreach (CombatPawn pawn in MoveTargets)
        {
            Debug.Log("Increasing pawn speed");
            int totalStatBoost = m_speedIncrease;
            totalStatBoost += (int)Math.Ceiling((double)MoveLevel / 2); //This may look kind of hacky, pls don't yell at me :-) - Connor
            if (MoveGenre == this.MoveOwner.PawnGenre)
            {
                totalStatBoost += 2; // Give a small boost if the types match up. We won't take the full type chart into effect here.
            }
            pawn.IncreasePawnSpeed(totalStatBoost);
        }
    }
}
