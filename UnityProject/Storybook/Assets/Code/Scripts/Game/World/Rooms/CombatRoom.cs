using UnityEngine;
using System.Collections.Generic;

// This is an empty room. There is nothing special about it.
// No events will occur upon entering this room.
public class CombatRoom : RoomObject {

    [SerializeField]
    private List<GameObject> m_roomEnemiesOverworld = new List<GameObject>();
    [SerializeField]
    private EnemyTeam m_roomEnemies;

    private List<Object> m_enemyPawns = new List<Object>();

    private CombatManager m_combatManager;
    private GameManager m_gameManager;

    private bool m_wonCombat = false;

	// Use this for initialization
	protected override void Awake ()
    {
        base.Awake();
        OnRoomEnter();
        m_gameManager = FindObjectOfType<GameManager>();
	}

    // On entering the room, do nothing since there is nothing special in this room.
    protected override void OnRoomEnter()
    {
        // TODO: Spawn monsters
        if (!m_wonCombat)
        {
            int x = 0, y = 2, z = 1;
            foreach (GameObject go in m_roomEnemiesOverworld)
            {
                m_enemyPawns.Add(Instantiate(go, new Vector3(x, y, z), Quaternion.identity));
                z-=2;
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
        foreach(Object o in m_enemyPawns)
        {
            m_enemyPawns.Remove(o);
            Destroy(o);
        }
        return;
    }

    // Change the value of the room's "won" status
    public void SetWonStatus(bool won)
    {
        m_wonCombat = won;
    }
}
