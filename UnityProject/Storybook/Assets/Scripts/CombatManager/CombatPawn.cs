using UnityEngine;
using System.Collections;

public abstract class CombatPawn : MonoBehaviour {

    private int m_speed;

    private CombatManager m_combatManager;

    private bool m_isInAction = false;

    private bool m_isActionComplete = false;

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

    public bool IsActionComplete
    {
        get { return m_isActionComplete; }
        set { m_isActionComplete = value; }
    }

    public int Speed
    {
        get { return m_speed; }
        set { m_speed = value; }
    }
}
