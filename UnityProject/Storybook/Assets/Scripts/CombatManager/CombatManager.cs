using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CombatManager : MonoBehaviour {

    [SerializeField]
    private CombatPawn m_combatPawnPrefab;

    [SerializeField]
    private CombatPawn m_enemyPawnPrefab;

    private CombatPawn m_combatPawn;
    private int m_submittedMoves = 0;
    private int m_submittedEnemyMoves = 0;
    private List<CombatPawn> m_playerPawnList = new List<CombatPawn>();
    private List<CombatEnemy> m_enemyList = new List<CombatEnemy>();
    private List<CombatPawn> m_allPawns = new List<CombatPawn>();
    private List<PlayerEntity> m_playerEntityList = new List<PlayerEntity>();
    private List<PlayerPositionNode> m_playerPositionList = new List<PlayerPositionNode>();
    private List<EnemyPositionNode> m_enemyPositionList = new List<EnemyPositionNode>();
    private Animator m_combatStateMachine;
    private CombatState m_currentState;

    // Use this for initialization
    void Start()
    {
        m_combatStateMachine = GetComponent<Animator>() as Animator;
        m_combatStateMachine.SetTrigger("StartCombat");

        CombatState[] allCombatStates = m_combatStateMachine.GetBehaviours<CombatState>();
        foreach (CombatState cm in allCombatStates)
        {
            cm.CManager = this;
        }

        // Spawn and place the player pawns
        _spawnPlayerPawns(1);
        _placePlayers();

        // Spawn and place the enemy pawns
        _spawnEnemyPawns(1);
        _placeEnemies();

        // Call the OnThink function on all of the combat pawns
        _setAllPawnsToThink();

        // Default current state to think state
        m_currentState = m_combatStateMachine.GetBehaviour<ThinkState>();
    }

    /// <summary>
    /// Called by CombatPawn when a player has submitted their move
    /// </summary>
    public void SubmitPlayerMove() {
        Debug.Log("Submitting player move");
        m_submittedMoves += 1;
    }

    /// <summary>
    /// Called by enemies when they select their move
    /// </summary>
    public void SubmitEnemyMove()
    {
        m_submittedEnemyMoves += 1;
    }

    /// <summary>
    /// Called by combat pawn when its attack animation has completed
    /// </summary>
    public void PlayerFinishedMoving()
    {
        m_combatStateMachine.GetBehaviour<ExecuteState>().GetNextCombatPawn();
    }

    /// <summary>
    /// Called by enemy pawn when its attack animation has completed
    /// </summary>
    public void EnemyFinishedMoving()
    {
        m_combatStateMachine.GetBehaviour<ExecuteState>().GetNextCombatPawn();
    }

    /// <summary>
    /// Called when a new turn is beginning
    /// Called by the ThinkState
    /// </summary>
    public void StartNewTurn()
    {
        Debug.Log("CombatManager starting new turn");
        Debug.Log("Player Health = " + PlayerPawnList[0].Health);
        Debug.Log("Enemy Health = " + EnemyList[0].Health);
        m_submittedMoves = 0;
        m_submittedEnemyMoves = 0;
        ResetPawnActions();
        _setAllPawnsToThink();
        m_currentState = m_combatStateMachine.GetBehaviour<ThinkState>();
    }

    /// <summary>
    /// Ends the current combat by calling the GameManager EndCombat function
    /// </summary>
    public void EndCurrentCombat()
    {
        FindObjectOfType<GameManager>().EndCombat(this);
    }

    /// <summary>
    /// Places the players at the player points on the combat plane
    /// </summary>
    private void _placePlayers()
    {
        for (int i = 0; i < m_playerPawnList.Count; i++)
        {
            CombatPawn currentPawn = m_playerPawnList[i];
            currentPawn.transform.position = m_playerPositionList[i].transform.position;
        }
    }

    /// <summary>
    /// Spawns the given number of combat pawns to represent the players
    /// </summary>
    /// <param name="numberToSpawn">The number of combat pawns to create</param>
    private void _spawnPlayerPawns(int numberToSpawn)
    {
        for (int i = 0; i < numberToSpawn; i++)
        {
            m_combatPawn = (CombatPawn)Instantiate(m_combatPawnPrefab, transform.position, Quaternion.identity);
            CombatPawn combatPawnScript = m_combatPawn.GetComponent<CombatPawn>();
            combatPawnScript.SetCombatManager(this);
            m_playerPawnList.Add(combatPawnScript);
            m_allPawns.Add(combatPawnScript);
        }
    }

    /// <summary>
    /// Places the enemy pawns at the nodes on the combat plane
    /// </summary>
    private void _placeEnemies()
    {
        for (int i = 0; i < m_enemyList.Count; i++)
        {
            CombatPawn currentPawn = m_enemyList[i];
            currentPawn.transform.position = m_enemyPositionList[i].transform.position;
        }
    }

    /// <summary>
    /// Spawns the given number of CombatEnemy objects
    /// </summary>
    /// <param name="numberToSpawn">The number of CombatEnemy objects to spawn</param>
    private void _spawnEnemyPawns(int numberToSpawn)
    {
        for (int i = 0; i < numberToSpawn; i++)
        {
            CombatEnemy enemyObject = (CombatEnemy)Instantiate(m_enemyPawnPrefab, transform.position, Quaternion.identity);
            enemyObject.SetCombatManager(this);
            m_enemyList.Add(enemyObject);
            m_allPawns.Add(enemyObject);
        }
    }

    /// <summary>
    /// Tells all the pawns in the combat to start their OnThink() functions
    /// </summary>
    private void _setAllPawnsToThink()
    {
        foreach (CombatPawn combatPawn in AllPawns)
        {
            combatPawn.HasSubmittedMove = false;
            StartCoroutine(combatPawn.OnThink());
        }
    }

    /// <summary>
    /// Resets the IsActionComplete boolean in all of the pawns to false
    /// </summary>
    public void ResetPawnActions()
    {
        foreach (CombatPawn combatPawn in AllPawns)
        {
            combatPawn.IsActionComplete = false;
        }
    }

    /// <summary>
    /// The list of all the PlayerEntity in the combat
    /// </summary>
    public List<PlayerEntity> PlayerList
    {
        get { return m_playerEntityList; }
        set { m_playerEntityList = value; }
    }

    /// <summary>
    /// The list of all the player pawns in the combat
    /// </summary>
    public List<CombatPawn> PlayerPawnList
    {
        get { return m_playerPawnList; }
        set { m_playerPawnList = value; }
    }

    /// <summary>
    /// The list of all the enemies in the combat
    /// </summary>
    public List<CombatEnemy> EnemyList
    {
        get { return m_enemyList; }
        set { m_enemyList = value; }
    }

    /// <summary>
    /// The list of the PlayerPositionNodes in the combat
    /// </summary>
    public List<PlayerPositionNode> PlayerPositions
    {
        get { return m_playerPositionList; }
        set { m_playerPositionList = value; }
    }

    /// <summary>
    /// The list of EnemyPositionNodes in the combat
    /// </summary>
    public List<EnemyPositionNode> EnemyPositions
    {
        get { return m_enemyPositionList; }
        set { m_enemyPositionList = value; }
    }

    /// <summary>
    /// The number of moves submitted by the players
    /// </summary>
    public int MovesSubmitted
    {
        get { return m_submittedMoves; }
        set { m_submittedMoves = value; }
    }

    /// <summary>
    /// The list of all the pawns in the combat (enemies and players)
    /// </summary>
    public List<CombatPawn> AllPawns
    {
        get { return m_allPawns; }
        set { m_allPawns = value; }
    }

    /// <summary>
    /// The current state that the state machine is in
    /// </summary>
    public CombatState CurrentState
    {
        get { return m_currentState; }
        set { m_currentState = value; }
    }
}
