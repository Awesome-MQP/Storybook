using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatManager : MonoBehaviour {

    [SerializeField]
    private TestCombatPawn m_combatPawnPrefab;

    private ThinkState m_thinkState;
    private ExecuteState m_executeState;
    private WinState m_winState;
    private LoseState m_loseState;

    private TestCombatPawn m_combatPawn;
    private int m_submittedMoves = 0;
    private List<CombatPawn> m_playerList = new List<CombatPawn>();

	// Use this for initialization
	void Start () {
        Animator m_combatStateMachine = GetComponent<Animator>() as Animator;
        m_combatStateMachine.SetTrigger("StartCombat");

        CombatState[] allCombatStates = m_combatStateMachine.GetBehaviours<CombatState>() as CombatState[];
        foreach (CombatState cm in allCombatStates)
        {
            cm.CManager = this;
        }

        m_combatPawn = Instantiate(m_combatPawnPrefab);
        CombatPawn combatPawnScript = m_combatPawn.GetComponent<CombatPawn>();
        combatPawnScript.SetCombatManager(this);
        m_playerList.Add(combatPawnScript);
    }

    public void SubmitPlayerMove() {
        m_submittedMoves += 1;
    }
}
