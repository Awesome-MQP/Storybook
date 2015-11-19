using UnityEngine;
using System.Collections.Generic;

public class GameManager : Photon.PunBehaviour {

    [SerializeField]
    private CombatManager m_combatInstancePrefab;

    [SerializeField]
    private Vector3 m_defaultLocation;

    [SerializeField]
    private CombatPawn[] m_enemiesForCombat;

    [SerializeField]
    private EnemyTeam m_enemyTeamForCombat;

    [SerializeField]
    private PlayerTeam m_playerTeamForCombat;

    [SerializeField]
    private CombatPlayer m_playerPawn;

    [SerializeField]
    private int m_playersInCombat;

    private List<GameObject> m_combatInstances = new List<GameObject>();
    private float m_timeElapsed = 0;

    private GameObject m_combatInstance;

	// Update is called once per frame
	void Start () {
        DontDestroyOnLoad(this);
        //Camera.main.GetComponent<AudioListener>().enabled = false;

        // Only call StartCombat on the master client
        if (PhotonNetwork.isMasterClient)
        {
            StartCombat();
        }
    }

    /// <summary>
    /// Starts a combat instance and sets the players for the combat manager to the list of players given to this function
    /// </summary>
    /// <param name="playersEnteringCombat"></param>
    public void StartCombat()
    {
        //TODO: Figure out how this will work between clients
        //TODO: Pass in enemies to start combat with

        if (PhotonNetwork.isMasterClient)
        {
            GameObject playerTeam = PhotonNetwork.Instantiate(m_playerTeamForCombat.name, Vector3.zero, Quaternion.identity, 0);
            PhotonNetwork.Spawn(playerTeam.GetComponent<PhotonView>());
            GameObject enemyTeam = PhotonNetwork.Instantiate(m_enemyTeamForCombat.name, Vector3.zero, Quaternion.identity, 0);
            PhotonNetwork.Spawn(enemyTeam.GetComponent<PhotonView>());

            for(int i = 0; i < PhotonNetwork.playerList.Length; i++)
            {
                playerTeam.GetComponent<CombatTeam>().AddPawnToSpawn(m_playerPawn);
            }

            playerTeam.GetComponent<CombatTeam>().TeamId = 1;
            enemyTeam.GetComponent<CombatTeam>().TeamId = 2;

            List<CombatTeam> combatTeams = new List<CombatTeam>();
            combatTeams.Add(playerTeam.GetComponent<CombatTeam>());
            combatTeams.Add(enemyTeam.GetComponent<CombatTeam>());

            List<PlayerEntity> playersEnteringCombat = new List<PlayerEntity>(FindObjectsOfType<PlayerEntity>());
            Vector3 combatPosition = new Vector3(m_defaultLocation.x + 1000 * m_combatInstances.Count, m_defaultLocation.y + 1000 * m_combatInstances.Count,
                m_defaultLocation.z + 1000 * m_combatInstances.Count);
            GameObject m_combatInstance = PhotonNetwork.Instantiate("CombatInstance", combatPosition, Quaternion.identity, 0);
            PhotonNetwork.Spawn(m_combatInstance.GetComponent<PhotonView>());
            CombatManager combatManager = m_combatInstance.GetComponent<CombatManager>();
            combatManager.SetCombatTeamList(combatTeams);

            m_combatInstances.Add(m_combatInstance);
        }
    }

    /// <summary>
    /// Ends the combat instance that has the given CombatManager
    /// </summary>
    /// <param name="cm">The CombatManager whose combat instance will be destroyed</param>
    public void EndCombat()
    {
        CombatManager cm = m_combatInstances[0].GetComponent<CombatManager>();

        cm.DestroyAllTeams();

        GameObject currentCombatInstance = m_combatInstances[0];
        m_combatInstances.Remove(currentCombatInstance);
        Destroy(currentCombatInstance);

        //_returnToDungeon();
    }

    /// <summary>
    /// Gets all of the PlayerEntity in the game
    /// </summary>
    /// <returns>A list of all the PlayerEntity currently in the game</returns>
    public List<PlayerEntity> GetAllPlayers()
    {
        PlayerEntity[] allPlayers = FindObjectsOfType(typeof(PlayerEntity)) as PlayerEntity[];
        List<PlayerEntity> allPlayersList = new List<PlayerEntity>(allPlayers);
        Debug.Log("Adding " + allPlayers.Length.ToString() + " players");
        return allPlayersList;
    }

    private void _returnToDungeon()
    {
        DungeonMovement dm = FindObjectOfType<DungeonMovement>();
        dm.enabled = true;
        dm.TransitionToDungeon();
    }

}
