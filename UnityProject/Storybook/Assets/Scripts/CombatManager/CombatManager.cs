using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CombatManager : MonoBehaviour {

    [SerializeField]
    private CombatPawn m_combatPawnPrefab;

    private CombatPawn m_combatPawn;
    private int m_submittedMoves = 0;
    private List<CombatPawn> m_playerPawnList = new List<CombatPawn>();
    private List<PlayerEntity> m_playerEntityList = new List<PlayerEntity>();
    private List<PlayerPositionNode> m_playerPositionList = new List<PlayerPositionNode>();
    private Animator m_combatStateMachine;

	// Use this for initialization
	void Start () {
        m_combatStateMachine = GetComponent<Animator>() as Animator;
        m_combatStateMachine.SetTrigger("StartCombat");

        CombatState[] allCombatStates = m_combatStateMachine.GetBehaviours<CombatState>();
        foreach (CombatState cm in allCombatStates)
        {
            cm.CManager = this;
        }
        Debug.Log("Spawning combat pawn");
        _spawnCombatPawns(1);
        _placePlayers();
    }

    // Called by CombatPawn when a player has submitted their move
    public void SubmitPlayerMove() {
        Debug.Log("Submitting player move");
        m_submittedMoves += 1;
    }

    // Called by combat pawn when its attack animation has completed
    public void PlayerFinishedMoving()
    {
        m_combatStateMachine.GetBehaviour<ExecuteState>().IncrementPawnIndex();
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

    public List<PlayerPositionNode> PlayerPositions
    {
        get { return m_playerPositionList; }
        set { m_playerPositionList = value; }
    }

    public int MovesSubmitted
    {
        get { return m_submittedMoves; }
        set { m_submittedMoves = value; }
    }

    // Places the players at the player points on the combat plane
    private void _placePlayers()
    {
        Debug.Log("Placing players");
        for (int i = 0; i < m_playerPawnList.Count; i++)
        {
            Debug.Log("Placing player");
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
        }
    }
}
