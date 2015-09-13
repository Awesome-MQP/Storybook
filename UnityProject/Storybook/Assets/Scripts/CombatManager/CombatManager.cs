using UnityEngine;
using System.Collections;

public class CombatManager : MonoBehaviour {

    private Animator m_combatStateMachine;
    private ThinkState m_thinkState;
    private ExecuteState m_executeState;
    private WinState m_winState;
    private LoseState m_loseState;

	// Use this for initialization
	void Start () {
        m_combatStateMachine = GetComponent<Animator>() as Animator;
        m_combatStateMachine.SetTrigger("StartCombat");

        // Get the ThinkState and set the CombatManager
        m_thinkState = m_combatStateMachine.GetBehaviour<ThinkState>() as ThinkState;
        m_thinkState.CManager = this;

        // Get the ExecuteState and set the CombatManager
        m_executeState = m_combatStateMachine.GetBehaviour<ExecuteState>() as ExecuteState;
        m_executeState.CManager = this;

        // Get the WinState and set the CombatManager
        m_winState = m_combatStateMachine.GetBehaviour<WinState>() as WinState;
        m_winState.CManager = this;

        // Get the LoseState and set the Combat Manager
        m_loseState = m_combatStateMachine.GetBehaviour<LoseState>() as LoseState;
        m_loseState.CManager = this;
    }

}
