using UnityEngine;
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

}
