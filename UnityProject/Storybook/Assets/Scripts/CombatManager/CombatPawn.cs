using UnityEngine;
using System.Collections;

public abstract class CombatPawn : MonoBehaviour {

    // Character stats
    private int m_speed;
    private int m_health;

    private bool m_hasSubmittedMove = false;
    private bool m_isAlive = true;

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
    /// Subtract the given damage amount from the health of the pawn
    /// </summary>
    /// <param name="damageAmount">The amount to subtract from the health of this pawn</param>
    public void DealDamageToPawn(int damageAmount)
    {
        m_health -= damageAmount;
        if (m_health <= 0)
        {
            m_isAlive = false;
        }
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

    /// <summary>
    /// The HP value for the combat pawn
    /// </summary>
    public int Health
    {
        get { return m_health; }
        set { m_health = value; }
    }
    
    /// <summary>
    /// True if the player's health is above 0, false otherwise
    /// </summary>
    public bool IsAlive
    {
        get { return m_isAlive; }
        set { m_isAlive = value; }
    }

    /// <summary>
    /// True if the pawn has submitted a move for this turn, false otherwise
    /// </summary>
    public bool HasSubmittedMove
    {
        get { return m_hasSubmittedMove; }
        set { m_hasSubmittedMove = value; }
    }
}
