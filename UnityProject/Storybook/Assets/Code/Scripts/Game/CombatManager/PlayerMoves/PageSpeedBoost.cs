using UnityEngine;
using System.Collections;

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
            pawn.IncreasePawnSpeed(m_speedIncrease);
        }
    }
}
