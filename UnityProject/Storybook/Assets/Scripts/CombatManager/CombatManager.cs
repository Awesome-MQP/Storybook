using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CombatManager : MonoBehaviour {

    [SerializeField]
    private CombatPawn m_combatPawnPrefab;

    [SerializeField]
    private CombatPawn m_enemyPawnPrefab;

    private CombatPawn[] m_enemiesToSpawn;

    private int m_submittedMoves = 0;
    private int m_submittedEnemyMoves = 0;
    private List<CombatPawn> m_playerPawnList = new List<CombatPawn>();
    private List<CombatEnemy> m_enemyList = new List<CombatEnemy>();
    private List<PlayerEntity> m_playerEntityList = new List<PlayerEntity>();
    private List<PlayerPositionNode> m_playerPositionList = new List<PlayerPositionNode>();
    private List<EnemyPositionNode> m_enemyPositionList = new List<EnemyPositionNode>();
    private Animator m_combatStateMachine;
    private CombatState m_currentState;
    private int m_playersToSpawn = 1;
    private CombatDemoUIHandler m_combatUI;

    private Dictionary<CombatPawn, CombatMove> m_pawnToCombatMove = new Dictionary<CombatPawn, CombatMove>();

    // Use this for initialization
    void Awake()
    {
        // Get the state machine and get it out of the start state by setting the StartCombat trigger
        m_combatStateMachine = GetComponent<Animator>() as Animator;
        m_combatStateMachine.SetTrigger("StartCombat");

        // Set the combat manager of all the combat state machine states to this object
        CombatState[] allCombatStates = m_combatStateMachine.GetBehaviours<CombatState>();
        foreach (CombatState cm in allCombatStates)
        {
            cm.SetCombatManager(this);
        }

        // Default current state to think state
        m_currentState = m_combatStateMachine.GetBehaviour<ThinkState>();

        m_combatUI = FindObjectOfType<CombatDemoUIHandler>();
    }

    public void StartCombat()
    {
        // Spawn and place the player pawns
        _spawnPlayerPawns(m_playersToSpawn);
        _placePlayers();

        // Spawn and place the enemy pawns
        _spawnEnemyPawns();
        _placeEnemies();

        _initializeDemoUI();
    }

    /// <summary>
    /// Called by CombatPawn when a player has submitted their move
    /// </summary>
    /// <param name="playerMove">The move that the player will do for the turn</param>
    public void SubmitPlayerMove(PlayerMove playerMove) {
        Debug.Log("Submitting player move");
        m_submittedMoves += 1;
    }

    /// <summary>
    /// Called by enemies when they select their move
    /// </summary>
    /// <param name="enemyMove">The move that the enemy will do for the turn</param>
    public void SubmitEnemyMove(EnemyMove enemyMove)
    {
        m_submittedEnemyMoves += 1;
    }

    /// <summary>
    /// Submits a move for a combat pawn and adds both to the dictionary
    /// </summary>
    /// <param name="combatPawn">The combat pawn that is submitting the move</param>
    /// <param name="moveForTurn">The move that is being submitted</param>
    public void SubmitMove(CombatPawn combatPawn, CombatMove moveForTurn)
    {
        if (!m_pawnToCombatMove.ContainsKey(combatPawn))
        {
            m_pawnToCombatMove.Add(combatPawn, moveForTurn);
        }
        else
        {
            m_pawnToCombatMove.Remove(combatPawn);
            m_pawnToCombatMove.Add(combatPawn, moveForTurn);
        }
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
        Debug.Log("Player 1 Health = " + m_playerPawnList[0].Health);
        Debug.Log("Enemy 1 Health = " + m_enemyList[0].Health);
        m_combatUI.EnableButtons();
        m_submittedMoves = 0;
        m_submittedEnemyMoves = 0;
        ResetPawnActions();
        _incrementEnemyMana();
        _decrementAllBoosts();
        m_currentState = m_combatStateMachine.GetBehaviour<ThinkState>();
        m_pawnToCombatMove = new Dictionary<CombatPawn, CombatMove>();
    }

    /// <summary>
    /// Ends the current combat by calling the GameManager EndCombat function
    /// </summary>
    public void EndCurrentCombat()
    {
        _stopDemoUI();
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
            CombatPawn combatPawn = (CombatPawn)Instantiate(m_combatPawnPrefab, transform.position, m_combatPawnPrefab.transform.rotation);
            CombatPawn combatPawnScript = combatPawn.GetComponent<CombatPawn>();
            combatPawnScript.RegisterCombatManager(this);
            m_playerPawnList.Add(combatPawnScript);
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
    private void _spawnEnemyPawns()
    {
        for (int i = 0; i < m_enemiesToSpawn.Length; i++)
        {
            CombatEnemy enemyObject = (CombatEnemy)Instantiate(m_enemiesToSpawn[i], transform.position, Quaternion.identity);
            enemyObject.RegisterCombatManager(this);
            m_enemyList.Add(enemyObject);
        }
    }

    /// <summary>
    /// Resets the IsActionComplete boolean in all of the pawns to false
    /// </summary>
    public void ResetPawnActions()
    {
        foreach (CombatPawn combatPawn in AllPawns)
        {
            combatPawn.SetIsActionComplete(false);
            combatPawn.ResetMove();
        }
    }

    /// <summary>
    /// Incremenets all of the enemy pawn mana for the turn
    /// </summary>
    private void _incrementEnemyMana()
    {
        foreach (CombatEnemy ce in m_enemyList)
        {
            ce.IncrementManaForTurn();
        }
    }

    /// <summary>
    /// Decrements all stat boosts of all the pawns for the turn
    /// </summary>
    private void _decrementAllBoosts()
    {
        foreach (CombatPawn combatPawn in AllPawns)
        {
            combatPawn.DecrementBoosts();
        }
    }

    /// <summary>
    /// Removes the given player from the combat by removing them from the pawn list and the PawnToMove dictionary
    /// </summary>
    /// <param name="playerToRemove">The player to remove</param>
    public void RemovePlayerFromCombat(CombatPawn playerToRemove)
    {
        m_playerPawnList.Remove(playerToRemove);
        m_pawnToCombatMove.Remove(playerToRemove);
    }

    /// <summary>
    /// Removes the given enemy from the combat by removing them from the enemy list and the PawnToMove dictionary
    /// </summary>
    /// <param name="enemyToRemove">The enemy to remove</param>
    public void RemoveEnemyFromCombat(CombatEnemy enemyToRemove)
    {
        m_enemyList.Remove(enemyToRemove);
        m_pawnToCombatMove.Remove(enemyToRemove);
    }

    // TODO - Remove from master
    private void _initializeDemoUI()
    {
        CombatDemoUIHandler combatUI = FindObjectOfType<CombatDemoUIHandler>();
        Debug.Log("Enemies = " + m_enemyList.Count);
        combatUI.SetPlayerPawns(m_playerPawnList.ToArray());
        combatUI.SetEnemyPawns(m_enemyList.ToArray());
        combatUI.SetIsCombatStarted(true);
    }

    private void _stopDemoUI()
    {
        CombatDemoUIHandler combatUI = FindObjectOfType<CombatDemoUIHandler>();
        combatUI.SetIsCombatStarted(false);
    }

    /// <summary>
    /// The list of all the PlayerEntity in the combat
    /// </summary>
    public IEnumerable<PlayerEntity> PlayerEntityList
    {
        get
        {
            foreach(PlayerEntity pe in m_playerEntityList)
            {
                yield return pe;
            }    
        }
    }

    public void SetPlayerEntityList(List<PlayerEntity> newPlayerList)
    {
        m_playerEntityList = newPlayerList;
    }

    /// <summary>
    /// The list of all the player pawns in the combat
    /// </summary>
    public CombatPawn[] PlayerPawnList
    {
        get { return m_playerPawnList.ToArray(); }
    }

    public void SetPlayerPawnList(List<CombatPawn> newPlayerPawnList)
    {
        m_playerPawnList = newPlayerPawnList;
    }

    /// <summary>
    /// The list of all the enemies in the combat
    /// </summary>
    public CombatEnemy[] EnemyList
    {
        get { return m_enemyList.ToArray(); }
    }

    public void SetEnemyList(List<CombatEnemy> newEnemyList)
    {
        m_enemyList = newEnemyList;
    }

    /// <summary>
    /// The list of the PlayerPositionNodes in the combat
    /// </summary>
    public IEnumerable<PlayerPositionNode> PlayerPositions
    {
        get
        {
            foreach(PlayerPositionNode ppn in m_playerPositionList)
            {
                yield return ppn;
            }
        }
    }

    public void SetPlayerPositions(List<PlayerPositionNode> newPlayerPositions)
    {
        m_playerPositionList = newPlayerPositions;
    }

    /// <summary>
    /// The list of EnemyPositionNodes in the combat
    /// </summary>
    public IEnumerable<EnemyPositionNode> EnemyPositions
    {
        get
        {
            foreach(EnemyPositionNode epn in m_enemyPositionList)
            {
                yield return epn;
            }
        }
    }

    public void SetEnemyPositions(List<EnemyPositionNode> newEnemyPositions)
    {
        m_enemyPositionList = newEnemyPositions;
    }

    /// <summary>
    /// The number of moves submitted by the players
    /// </summary>
    public int MovesSubmitted
    {
        get { return m_submittedMoves; }
    }

    /// <summary>
    /// The list of all the pawns in the combat (enemies and players)
    /// </summary>
    public IEnumerable<CombatPawn> AllPawns
    {
        get
        {
            foreach (CombatPawn playerPawn in m_playerPawnList)
            {
                yield return playerPawn;
            }
            foreach (CombatPawn enemyPawn in m_enemyList)
            {
                yield return enemyPawn;
            }
        }
    }

    /// <summary>
    /// The current state that the state machine is in
    /// </summary>
    public CombatState CurrentState
    {
        get { return m_currentState; }
    }

    public void SetCurrentState(CombatState newState)
    {
        m_currentState = newState;
    }

    public Dictionary<CombatPawn, CombatMove> PawnToMove
    {
        get { return m_pawnToCombatMove; }
    }

    /// <summary>
    /// The list of the enemy pawns to spawn in the combat
    /// </summary>
    /// <param name="enemiesToSpawn">The list of enemies that will be spawned</param>
    public void SetEnemiesToSpawn(CombatPawn[] enemiesToSpawn)
    {
        m_enemiesToSpawn = enemiesToSpawn;
    }

    /// <summary>
    /// The number of players that will be placed in the combat
    /// </summary>
    /// <param name="playersToSpawn">The new value for the number of players to place in the combat</param>
    public void SetPlayersToSpawn(int playersToSpawn)
    {
        m_playersToSpawn = playersToSpawn;
    }
}
