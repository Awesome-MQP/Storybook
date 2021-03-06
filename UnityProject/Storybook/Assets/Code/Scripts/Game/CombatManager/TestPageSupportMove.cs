﻿using UnityEngine;
using System.Collections;

public class TestPageSupportMove : PageMove {

    [SerializeField]
    private int m_speedIncrease = 3;

    private const bool IS_MOVE_ATTACK = false;

    // Use this for initialization
    void Start () {
        SetIsMoveAttack(IS_MOVE_ATTACK);
    }

    /// <summary>
    /// Increases the speed stat of all pawns in the target list
    /// </summary>
    protected override void DoMoveEffect()
    {
        foreach (CombatPawn playerPawn in MoveTargets)
        {
            playerPawn.IncreasePawnSpeed(m_speedIncrease);
        }
    }
}
