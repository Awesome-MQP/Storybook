using UnityEngine;
using System.Collections;

public abstract class CombatPawn : MonoBehaviour {

    private int m_speed;

    private CombatManager m_combatManager;

    private bool m_isInAction = false;

    private bool m_isActionComplete = false;

    public abstract IEnumerator OnThink();

    public abstract void OnAction();

    /// <summary>
    /// Setter for the pawn's CombatManager
    /// </summary>
    /// <param name="newCombatManager"></param>
    public void SetCombatManager(CombatManager newCombatManager) {
        m_combatManager = newCombatManager;
    }

    /// <summary>
    /// The CombatManager that the CombatPawn is involved
    /// </summary>
    public CombatManager CManager
    {
        get{return m_combatManager;}
    }

    /// <summary>
    /// True if the CombatPawn is currently executing its action animation, false otherwise
    /// </summary>
    public bool IsInAction
    {
        get { return m_isInAction; }
        set { m_isInAction = value; }
    }

    /// <summary>
    /// True if the CombatPawn has executed its action for the turn, false otherwise
    /// </summary>
    public bool IsActionComplete
    {
        get { return m_isActionComplete; }
        set { m_isActionComplete = value; }
    }

    /// <summary>
    /// The speed value (stat) for the combat pawn, used to determine turn order
    /// </summary>
    public int Speed
    {
        get { return m_speed; }
        set { m_speed = value; }
    }
}
