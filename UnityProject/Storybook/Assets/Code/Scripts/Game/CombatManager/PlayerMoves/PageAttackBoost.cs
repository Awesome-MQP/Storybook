using UnityEngine;
using System.Collections;

public class PageAttackBoost : PageMove {

    [SerializeField]
    private int m_attackIncrease;

    void Awake()
    {
        SetIsMoveAttack(false);
    }

    protected override void DoMoveEffect()
    {
        foreach (CombatPawn pawn in MoveTargets)
        {
            Debug.Log("Increasing pawn attack");
            pawn.IncreasePawnAttack(m_attackIncrease);
        }
    }
}
