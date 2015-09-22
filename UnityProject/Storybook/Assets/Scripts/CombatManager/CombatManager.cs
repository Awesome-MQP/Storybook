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

    // Use this for initialization
    void Start() {
        m_combatStateMachine = GetComponent<Animator>() as Animator;
        m_combatStateMachine.SetTrigger("StartCombat");

        CombatState[] allCombatStates = m_combatStateMachine.GetBehaviours<CombatState>();
        foreach (CombatState cm in allCombatStates)
        {
            cm.CManager = this;
        }
        _spawnCombatPawns(1);
        _placePlayers();
        _spawnEnemyPawns(1);
        _placeEnemies();
    }

    // Called by CombatPawn when a player has submitted their move
    public void SubmitPlayerMove() {
        m_submittedMoves += 1;
    }

    // Called by enemies when they select their move
    public void SubmitEnemyMove()
    {
        m_submittedEnemyMoves += 1;
    }

    // Called by combat pawn when its attack animation has completed
    public void PlayerFinishedMoving()
    {
        m_combatStateMachine.GetBehaviour<ExecuteState>().GetNextCombatPawn();
    }

    // Called by enemy pawn when its attack animation has completed
    public void EnemyFinishedMoving()
    {
        m_combatStateMachine.GetBehaviour<ExecuteState>().GetNextCombatPawn();
    }

    // Places the players at the player points on the combat plane
    private void _placePlayers()
    {
        for (int i = 0; i < m_playerPawnList.Count; i++)
        {
            CombatPawn currentPawn = m_playerPawnList[i];
            currentPawn.transform.position = m_playerPositionList[i].transform.position;
        }
    }

    private void _spawnCombatPawns(int numberToSpawn)
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

    private void _placeEnemies()
    {
        for (int i = 0; i < m_enemyList.Count; i++)
        {
            CombatPawn currentPawn = m_enemyList[i];
            currentPawn.transform.position = m_enemyPositionList[i].transform.position;
        }
    }

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

    public List<PlayerEntity> PlayerList
    {
        get { return m_playerEntityList; }
        set { m_playerEntityList = value; }
    }

    public List<CombatPawn> PawnList
    {
        get { return m_playerPawnList; }
        set { m_playerPawnList = value; }
    }

    public List<CombatEnemy> EnemyList
    {
        get { return m_enemyList; }
        set { m_enemyList = value; }
    }

    public List<PlayerPositionNode> PlayerPositions
    {
        get { return m_playerPositionList; }
        set { m_playerPositionList = value; }
    }

    public List<EnemyPositionNode> EnemyPositions
    {
        get { return m_enemyPositionList; }
        set { m_enemyPositionList = value; }
    }

    public int MovesSubmitted
    {
        get { return m_submittedMoves; }
        set { m_submittedMoves = value; }
    }

    public List<CombatPawn> AllPawns
    {
        get { return m_allPawns; }
        set { m_allPawns = value; }
    }
}
