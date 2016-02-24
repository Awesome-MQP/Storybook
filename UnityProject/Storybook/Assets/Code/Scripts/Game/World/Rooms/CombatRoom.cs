using System.Collections;
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

    protected void Start()
    {
        Renderer floorRenderer = m_floorObject.GetComponent<Renderer>();
        floorRenderer.material = _getFloorMaterial();
        _setRoomMusic();
    }

    // On entering the room, do nothing since there is nothing special in this room.
    protected override void OnRoomEnter(RoomMover mover)
    {
        if (!(mover is BasePlayerMover))
            return;

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
                GameObject pawnGameObject = PhotonNetwork.Instantiate("Enemies/" + pawn.PawnGenre + "/" + pawn.name, currentEnemyPos, currentEnemyRot, 0);
                pawnGameObject.GetComponent<CombatPawn>().enabled = false;
                PhotonNetwork.Spawn(pawnGameObject.GetComponent<PhotonView>());
                m_enemyWorldPawns.Add(pawnGameObject);
                i++;
            }
            //m_musicManager.Fade(m_musicTracks[1], 5, true);
            return;
        }
        m_musicManager.Fade(m_musicTracks[0], 5, true);

        return;
    }

    protected override IEnumerable OnRoomEvent(RoomMover mover)
    {
        //TODO: Integrate with new game manager code to start combat
        if (!(mover is BasePlayerMover))
            yield break;

        //TODO: This code can be moved into the combat manager, seeing as it is the combats music.
        if (!m_wonCombat)
        {
            m_musicManager.Fade(m_musicTracks[1], 5, true);
            //TODO: m_gameManager.TransitionToCombat();
        }
        else
        {
            //TODO: We should just halt with yield return null
            m_musicManager.Fade(m_musicTracks[0], 5, true);
            ClearRoom();
        }
    }

    protected override void OnRoomExit(RoomMover mover)
    {
        if (!(mover is BasePlayerMover))
            return;

        m_wonCombat = true;
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

    private Material _getFloorMaterial()
    {
        Material floorMaterial = Resources.Load("FloorTiles/fantasy-tile") as Material;
        switch (RoomPageData.PageGenre)
        {
            case Genre.SciFi:
                floorMaterial = Resources.Load("FloorTiles/sci-fi-tile") as Material;
                break;
            case Genre.Fantasy:
                floorMaterial = Resources.Load("FloorTiles/fantasy-tile") as Material;
                break;
            case Genre.GraphicNovel:
                floorMaterial = Resources.Load("FloorTiles/comic-book-tile") as Material;
                break;
            case Genre.Horror:
                floorMaterial = Resources.Load("FloorTiles/horror-tile") as Material;
                break;
        }
        return floorMaterial;
    }

    // Similar to get floor material, set the room's music based on the genre
    private void _setRoomMusic()
    {
        switch(RoomPageData.PageGenre)
        {
            case Genre.GraphicNovel:
                m_musicTracks[0] = m_musicTracks[2];
                break;
            case Genre.Fantasy:
                m_musicTracks[0] = m_musicTracks[3];
                break;
            case Genre.Horror:
                m_musicTracks[0] = m_musicTracks[4];
                break;
            case Genre.SciFi:
                m_musicTracks[0] = m_musicTracks[5];
                break;
            default:
                m_musicTracks[0] = m_musicTracks[0];
                break;
        }
    }
}
