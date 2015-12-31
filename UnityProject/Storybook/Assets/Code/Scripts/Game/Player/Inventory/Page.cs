using UnityEngine;
using System.Collections.Generic;
using System;

public class Page : Item  {

    // Is it rare? Rare pages affect all targets, not just one.
    [SerializeField]
    private bool m_isRare = false;

    // Page level
    [SerializeField]
    private int m_pageLevel = 0;

    // Page Genre
    [SerializeField]
    private Genre m_pageGenre;

    // Move Type
    [SerializeField]
    private MoveType m_pageMoveType;

    // Initialize mods to 0. They are set based on Genre when the page is initiated
    private int m_strengthMod = 0;
    private int m_hitpointsMod = 0;
    private int m_speedMod = 0;
    private int m_defenseMod = 0;

    // Game Manager, pulled from the scene
    GameManager m_gameManager;

    // ==Leftovers from the shift from CombatMove.============================================
    private float m_timeSinceMoveStarted = 0;
    private bool m_isMoveEffectDone = false;
    private bool m_isMoveComplete = false;

    private List<CombatPawn> m_moveTargets = new List<CombatPawn>();
    private CombatPawn[] m_allTargets;
    private CombatPawn m_singleTarget;
    private CombatPawn m_moveOwner;

    private int m_numberOfTargets;

    [SerializeField]
    private PlayerMove m_playerCombatMove;

	// Use this for initialization
	protected override void Awake () {
        base.Awake();
    }

    public void InitializePage()
    {
        m_timeSinceMoveStarted = 0;
        m_isMoveEffectDone = false;
        m_isMoveComplete = false;
        switch (PageGenre)
        {
            case Genre.Fantasy:
                HitpointsMod = PageLevel;
                break;
            case Genre.GraphicNovel:
                SpeedMod = PageLevel;
                break;
            case Genre.Horror:
                StrengthMod = PageLevel;
                break;
            case Genre.SciFi:
                DefenseMod = PageLevel;
                break;
            default:
                break;
        }
    }

    void OnPageUse()
    {
        // First get the gamemanager so we can get the teams
        m_gameManager = FindObjectOfType<GameManager>();

        switch (m_pageMoveType)
        {
            case MoveType.Attack:
                UseAttackMove();
                break;
            case MoveType.Boost:
                UseBoostMove();
                break;
            case MoveType.Status:
                UseStatusMove();
                break;
            default:
                break;
        }
    }

    public virtual void ExecuteMove()
    {
        TimeSinceMoveStarted += Time.deltaTime;
        if (TimeSinceMoveStarted >= 0.5 && !IsMoveEffectDone)
        {
            OnPageUse();
            IsMoveEffectDone = true;
        }
        else if (TimeSinceMoveStarted >= 1)
        {
            Debug.Log("Page move is complete");
            IsMoveComplete = true;
        }
    }

    //=EFFECTS OF PAGE MOVES: ATTACKS, SUPPORTS, and STATUS AILMENT===============================
    // Determines the effect of an attack move
    protected virtual void UseAttackMove()
    {
        int moveDamage = 0;

        if (m_isRare) // Rare page, target all enemies
        {
            m_allTargets = m_gameManager.EnemyTeamForCombat.ActivePawnsOnTeam.ToArray();
            foreach (CombatPawn enemyPawn in m_allTargets)
            {
                moveDamage = StatsManager.CalcDamage(m_moveOwner.PawnGenre,
                                                         enemyPawn.PawnGenre,
                                                         PageGenre,
                                                         PageLevel,
                                                         m_moveOwner.Attack,
                                                         enemyPawn.Defense);
                enemyPawn.DealDamageToPawn(moveDamage);
            }
        }
        else // Not a rare page, so only single target
        {
            moveDamage = StatsManager.CalcDamage(m_moveOwner.PawnGenre,
                                                         m_singleTarget.PawnGenre,
                                                         PageGenre,
                                                         PageLevel,
                                                         m_moveOwner.Attack,
                                                         m_singleTarget.Defense);
            m_singleTarget.DealDamageToPawn(moveDamage);
        }
    }

    // Determines the effect of a support move
    protected virtual void UseBoostMove()
    {
        if (m_isRare) // Rare page, target all allies
        {
            m_allTargets = m_gameManager.PlayerTeamForCombat.ActivePawnsOnTeam.ToArray();
            foreach (CombatPawn allyPawn in m_allTargets)
            {
                allyPawn.IncreasePawnSpeed(PageLevel);
            }
        }
        else // Not a rare page, so only single target
        {
            m_singleTarget.IncreasePawnSpeed(PageLevel);
        }
    }

    // Determines the effect of a status move
    protected virtual void UseStatusMove()
    {
        if(m_isRare) // Rare page, target all foes
        {
            m_allTargets = m_gameManager.EnemyTeamForCombat.ActivePawnsOnTeam.ToArray();
            foreach(CombatPawn enemyPawn in m_allTargets)
            {
                enemyPawn.InflictStatus(PageLevel);
            }
        }
        else // Not a rare page, so only a single target
        {
            m_singleTarget.InflictStatus(PageLevel);
        }
    }

    //=INHERITED CLASSES FROM ITEM================================================================
    protected override void OnPickup()
    {
        // TODO in Player (I think), make the player set itself as the owner of this page.
    }

    protected override void OnDrop()
    {
        //TODO
    }

    protected override void OnMoved()
    {
        //TODO
    }

    //=PROPERTIES=================================================================================
    [SyncProperty]
    public int PageLevel
    {
        get { return m_pageLevel; }
        set
        {
            m_pageLevel = value;
            PropertyChanged();
        }
    }
    
    [SyncProperty]
    public Genre PageGenre
    {
        get { return m_pageGenre; }
        set
        {
            m_pageGenre = value;
            PropertyChanged();
        }
    }
    
    public int StrengthMod
    {
        get { return m_strengthMod; }
        set { m_strengthMod = value; }
    }

    public int SpeedMod
    {
        get { return m_speedMod; }
        set { m_speedMod = value; }
    }

    public int DefenseMod
    {
        get { return m_defenseMod; }
        set { m_defenseMod = value; }
    }

    public int HitpointsMod
    {
        get { return m_hitpointsMod; }
        set { m_hitpointsMod = value; }
    }

    [SyncProperty]
    public bool Rarity
    {
        get { return m_isRare; }
        set
        {
            m_isRare = value;
            PropertyChanged();
        }
    }

    [SyncProperty]
    public MoveType PageType
    {
        get { return m_pageMoveType; }
        set
        {
            m_pageMoveType = value;
            PropertyChanged();
        }
    }

    public float TimeSinceMoveStarted
    {
        get { return m_timeSinceMoveStarted; }
        set { m_timeSinceMoveStarted = value; }
    }

    public bool IsMoveComplete
    {
        get { return m_isMoveComplete; }
        set { m_isMoveComplete = value; }
    }

    public bool IsMoveEffectDone
    {
        get { return m_isMoveEffectDone; }
        set { m_isMoveEffectDone = value; }
    }

    public CombatPawn PageOwner
    {
        get { return m_moveOwner; }
        set { m_moveOwner = value; }
    }

    public List<CombatPawn> MoveTargets
    {
        get { return m_moveTargets; }
        set { m_moveTargets = value; }
    }

    public int NumberOfTargets
    {
        get { return m_numberOfTargets; }
        set { m_numberOfTargets = value; }
    }

    public PlayerMove PlayerCombatMove
    {
        get { return m_playerCombatMove; }
    }
}
