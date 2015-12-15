using UnityEngine;
using System.Collections.Generic;

// This is an empty room. There is nothing special about it.
// No events will occur upon entering this room.
public class CombatRoom : RoomObject {

    [SerializeField]
    private List<GameObject> m_roomEnemiesOverworld = new List<GameObject>();
    [SerializeField]
    private EnemyTeam m_roomEnemies;

    [SerializeField]
    private List<Transform> m_enemyPosList = new List<Transform>();

    private List<CombatPawn> m_enemyPawns = new List<CombatPawn>();

    private CombatManager m_combatManager;
    private GameManager m_gameManager = null;

    private bool m_wonCombat = false;

	// Use this for initialization
	protected override void Awake ()
    {
        base.Awake();
        m_gameManager = FindObjectOfType<GameManager>();
        // CombatTeam.m_activePawnsOnTeam is the list of all combat pawns
        m_enemyPawns = m_gameManager.EnemyTeamForCombat.ActivePawnsOnTeam;
	}

    // On entering the room, do nothing since there is nothing special in this room.
    public override void OnRoomEnter()
    {
        if (!m_wonCombat)
        {
            _chooseEnemyTeam();
            m_gameManager.EnemyTeamForCombat = m_roomEnemies;

            int i = 0;

            // TODO: Change this to work over network
            /*
            foreach (CombatPawn pawn in m_roomEnemies.PawnsToSpawn)
            {
                Vector3 currentEnemyPos = m_enemyPosList[i].position;
                GameObject pawnGameObject = PhotonNetwork.Instantiate(pawn.name, currentEnemyPos, Quaternion.identity, 0);
                PhotonNetwork.Spawn(pawnGameObject.GetComponent<PhotonView>());
                m_enemyPawns.Add(pawnGameObject.GetComponent<CombatPawn>());
                i++;
            }
            */
        }
        return;
    }

    public override void OnRoomEvent()
    {
        m_gameManager.TransitionToCombat();
        return;
    }

    public override void OnRoomExit()
    {
        /*
        foreach(CombatPawn go in m_enemyPawns)
        {
            m_enemyPawns.Remove(go);
            Destroy(go);
        }
        return;
        */
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
