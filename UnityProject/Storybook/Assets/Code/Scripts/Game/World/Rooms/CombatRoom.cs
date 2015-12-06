using UnityEngine;
using System.Collections.Generic;

// This is an empty room. There is nothing special about it.
// No events will occur upon entering this room.
public class CombatRoom : RoomObject {

    [SerializeField]
    private List<GameObject> m_roomEnemiesOverworld = new List<GameObject>();
    [SerializeField]
    private EnemyTeam m_roomEnemies;

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
        OnRoomEnter();
	}

    // On entering the room, do nothing since there is nothing special in this room.
    protected override void OnRoomEnter()
    {
        if (!m_wonCombat)
        {
            _chooseEnemyTeam();
            m_gameManager.EnemyTeamForCombat = m_roomEnemies;

            int x = 0, y = 2, z = 1, i = 0;

            // TODO: Change this to work over network
            
            foreach (CombatPawn pawn in m_roomEnemies.PawnsToSpawn)
            {
                m_enemyPawns[i] = (CombatPawn) Instantiate(pawn.gameObject, new Vector3(x, y, z), Quaternion.identity);
                m_enemyPawns[i].gameObject.SetActive(true);
                z -= 2;
                i++;
            }
        }
        return;
    }

    // What do we do when all players reach the center of the room?
    // Most likely nothing, but that may change.
    protected override void OnRoomEvent()
    {
        // TODO: Transition into Combat.
        // Use DungeonMovement for this.
        m_gameManager.StartCombat();
        return;
    }

    // What happens when the players leave this room?
    // Hint: Nothing.
    protected override void OnRoomExit()
    {
        foreach(CombatPawn go in m_enemyPawns)
        {
            m_enemyPawns.Remove(go);
            Destroy(go);
        }
        return;
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
