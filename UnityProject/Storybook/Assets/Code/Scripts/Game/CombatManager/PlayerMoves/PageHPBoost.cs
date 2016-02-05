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
            pawn.IncreasePawnHP(m_HPIncrease);
        }
    }

}
