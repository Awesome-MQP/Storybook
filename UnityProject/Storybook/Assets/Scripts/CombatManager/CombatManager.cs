using UnityEngine;
using System.Collections;

public class CombatManager : MonoBehaviour {

    private Animator combatStateMachine;
    private ThinkState thinkState;
    private ExecuteState executeState;
    private WinState winState;
    private LoseState loseState;

	// Use this for initialization
	void Start () {
        combatStateMachine = GetComponent<Animator>() as Animator;
        combatStateMachine.SetTrigger("StartCombat");

        // Get the ThinkState and set the CombatManager
        thinkState = combatStateMachine.GetBehaviour<ThinkState>() as ThinkState;
        thinkState.setCombatManager(this);

        // Get the ExecuteState and set the CombatManager
        executeState = combatStateMachine.GetBehaviour<ExecuteState>() as ExecuteState;
        executeState.setCombatManager(this);

        // Get the WinState and set the CombatManager
        winState = combatStateMachine.GetBehaviour<WinState>() as WinState;
        winState.setCombatManager(this);

        // Get the LoseState and set the Combat Manager
        loseState = combatStateMachine.GetBehaviour<LoseState>() as LoseState;
        loseState.setCombatManager(this);
    }
	
	// Update is called once per frame
	void Update () {
       
	}

    public void think() {
        Debug.Log("Combat Manager running think code");
        combatStateMachine.SetTrigger("ThinkToExecute");
    }

    public void executeTurn() {
        Debug.Log("Combat Manager running execute code");
        combatStateMachine.SetTrigger("ExecuteToWin");
    }

    public void win() {
        Debug.Log("Combat Manager running win code");
        combatStateMachine.SetTrigger("ExitCombat");
    }

    public void lose() {
        Debug.Log("Combat Manager running lose code");
        combatStateMachine.SetTrigger("ExitCombat");
    }

}
