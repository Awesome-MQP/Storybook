using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

public class CombatManager : Photon.PunBehaviour, IConstructable<CombatInstance>
{

    [SerializeField]
    private Transform m_cameraPos;

    [SerializeField]
    private GameObject m_combatUIPrefab;

    private List<CombatTeam> m_teamList = new List<CombatTeam>();

    private Animator m_combatStateMachine;
    private CombatState m_currentState;

    private Dictionary<CombatPawn, CombatMove> m_pawnToCombatMove = new Dictionary<CombatPawn, CombatMove>();

    private AudioClip m_previousMusic;
    private AudioClip m_combatMusic;

    private int m_combatLevel;
    private Genre m_combatGenre;

    private bool m_isRunning = true;

    override protected void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void Construct(CombatInstance combatInfo)
    {
        Assert.IsTrue(IsMine);

        m_combatLevel = combatInfo.GetLevel();
        m_combatGenre = combatInfo.GetGenre();
        Debug.Log("Combat Level: " + m_combatLevel + " Genre: " + m_combatGenre);

        m_previousMusic = combatInfo.GetPreviousMusic();
        m_combatMusic = combatInfo.GetCombatMusic();

        FindObjectOfType<GameManager>().GetComponent<MusicManager>().Fade(m_combatMusic, 5, true);

        CombatTeam[] teams = combatInfo.CreateTeams();
        m_teamList = new List<CombatTeam>(teams);

        foreach (CombatTeam team in teams)
        {

            Debug.Log("Combat Team: " + team.name);
            PhotonNetwork.Spawn(team.photonView);
        }
    }

    public override void OnStartOwner(bool wasSpawn)
    {
        _startup();
    }

    public override void OnStartPeer(bool wasSpawn)
    {
        _startup();

        m_combatStateMachine.enabled = false;
    }

    private void _startup()
    {
        Instantiate(m_combatUIPrefab);

        Camera.main.transform.position = CameraPos.position;
        Camera.main.transform.rotation = Quaternion.identity;

        m_combatStateMachine = GetComponent<Animator>();

        // Send out a tutorial event
        EventDispatcher.GetDispatcher<TutorialEventDispatcher>().OnCombatStarted();
    }

    // Use this for initialization
    //TODO use network safe start
    void Start()
    {
        if(!IsMine)
        {
            m_combatStateMachine.enabled = false;
        }
        else
        {
            foreach (CombatTeam team in m_teamList)
            {
                team.RegisterCombatManager(this);
                team.TeamLevel = m_combatLevel;
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

    public void RemovePawnMove(CombatPawn pawnToRemove)
    {
        m_pawnToCombatMove.Remove(pawnToRemove);
    }

    /// <summary>
    /// Called when a new turn is beginning
    /// Called by the ThinkState
    /// </summary>
    public void StartNewTurn()
    {
        foreach (CombatTeam team in m_teamList)
        {
            team.StartNewTurn();
        }
        m_currentState = m_combatStateMachine.GetBehaviour<ThinkState>();
        m_pawnToCombatMove = new Dictionary<CombatPawn, CombatMove>();
    }

    private void _spawnTeams()
    {
        int teamId = 0;
        foreach (CombatTeam team in m_teamList)
        {
            team.TeamId = teamId;
            team.SpawnTeam();
            team.StartCombat();
            teamId++;
        }
    }

    /// <summary>
    /// Ends the current combat by calling the GameManager EndCombat function
    /// </summary>
    public void EndCurrentCombat()
    {
        //FindObjectOfType<GameManager>().EndCombat();

        DestroyAllTeams();
        Destroy(gameObject);

        m_isRunning = false;
        FindObjectOfType<GameManager>().GetComponent<MusicManager>().Fade(m_previousMusic, 5, true);
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

    public Transform CameraPos
    {
        get { return m_cameraPos; }
    }

    public int TeamsInCombat
    {
        get { return m_teamList.Count; }
    }

    /// <summary>
    /// Destroys all of the teams in the combat
    /// </summary>
    public void DestroyAllTeams()
    {
        foreach(CombatTeam team in m_teamList)
        {
            team.EndCombat();
            // TODO: Change back to just calling Destroy when that is fixed]
            PhotonNetwork.Destroy(team.gameObject);
            Destroy(team.gameObject);
        }
    }

    public void DestroyAllPages()
    {
        Page[] listOfAllPages = FindObjectsOfType<Page>();
        foreach(Page p in listOfAllPages)
        {
            // TODO: Change back to just calling Destroy when that is fixed
            PhotonNetwork.Destroy(p.gameObject);
            Destroy(p.gameObject);
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
        foreach(CombatTeam team in m_teamList)
        {
            if (team.TeamId == teamId)
            {
                return team;
            }
        }
        return null;
    }

    [SyncProperty]
    public int CombatLevel
    {
        get { return m_combatLevel; }
        private set
        {
            Assert.IsTrue(ShouldBeChanging);
            m_combatLevel = value;
            PropertyChanged();
        }
    }

    [SyncProperty]
    public Genre CombatGenre
    {
        get { return m_combatGenre; }
        set
        {
            Assert.IsTrue(ShouldBeChanging);
            m_combatGenre = value;
            PropertyChanged();
        }
    }

    public bool IsRunning
    {
        get { return m_isRunning; }
    }
}
