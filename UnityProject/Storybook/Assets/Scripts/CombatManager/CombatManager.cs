using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CombatManager : MonoBehaviour {

    [SerializeField]
    private CombatPawn m_combatPawnPrefab;

    private CombatPawn m_combatPawn;
    private int m_submittedMoves = 0;
    private List<CombatPawn> m_playerList = new List<CombatPawn>();

	// Use this for initialization
	void Start () {
        Animator m_combatStateMachine = GetComponent<Animator>() as Animator;
        m_combatStateMachine.SetTrigger("StartCombat");

        CombatState[] allCombatStates = m_combatStateMachine.GetBehaviours<CombatState>();
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
