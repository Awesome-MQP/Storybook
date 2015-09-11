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
    }

    public void Think() {
        Debug.Log("Combat Manager running think code");
        m_combatStateMachine.SetTrigger("ThinkToExecute");
    }

    public void ExecuteTurn() {
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

}
