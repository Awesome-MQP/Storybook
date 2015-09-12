﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatManager : MonoBehaviour {

    [SerializeField]
    private GameObject m_combatPawnPrefab;

    private Animator m_combatStateMachine;
    private ThinkState m_thinkState;
    private ExecuteState m_executeState;
    private WinState m_winState;
    private LoseState m_loseState;

    private GameObject m_combatPawn;
    private int m_submittedMoves = 0;
    private List<CombatPawn> m_playerList = new List<CombatPawn>();

	// Use this for initialization
	void Start () {
        m_combatStateMachine = GetComponent<Animator>() as Animator;
        m_combatStateMachine.SetTrigger("StartCombat");

        // Get the ThinkState and set the CombatManager
        m_thinkState = m_combatStateMachine.GetBehaviour<ThinkState>() as ThinkState;
        m_thinkState.SetCombatManager(this);

        // Get the ExecuteState and set the CombatManager
        m_executeState = m_combatStateMachine.GetBehaviour<ExecuteState>() as ExecuteState;
        m_executeState.SetCombatManager(this);

        // Get the WinState and set the CombatManager
        m_winState = m_combatStateMachine.GetBehaviour<WinState>() as WinState;
        m_winState.SetCombatManager(this);

        // Get the LoseState and set the Combat Manager
        m_loseState = m_combatStateMachine.GetBehaviour<LoseState>() as LoseState;
        m_loseState.SetCombatManager(this);

        m_combatPawn = (GameObject) Instantiate(m_combatPawnPrefab);
        CombatPawn combatPawnScript = m_combatPawn.GetComponent<CombatPawn>();
        combatPawnScript.SetCombatManager(this);
        m_playerList.Add(combatPawnScript);
    }

    public void Think() {
        Debug.Log("Combat Manager running think code");
        
        // If all player moves have been submitted, execute the turn
        if (m_submittedMoves == m_playerList.Count) {
            Debug.Log("All player moves received");
            m_combatStateMachine.SetTrigger("ThinkToExecute");
        }
    }

    public void ExecuteTurn() {
        foreach (CombatPawn pawn in m_playerList) {
            pawn.OnAction();
        }
        Debug.Log("Combat Manager running execute code");
        m_combatStateMachine.SetTrigger("ExecuteToWin");
    }

    public void Win() {
        Debug.Log("Combat Manager running win code");
        m_combatStateMachine.SetTrigger("ExitCombat");
    }

    public void Lose() {
        Debug.Log("Combat Manager running lose code");
        m_combatStateMachine.SetTrigger("ExitCombat");
    }

    public void SubmitPlayerMove() {
        m_submittedMoves += 1;
        Debug.Log("Player move received");
    }
}
