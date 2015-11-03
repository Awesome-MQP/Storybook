using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TestCombatPawn : CombatPlayer
{
    private CombatNetworker m_combatNetworker = null;

    void Start()
    {
        m_combatNetworker = FindObjectOfType<CombatNetworker>();
    }

    void Update()
    {
        if (m_combatNetworker == null)
        {
            m_combatNetworker = FindObjectOfType<CombatNetworker>();
        }
    }

    // Waits for input of a move
    public override void OnThink()
    {
        // TODO - Change back to player hitting space bar to select the first move
        if (PlayerId == PhotonNetwork.player.ID && Input.GetKeyDown(KeyCode.Space) && m_combatNetworker != null)
        {
            Debug.Log("Space bar pressed");
            List<CombatPawn> targetList = new List<CombatPawn>();
            targetList.Add(CManager.EnemyList[0]);
            PlayerMove chosenMove = PlayerHand[0];
            chosenMove.SetMoveOwner(this);
            chosenMove.SetMoveTargets(targetList);
            chosenMove.InitializeMove();
            SetMoveForTurn(chosenMove);
            SetHasSubmittedMove(true);
            m_combatNetworker.SubmitMoveForPlayer(PlayerId, chosenMove);
        }
    }
}