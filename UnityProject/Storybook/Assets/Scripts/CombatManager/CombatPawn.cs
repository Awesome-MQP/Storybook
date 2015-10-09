using UnityEngine;
using System.Collections;

public abstract class CombatPawn : MonoBehaviour {

    // Character stats
    [SerializeField]
    private float m_health;
    private float m_healthMod = 0;

    [SerializeField]
    private float m_speed;
    private float m_speedBoost = 0;
    private float m_speedMod = 0; 

    [SerializeField]
    private float m_defense;
    private float m_defenseBoost = 0;
    private float m_defenseMod = 0;

    [SerializeField]
    private float m_attack;
    private float m_attackBoost = 0;
    private float m_attackMod = 0;

    [SerializeField]
    private Genre m_genre;

    private bool m_hasSubmittedMove = false;
    private bool m_isAlive = true;
    private bool m_isActionComplete = false;

    private CombatManager m_combatManager;

    // Defaults to null because it needs to be able to return null moves
    private CombatMove m_moveForTurn = null;

    public abstract void OnThink();

    /// <summary>
    /// Setter for the pawn's CombatManager
    /// </summary>
    /// <param name="newCombatManager"></param>
    public void RegisterCombatManager(CombatManager newCombatManager) {
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
    /// Increases the pawn speed by the given amount
    /// </summary>
    /// <param name="speedIncrease">The amount to increase the speed by</param>
    public void IncreasePawnSpeed(int speedIncrease)
    {
        m_speed += speedIncrease;
    }

    /// <summary>
    /// The CombatManager that the CombatPawn is involved
    /// </summary>
    public CombatManager CManager
    {
        get{ return m_combatManager; }
    }

    /// <summary>
    /// True if the CombatPawn has executed its action for the turn, false otherwise
    /// </summary>
    public bool IsActionComplete
    {
        get { return m_isActionComplete; }
    }

    public void SetIsActionComplete(bool newIsActionComplete)
    {
        m_isActionComplete = newIsActionComplete;
    }

    /// <summary>
    /// The speed value (stat) for the combat pawn, used to determine turn order
    /// </summary>
    public float Speed
    {
        get { return m_speed + m_speedBoost; }
    }

    public void SetSpeed(float newSpeed)
    {
        m_speed = newSpeed;
    }

    /// <summary>
    /// The HP value for the combat pawn
    /// </summary>
    public float Health
    {
        get { return m_health; }
        set { m_health = value; }
    }

    public void SetHealth(float newHealth)
    {
        m_health = newHealth;
    }
    
    /// <summary>
    /// True if the player's health is above 0, false otherwise
    /// </summary>
    public bool IsAlive
    {
        get { return m_isAlive; }
    }

    /// <summary>
    /// True if the pawn has submitted a move for this turn, false otherwise
    /// </summary>
    public bool HasSubmittedMove
    {
        get { return m_hasSubmittedMove; }
    }

    public void SetHasSubmittedMove(bool newHasSubmittedMove)
    {
        m_hasSubmittedMove = newHasSubmittedMove;
    }

    /// <summary>
    /// The move that the combat pawn is going to execute in the current turn of combat
    /// </summary>
    public CombatMove MoveForTurn
    {
        get { return m_moveForTurn; }
    }

    public void SetMoveForTurn(CombatMove newMoveForTurn)
    {
        m_moveForTurn = newMoveForTurn;
    }

    /// <summary>
    /// Resets move variables of the combat pawn
    /// </summary>
    public void ResetMove()
    {
        m_moveForTurn = null;
        m_hasSubmittedMove = false;
    }

    public Genre PawnGenre
    {
        get { return m_genre; }
    }

    public void DecrementBoosts()
    {
        if (m_speedBoost > 0)
        {
            m_speedBoost -= 1;
        }
    }
}
