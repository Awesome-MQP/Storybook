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
    private AudioClip[] m_musicTracks; // This array holds all music tracks for a room, in an effort to make it more general. 
                                       // To make accessing tracks from this more easy to follow, use this standard for putting tracks into the array
                                       // INDEX | TRACK
                                       // 0.......RoomMusic
                                       // 1.......FightMusic
                                       // 2+......Miscellaneous

    [SerializeField]
    private List<GameObject> m_roomEnemiesOverworld = new List<GameObject>();
    [SerializeField]
    private EnemyTeam m_roomEnemies;
    private string m_roomEnemiesPrefabLoc;

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
        m_musicManager.MusicTracks = m_musicTracks;
        if (!m_wonCombat)
        {
            _chooseEnemyTeam();
            //m_gameManager.EnemyTeamForCombat = m_roomEnemies;
            //m_gameManager.EnemyTeamPrefabLoc = m_roomEnemiesPrefabLoc;


            int i = 0;

            // TODO: Change this to work over network
            foreach (CombatPawn pawn in m_roomEnemies.PawnsToSpawn)
            {
                Vector3 currentEnemyPos = m_enemyPosList[i].position;
                Quaternion currentEnemyRot = m_enemyPosList[i].rotation;
                GameObject pawnGameObject = PhotonNetwork.Instantiate(pawn.name, currentEnemyPos, currentEnemyRot, 0);
                pawnGameObject.GetComponent<CombatPawn>().enabled = false;
                PhotonNetwork.Spawn(pawnGameObject.GetComponent<PhotonView>());
                m_enemyWorldPawns.Add(pawnGameObject);
                i++;
            }
            StartCoroutine(m_musicManager.Fade(m_musicTracks[1], 5, true));
            return;
        }
        StartCoroutine(m_musicManager.Fade(m_musicTracks[0], 5, true));

        return;
    }

    public override void OnRoomEvent()
    {
        if (!m_wonCombat)
        {
            StartCoroutine(m_musicManager.Fade(m_musicTracks[1], 5, true));
            //TODO: m_gameManager.TransitionToCombat();
            return;
        }
        else
        {
            StartCoroutine(m_musicManager.Fade(m_musicTracks[0], 5, true));
            return;
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
        switch (RoomPageData.PageGenre)
        {
            case Genre.Fantasy:
                m_roomEnemiesPrefabLoc = "EnemyTeams/Fantasy/";
                teams = Resources.LoadAll("EnemyTeams/Fantasy");
                break;
            case Genre.GraphicNovel:
                m_roomEnemiesPrefabLoc = "EnemyTeams/Comic/";
                teams = Resources.LoadAll("EnemyTeams/Comic");
                break;
            case Genre.Horror:
                m_roomEnemiesPrefabLoc = "EnemyTeams/Horror/";
                teams = Resources.LoadAll("EnemyTeams/Horror");
                break;
            case Genre.SciFi:
                m_roomEnemiesPrefabLoc = "EnemyTeams/SciFi/";
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
