using UnityEngine;
using UnityEngine.UI;
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

        m_combatPawn = (GameObject) Instantiate(m_combatPawnPrefab);
        CombatPawn combatPawnScript = m_combatPawn.GetComponent<CombatPawn>();
        combatPawnScript.SetCombatManager(this);
        m_playerList.Add(combatPawnScript);
    }

    public void SubmitPlayerMove() {
        m_submittedMoves += 1;
    }

    public void PageButtonPressed()
    {
        Debug.Log("Page button pressed");
    }

    public void ItemButtonPressed()
    {
        Debug.Log("Item button pressed");
    }

    public void InfoButtonPressed()
    {
        Debug.Log("Info button pressed");
    }

    public void RunButtonPressed()
    {
        Debug.Log("Run button pressed");
    }
}
