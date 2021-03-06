﻿using UnityEngine;
using System.Collections;

public abstract class CombatState : StateMachineBehaviour {

    private Animator m_stateMachine;
    private CombatManager m_combatManager;

    public abstract void ExitState();

    /// <summary>
    /// The Animator that acts as the state machine for combat
    /// </summary>
    public Animator StateMachine
    {
        get { return m_stateMachine; }
        set { m_stateMachine = value; }
    }

    /// <summary>
    /// The CombatManager that the CombatState is tied to
    /// </summary>
    public CombatManager CManager
    {
        get { return m_combatManager; }
    }

    public void SetCombatManager(CombatManager newCombatManager)
    {
        m_combatManager = newCombatManager;
    }

    /// <summary>
    /// Sets the state machine to the given animator
    /// </summary>
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        StateMachine = animator;
    }

    /// <summary>
    /// Resets all of the bools that are used to transition from state to state to false
    /// </summary>
    protected void ResetBools()
    {
        m_stateMachine.SetBool("StartToThink", false);
        m_stateMachine.SetBool("ThinkToExecute", false);
        m_stateMachine.SetBool("ExecuteToThink", false);
        m_stateMachine.SetBool("ExecuteToWin", false);
        m_stateMachine.SetBool("ExecuteToLose", false);
        m_stateMachine.SetBool("ExitCombat", false);
    }
}
