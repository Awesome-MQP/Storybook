using UnityEngine;
using System.Collections;

public abstract class CombatState : StateMachineBehaviour {

    private Animator m_stateMachine;
    private CombatManager m_combatManager;

    public abstract void ExitState();

    public Animator StateMachine
    {
        get { return m_stateMachine; }
        set { m_stateMachine = value; }
    }

    public CombatManager CManager
    {
        get { return m_combatManager; }
        set { m_combatManager = value; }
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        StateMachine = animator;
    }

}
