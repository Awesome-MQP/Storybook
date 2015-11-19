using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CombatManager : Photon.PunBehaviour {

    [SerializeField]
    private CombatPawn m_combatPawnPrefab;

    [SerializeField]
    private CombatPawn m_enemyPawnPrefab;

    [SerializeField]
    private Transform m_cameraPos;

    private CombatPawn[] m_enemiesToSpawn;

    private int m_submittedMoves = 0;
    private int m_submittedEnemyMoves = 0;
    private List<CombatPawn> m_allPawnsActive = new List<CombatPawn>();
    private List<CombatPawn> m_pawnsSpawned = new List<CombatPawn>();

    [SerializeField]
    private List<CombatTeam> m_teamList = new List<CombatTeam>();

    private Animator m_combatStateMachine;
    private CombatState m_currentState;

    private Dictionary<CombatPawn, CombatMove> m_pawnToCombatMove = new Dictionary<CombatPawn, CombatMove>();

    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    // Use this for initialization
    void Start()
    {
        Camera.main.transform.position = CameraPos.position;
        Camera.main.transform.rotation = Quaternion.identity;

        Debug.Log("Teams in combat = " + m_teamList.Count);

        // Get the state machine and get it out of the start state by setting the StartCombat trigger
        m_combatStateMachine = GetComponent<Animator>();
        if (PhotonNetwork.isMasterClient)
        {
            foreach (CombatTeam team in m_teamList)
            {
                team.RegisterCombatManager(this);
            }
            _spawnTeams();

            // Set the combat manager of all the combat state machine states to this object
            CombatState[] allCombatStates = m_combatStateMachine.GetBehaviours<CombatState>();
            foreach (CombatState cm in allCombatStates)
            {
                cm.SetCombatManager(this);
            }

            // Default current state to think state
            m_currentState = m_combatStateMachine.GetBehaviour<ThinkState>();

            m_combatStateMachine.SetBool("StartToThink", true);
        }
        else
        {
            m_combatStateMachine.enabled = false;
            /*
            CombatTeam[] teamList = FindObjectsOfType<CombatTeam>();
            foreach(CombatTeam team in teamList)
            {
                m_teamList.Add(team);
            }
            */
        }
            
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
            m_pawnToCombatMove[combatPawn] = moveForTurn;
        }
    }

    /// <summary>
    /// Called when a new turn is beginning
    /// Called by the ThinkState
    /// </summary>
    public void StartNewTurn()
    {
        m_submittedMoves = 0;
        m_submittedEnemyMoves = 0;
        foreach (CombatTeam team in m_teamList)
        {
            team.StartNewTurn();
        }
        m_currentState = m_combatStateMachine.GetBehaviour<ThinkState>();
        m_pawnToCombatMove = new Dictionary<CombatPawn, CombatMove>();
    }

    private void _spawnTeams()
    {
        foreach (CombatTeam team in m_teamList)
        {
            team.SpawnTeam();
            team.StartCombat();
            m_allPawnsActive.AddRange(team.ActivePawnsOnTeam);
        }
        m_pawnsSpawned = new List<CombatPawn>(m_allPawnsActive.ToArray());
    }

    /// <summary>
    /// Ends the current combat by calling the GameManager EndCombat function
    /// </summary>
    public void EndCurrentCombat()
    {
        FindObjectOfType<GameManager>().EndCombat();
    }

    /// <summary>
    /// Removes the given player from the combat by removing them from the pawn list and the PawnToMove dictionary
    /// </summary>
    /// <param name="playerToRemove">The player to remove</param>
    public void RemovePlayerFromCombat(CombatPawn playerToRemove)
    {
        m_allPawnsActive.Remove(playerToRemove);
        m_pawnToCombatMove.Remove(playerToRemove);
    }

    /// <summary>
    /// Removes the given enemy from the combat by removing them from the enemy list and the PawnToMove dictionary
    /// </summary>
    /// <param name="enemyToRemove">The enemy to remove</param>
    public void RemoveAIFromCombat(CombatAI enemyToRemove)
    {
        m_allPawnsActive.Remove(enemyToRemove);
        m_pawnToCombatMove.Remove(enemyToRemove);
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
    public CombatPawn[] AllPawnsActive
    {
        get
        {
            List<CombatPawn> allPawns = new List<CombatPawn>();
            foreach(CombatTeam team in m_teamList)
            {
                allPawns.AddRange(team.ActivePawnsOnTeam);
            }
            return allPawns.ToArray();
        }
    }

    public void SetAllPawns(List<CombatPawn> allPawns)
    {
        m_allPawnsActive = allPawns;
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

    public Transform CameraPos
    {
        get { return m_cameraPos; }
    }

    public int TeamsInCombat
    {
        get { return m_teamList.Count; }
    }

    /// <summary>
    /// Destroys the game object of all of the pawns that are in the current combat
    /// </summary>
    public void DestroyAllPawns()
    {
        foreach(CombatPawn pawn in m_pawnsSpawned)
        {
            Destroy(pawn.gameObject);
        }
    }

    public CombatTeam[] TeamList
    {
        get { return m_teamList.ToArray(); }
    }

    public void SetCombatTeamList(List<CombatTeam> teamList)
    {
        m_teamList = teamList;
    }

    public void RegisterTeamLocal(CombatTeam teamToRegister)
    {
        m_teamList.Add(teamToRegister);
    }

    /// <summary>
    /// Searches for the CombatTeam with the given teamId
    /// </summary>
    /// <param name="teamId">The ID of the team to get</param>
    /// <returns>The team with the given ID or null if no team with that ID exists</returns>
    public CombatTeam GetTeamById(int teamId)
    {
        Debug.Log("Looking for team " + teamId);
        foreach(CombatTeam team in m_teamList)
        {
            Debug.Log("TeamId = " + team.TeamId);
            if (team.TeamId == teamId)
            {
                return team;
            }
        }
        return null;
    }
}
