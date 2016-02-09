using UnityEngine;
using System.Collections;

public class PageDefenseBoost : PageMove {

    [SerializeField]
    private int m_defenseIncrease;

    void Awake()
    {
        SetIsMoveAttack(false);
    }

    protected override void DoMoveEffect()
    {
        foreach (CombatPawn pawn in MoveTargets)
        {
            Debug.Log("Increasing pawn defense");
            pawn.IncreasePawnDefense(m_defenseIncrease);
        }
    }
}
