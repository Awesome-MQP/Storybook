using UnityEngine;
using System.Collections.Generic;

// This is an empty room. There is nothing special about it.
// No events will occur upon entering this room.
public class CombatRoom : RoomObject {
    [SerializeField]
    private AudioClip m_roomMusic;

    [SerializeField]
    private AudioClip m_fightMusic;

    [SerializeField]
    private List<GameObject> m_roomEnemiesOverworld = new List<GameObject>();
    [SerializeField]
    private EnemyTeam m_roomEnemies;

    [SerializeField]
    private List<Transform> m_enemyPosList = new List<Transform>();

    private List<GameObject> m_enemyWorldPawns = new List<GameObject>();

    private CombatManager m_combatManager;
    private MusicManager m_musicManager;
    private GameManager m_gameManager = null;

    private bool m_wonCombat = false;

	// Use this for initialization
	protected override void Awake ()
    {
        base.Awake();
        m_gameManager = FindObjectOfType<GameManager>();
        // CombatTeam.m_activePawnsOnTeam is the list of all combat pawns
        // m_enemyWorldPawns = m_gameManager.EnemyTeamForCombat.ActivePawnsOnTeam;
        m_musicManager = FindObjectOfType<MusicManager>();
	}

    // On entering the room, do nothing since there is nothing special in this room.
    public override void OnRoomEnter()
    {
        m_musicManager.FightMusic = m_fightMusic;
        m_musicManager.RoomMusic = m_roomMusic;
        m_musicManager.Fade(m_roomMusic, 5, true);
        if (!m_wonCombat)
        {
            _chooseEnemyTeam();
            m_gameManager.EnemyTeamForCombat = m_roomEnemies;

            int i = 0;

            // TODO: Change this to work over network
            foreach (CombatPawn pawn in m_roomEnemies.PawnsToSpawn)
            {
                Vector3 currentEnemyPos = m_enemyPosList[i].position;
                Quaternion currentEnemyRot = m_enemyPosList[i].rotation;
                GameObject pawnGameObject = PhotonNetwork.Instantiate(pawn.name, currentEnemyPos, currentEnemyRot, 0);
                Debug.Log("Spawning pawn with name = " + pawn.name);
                pawnGameObject.GetComponent<CombatPawn>().enabled = false;
                PhotonNetwork.Spawn(pawnGameObject.GetComponent<PhotonView>());
                m_enemyWorldPawns.Add(pawnGameObject);
                i++;
            }

        }
        return;
    }

    public override void OnRoomEvent()
    {
        if (!m_wonCombat)
        {
            //m_musicManager.Fade(m_fightMusic, 5, true);
            m_gameManager.TransitionToCombat();
            return;
        }
        if(m_wonCombat)
        {
            //m_musicManager.Fade(m_roomMusic, 5, true);
        }
    }

    public override void OnRoomExit()
    {
        m_wonCombat = true;
        return;
    }

    public void DestroyEnemyWorldPawns()
    {
        foreach (GameObject go in m_enemyWorldPawns)
        {
            PhotonNetwork.Destroy(go);
            Destroy(go);
        }
    }

    // Change the value of the room's "won" status
    public void SetWonStatus(bool won)
    {
        m_wonCombat = won;
    }

    private void _chooseEnemyTeam()
    {
        Object[] teams = null;
        switch (RoomGenre)
        {
            case Genre.Fantasy:
                teams = Resources.LoadAll("EnemyTeams/Fantasy");
                break;
            case Genre.GraphicNovel:
                teams = Resources.LoadAll("EnemyTeams/Comic");
                break;
            case Genre.Horror:
                teams = Resources.LoadAll("EnemyTeams/Horror");
                break;
            case Genre.SciFi:
                teams = Resources.LoadAll("EnemyTeams/SciFi");
                break;
        }

        if (teams != null)
        {
            GameObject enemyTeam = (GameObject) teams[Random.Range(0, teams.Length)];
            m_roomEnemies = enemyTeam.GetComponent<EnemyTeam>();
        }
    }
}
