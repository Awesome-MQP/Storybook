using UnityEngine;
using System.Collections;
using System;

public class PageHPBoost : PageMove
{

    [SerializeField]
    private int m_HPIncrease;

    void Awake()
    {
        SetIsMoveAttack(false);
    }

    protected override void DoMoveEffect()
    {
        foreach(CombatPawn pawn in MoveTargets)
        {
            Debug.Log("Increasing pawn HP");
            int totalStatBoost = m_HPIncrease;
            totalStatBoost += (int)Math.Ceiling((double)MoveLevel / 2); //This may look kind of hacky, pls don't yell at me :-) - Connor
            if (MoveGenre == this.MoveOwner.PawnGenre)
            {
                totalStatBoost += 2; // Give a small boost if the types match up. We won't take the full type chart into effect here.
            }
            pawn.IncreasePawnHP(totalStatBoost);
        }
    }

}
