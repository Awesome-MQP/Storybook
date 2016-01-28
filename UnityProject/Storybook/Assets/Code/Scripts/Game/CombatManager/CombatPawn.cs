using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class CombatPawn : Photon.PunBehaviour
{

    private const int MAX_STATUS_TURNS = 7;

    // Character stats
    [SerializeField]
    private float m_maxHealth;

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

    private bool m_underStatusEffect = false;
    private int m_turnsUnderStatus = 0;

    private CombatManager m_combatManager;

    // Defaults to null because it needs to be able to return null moves
    private CombatMove m_moveForTurn = null;

    private int m_pawnId = -1;

    private static PhotonView m_scenePhotonView = null;

    private int m_teamId;

    private CombatTeam m_pawnTeam;

    public abstract void OnThink();

    public void Start()
    {
        DontDestroyOnLoad(this);
        m_combatManager = FindObjectOfType<CombatManager>();
        m_scenePhotonView = GetComponent<PhotonView>();
    }

    /// <summary>
    /// Setter for the pawn's CombatManager
    /// </summary>
    /// <param name="newCombatManager"></param>
    public void RegisterCombatManager(CombatManager newCombatManager)
    {
        m_combatManager = newCombatManager;
    }

    /// <summary>
    /// Subtract the given damage amount from the health of the pawn
    /// </summary>
    /// <param name="damageAmount">The amount to subtract from the health of this pawn</param>
    public void DealDamageToPawn(int damageAmount)
    {
        _playHurtAnimation();
        m_health -= damageAmount;

        if (m_health <= 0)
        {
            m_isAlive = false;
        }
    }

    private void _playHurtAnimation()
    {
        Animator pawnAnimator = GetComponent<Animator>();
        pawnAnimator.SetBool("IdleToIdle", false);
        pawnAnimator.SetBool("HurtToIdle", false);
        pawnAnimator.SetBool("IdleToHurt", true);
    }

    public void SwitchToIdleAnim()
    {
        Animator pawnAnimator = GetComponent<Animator>();
        pawnAnimator.SetBool("IdleToIdle", true);
        pawnAnimator.SetBool("IdleToHurt", false);
        pawnAnimator.SetBool("HurtToIdle", true);
    }

    /// <summary>
    /// Inflict a status effect on this pawn. Currently, just flips a boolean to say that
    /// this pawn is under a status effect. 
    /// Duration of status is also set, and is capped by a const above.
    /// </summary>
    /// <param name="potency">The strength of the status effect, as determined by page level.</param>
    public void InflictStatus(int potency)
    {
        m_underStatusEffect = true;
        m_turnsUnderStatus += Mathf.CeilToInt(potency / 2);
        if (m_turnsUnderStatus > MAX_STATUS_TURNS)
        {
            m_turnsUnderStatus = MAX_STATUS_TURNS;
        }
    }

    ///<summary>
    /// Deals effects of status, and decrements the status counter.
    /// </summary>
    public void DealStatusEffect()
    {

        // TODO: Status things.

        m_turnsUnderStatus--;
        if (m_turnsUnderStatus <= 0)
        {
            m_underStatusEffect = false;
            Debug.Log("Status has dissipated!");
            return;
        }
    }

    /// <summary>
    /// Increases the pawn speed boost by the given amount
    /// Adds to the speed boost value so that it will be decremented each turn
    /// </summary>
    /// <param name="speedIncrease">The amount to increase the speed by</param>
    public void IncreasePawnSpeed(int speedIncrease)
    {
        m_speedBoost += speedIncrease;
    }

    /// <summary>
    /// Increases the pawn HP by the given amount
    /// </summary>
    /// <param name="hpIncrease">The amount to increase the hp by</param>
    public void IncreasePawnHP(int hpIncrease)
    {
        m_health += hpIncrease;
        if (m_health > m_maxHealth)
        {
            m_health = m_maxHealth;
        }
    }

    /// <summary>
    /// Increases the pawn attack boost by the given amount
    /// Adds the the attack boost value so that it will be decremented each turn
    /// </summary>
    /// <param name="attackIncrease">The amount to increase the attack by</param>
    public void IncreasePawnAttack(int attackIncrease)
    {
        m_attackBoost += attackIncrease;
    }

    /// <summary>
    /// Increases the pawn defense boost by the given amount
    /// Adds to the defense boost value so that it will be decremented each turn
    /// </summary>
    /// <param name="defenseIncrease">The amount to increase the defense by</param>
    public void IncreasePawnDefense(int defenseIncrease)
    {
        m_defenseBoost += defenseIncrease;
    }

    /// <summary>
    /// The CombatManager that the CombatPawn is involved
    /// </summary>
    public CombatManager CManager
    {
        get { return m_combatManager; }
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
        get { return m_speed + m_speedBoost + m_speedMod; }
        set { m_speed = value; }
    }

    /// <summary>
    /// The HP value for the combat pawn
    /// </summary>
    public float Health
    {
        get { return m_health; }
        set { m_health = value; }
    }

    public void SetMaxHealth(float newMaxHealth)
    {
        m_maxHealth = newMaxHealth;
    }


    // ==PROPERTIES FOR STAT MODS===================================================

    [SyncProperty]
    public float AttackMod
    {
        get { return m_attackMod; }
        set
        {
            m_attackMod = value;
            PropertyChanged();
        }
    }

    [SyncProperty]
    public float DefenseMod
    {
        get { return m_defenseMod; }
        set
        {
            m_defenseMod = value;
            PropertyChanged();
        }
    }

    [SyncProperty]
    public float SpeedMod
    {
        get { return m_speedMod; }
        set
        {
            m_speedMod = value;
            PropertyChanged();
        }
    }

    [SyncProperty]
    public float HitpointsMod
    {
        get { return m_healthMod; }
        set
        {
            m_healthMod = value;
            PropertyChanged();
        }
    }

    // ==============================================================================

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

    public float Attack
    {
        get { return m_attack + m_attackBoost + m_attackMod; }
        set { m_attack = value; }
    }

    public float Defense
    {
        get { return m_defense + m_defenseBoost + m_defenseMod; }
        set { m_defense = value; }
    }

    [SyncProperty]
    public int PawnId
    {
        get { return m_pawnId; }
        set
        {
            m_pawnId = value;
            PropertyChanged();
        }
    }

    [SyncProperty]
    public int TeamId
    {
        get { return m_teamId; }
        set
        {
            m_teamId = value;
            PropertyChanged();
        }
    }

    public PhotonView ScenePhotonView
    {
        get { return m_scenePhotonView; }
    }

    public void SetTeamId(byte teamId)
    {
        m_teamId = teamId;
    }

    /// <summary>
    /// Gets an array of all the pawns on the same team as this one
    /// </summary>
    /// <returns>The array of pawns that are on the same team as the current pawn</returns>
    public CombatPawn[] GetPawnsOnTeam()
    {
        return m_pawnTeam.ActivePawnsOnTeam.ToArray();
    }

    /// <summary>
    /// Gets an array of all the pawns on the opposing teams of this pawn
    /// </summary>
    /// <param name="pawnsToSearch">The list of pawns to search through</param>
    /// <returns>The array of pawns that are on opposing teams as the current pawn</returns>
    public CombatPawn[] GetPawnsOpposing()
    {
        CombatManager combatManager = FindObjectOfType<CombatManager>();

        List<CombatPawn> opposingPawns = new List<CombatPawn>();
        CombatTeam[] allTeams = combatManager.TeamList;
        foreach (CombatTeam team in allTeams)
        {
            if (team != m_pawnTeam)
            {
                opposingPawns.AddRange(team.ActivePawnsOnTeam);
            }
        }
        return opposingPawns.ToArray();
    }


    /// <summary>
    /// The team that the pawn is currently a part of
    /// </summary>
    protected CombatTeam PawnTeam
    {
        get { return m_pawnTeam; }
    }

    public void RegisterTeam(CombatTeam team)
    {
        m_pawnTeam = team;
    }

    /// <summary>
    /// Calls an RPC to add the pawn to the team on all clients
    /// </summary>
    public void SendPawnTeam(int teamId)
    {
        m_scenePhotonView = GetComponent<PhotonView>();
        m_scenePhotonView.RPC("RPCAddPawnToTeam", PhotonTargets.Others, teamId);
    }

    /// <summary>
    /// Adds the pawn to its corresponding team in the combat manager
    /// </summary>
    [PunRPC]
    public void RPCAddPawnToTeam(int teamId)
    {
        CombatManager combatManager = FindObjectOfType<CombatManager>();
        TeamId = teamId;
        CombatTeam pawnTeam = combatManager.GetTeamById(m_teamId);
        pawnTeam.AddPawnToTeam(this);
        RegisterTeam(pawnTeam);
    }

    public void AddOrRemoveMod(Genre modToAdd, int pageLevel, bool isAdd)
    {
        switch (modToAdd)
        {
            case Genre.Fantasy:
                if (isAdd)
                {
                    Debug.Log("Adding speed mod");
                    SpeedMod += pageLevel;
                }
                else
                {
                    SpeedMod -= pageLevel;
                }
                break;

            case Genre.GraphicNovel:
                if (isAdd)
                {
                    Debug.Log("Adding attack mod");
                    AttackMod += pageLevel;
                }
                else
                {
                    AttackMod -= pageLevel;
                }
                break;

            case Genre.Horror:
                if (isAdd)
                {
                    Debug.Log("Adding HP mod");
                    HitpointsMod += pageLevel;
                }
                else
                {
                    HitpointsMod -= pageLevel;
                }
                break;

            case Genre.SciFi:
                if (isAdd)
                {
                    Debug.Log("Adding defense mod");
                    DefenseMod += pageLevel;
                }
                else
                {
                    DefenseMod -= pageLevel;
                }
                break;
        }
    }
}
