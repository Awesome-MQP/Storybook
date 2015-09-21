using UnityEngine;
using System.Collections;

public abstract class CombatPawn : MonoBehaviour {

    private CombatManager m_combatManager;

    private bool m_isInAction = false;

    public abstract IEnumerator OnThink();

    public abstract void OnAction();

    public void SetCombatManager(CombatManager newCombatManager) {
        m_combatManager = newCombatManager;
    }

    public CombatManager CManager
    {
        get{return m_combatManager;}
    }

    public bool IsInAction
    {
        get { return m_isInAction; }
        set { m_isInAction = value; }
    }
}
