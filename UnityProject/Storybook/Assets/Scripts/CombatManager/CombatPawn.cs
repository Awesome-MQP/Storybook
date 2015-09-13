using UnityEngine;
using System.Collections;

public abstract class CombatPawn : MonoBehaviour {

    private CombatManager m_combatManager;

    public abstract IEnumerator OnThink();

    public abstract void OnAction();

    public void SetCombatManager(CombatManager newCombatManager) {
        m_combatManager = newCombatManager;
    }

    public CombatManager CManager
    {
        get{return m_combatManager;}
    }
}
