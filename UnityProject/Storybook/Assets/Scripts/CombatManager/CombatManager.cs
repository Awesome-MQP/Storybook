﻿using UnityEngine;
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
    private List<PlayerEntity> m_playerEntityList = new List<PlayerEntity>();
    private List<PlayerPositionNode> m_playerPositionList = new List<PlayerPositionNode>();
    private List<EnemyPositionNode> m_enemyPositionList = new List<EnemyPositionNode>();
    private Animator m_combatStateMachine;
    private CombatState m_currentState;

    private Dictionary<CombatPawn, CombatMove> m_pawnToCombatMove = new Dictionary<CombatPawn, CombatMove>();

    // Use this for initialization
    void Start()
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

        // Spawn and place the player pawns
        _spawnPlayerPawns(1);
        _placePlayers();

        // Spawn and place the enemy pawns
        _spawnEnemyPawns(1);
        _placeEnemies();

        // Default current state to think state
        m_currentState = m_combatStateMachine.GetBehaviour<ThinkState>();
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

    public void SubmitMove(CombatPawn combatPawn, CombatMove moveForTurn)
    {
        m_pawnToCombatMove.Add(combatPawn, moveForTurn);
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
        Debug.Log("Player Health = " + m_playerPawnList[0].Health);
        Debug.Log("Enemy Health = " + m_enemyList[0].Health);
        m_submittedMoves = 0;
        m_submittedEnemyMoves = 0;
        ResetPawnActions();
        m_currentState = m_combatStateMachine.GetBehaviour<ThinkState>();
        m_pawnToCombatMove = new Dictionary<CombatPawn, CombatMove>();
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
    private void _spawnEnemyPawns(int numberToSpawn)
    {
        for (int i = 0; i < numberToSpawn; i++)
        {
            CombatEnemy enemyObject = (CombatEnemy)Instantiate(m_enemyPawnPrefab, transform.position, Quaternion.identity);
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
}
