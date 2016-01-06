using UnityEngine;
using System.Collections.Generic;

public class GameManager : Photon.PunBehaviour
{

    [SerializeField]
    private CombatManager m_combatInstancePrefab;

    [SerializeField]
    private Vector3 m_defaultLocation;

    [SerializeField]
    private EnemyTeam m_enemyTeamForCombat;

    [SerializeField]
    private PlayerTeam m_playerTeamForCombat;

    [SerializeField]
    private CombatPlayer m_playerPawn;

    [SerializeField]
    private DungeonMaster m_dungeonMaster;

    private GameObject m_combatInstance;
    private MusicManager m_musicMgr;

	void Start ()
    {
        DontDestroyOnLoad(this);

        // Only call StartCombat on the master client
        if (IsMine)
        {
            StartGame();
        }

        m_musicMgr = FindObjectOfType<MusicManager>();
    }

    /// <summary>
    /// Starts a combat instance and sets the players for the combat manager to the list of players given to this function
    /// </summary>
    public void StartCombat()
    {
        if (PhotonNetwork.isMasterClient)
        {
            /*
            GameObject dungeonMaster = PhotonNetwork.Instantiate(m_dungeonMaster.name, Vector3.zero, Quaternion.identity, 0);
            PhotonNetwork.Spawn(dungeonMaster.GetComponent<PhotonView>());
            */

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

            List<CombatTeam> combatTeams = new List<CombatTeam>
            {
                playerTeam.GetComponent<CombatTeam>(),
                enemyTeam.GetComponent<CombatTeam>()
            };

            List<PlayerEntity> playersEnteringCombat = new List<PlayerEntity>(FindObjectsOfType<PlayerEntity>());
            Vector3 combatPosition = new Vector3(m_defaultLocation.x + 1000, m_defaultLocation.y + 1000, m_defaultLocation.z + 1000);
            m_combatInstance = PhotonNetwork.Instantiate("CombatInstance", combatPosition, Quaternion.identity, 0);
            PhotonNetwork.Spawn(m_combatInstance.GetComponent<PhotonView>());
            CombatManager combatManager = m_combatInstance.GetComponent<CombatManager>();
            combatManager.SetCombatTeamList(combatTeams);

            //CameraManager m_camManager = FindObjectOfType<CameraManager>();
            //m_camManager.SwitchToCombatCamera(); // Switch to combat camera.

            m_musicMgr.switchToFightMusic();
        }
    }

    public void StartGame()
    {
        MapManager mapManager = FindObjectOfType<MapManager>();
        mapManager.GenerateMap();
        RoomObject startRoom = mapManager.PlaceStartRoom();
        RoomMover playerMover = FindObjectOfType<RoomMover>();
        List<NetworkNodeMover> players = new List<NetworkNodeMover>(FindObjectsOfType<NetworkNodeMover>());
        playerMover.SpawnInRoom(startRoom);
    }

    public void TransitionToCombat()
    {
        Debug.Log("Transitioning to combat");
        photonView.RPC("EnableMovementComponents", PhotonTargets.All, false);
        StartCombat();
    }

    private void _TransitionToOverworld()
    {
        Debug.Log("Transitioning to overworld");
        photonView.RPC("EnableMovementComponents", PhotonTargets.All, true);
    }

    /// <summary>
    /// Ends the combat instance that has the given CombatManager
    /// </summary>
    public void EndCombat()
    {
        CombatManager cm = m_combatInstance.GetComponent<CombatManager>();

        Debug.Log("Destroying all teams");

        cm.DestroyAllTeams();

        Debug.Log("Destroying combat instance");

        GameObject currentCombatInstance = m_combatInstance;

        PhotonNetwork.Destroy(currentCombatInstance);
        Destroy(currentCombatInstance);

        m_musicMgr.switchToRoomMusic();

        _TransitionToOverworld();

        /*
        CameraManager m_camManager = FindObjectOfType<CameraManager>();
        m_camManager.SwitchToOverworldCamera(); // Switch to the overworld camera.
        */
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

    public EnemyTeam EnemyTeamForCombat
    {
        get { return m_enemyTeamForCombat; }
        set { m_enemyTeamForCombat = value; }
    }

    public PlayerTeam PlayerTeamForCombat
    {
        get { return m_playerTeamForCombat; }
        set { m_playerTeamForCombat = value; }
    }
}
