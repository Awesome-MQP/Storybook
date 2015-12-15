﻿using UnityEngine;
using System.Collections.Generic;

public class GameManager : Photon.PunBehaviour {

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

	void Start () {
        DontDestroyOnLoad(this);

        // Only call StartCombat on the master client
        if (PhotonNetwork.isMasterClient)
        {
            StartGame();
        }
    }

    /// <summary>
    /// Starts a combat instance and sets the players for the combat manager to the list of players given to this function
    /// </summary>
    public void StartCombat()
    {
        if (PhotonNetwork.isMasterClient)
        {
            GameObject dungeonMaster = PhotonNetwork.Instantiate(m_dungeonMaster.name, Vector3.zero, Quaternion.identity, 0);
            PhotonNetwork.Spawn(dungeonMaster.GetComponent<PhotonView>());

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
            Vector3 combatPosition = new Vector3(m_defaultLocation.x + 1000, m_defaultLocation.y + 1000, m_defaultLocation.z + 1000);
            m_combatInstance = PhotonNetwork.Instantiate("CombatInstance", combatPosition, Quaternion.identity, 0);
            PhotonNetwork.Spawn(m_combatInstance.GetComponent<PhotonView>());
            CombatManager combatManager = m_combatInstance.GetComponent<CombatManager>();
            combatManager.SetCombatTeamList(combatTeams);
            
            CameraManager m_camManager = FindObjectOfType<CameraManager>();
            m_camManager.SwitchToCombatCamera(); // Switch to combat camera.
            
        }
    }

    public void StartGame()
    {
        MapManager mapManager = FindObjectOfType<MapManager>();
        mapManager.GenerateMap();
        RoomObject startRoom = mapManager.PlaceStartRoom();
        PlayerMover playerMover = FindObjectOfType<PlayerMover>();
        List<NetworkNodeMover> players = new List<NetworkNodeMover>(FindObjectsOfType<NetworkNodeMover>());
        playerMover.WorldPlayers = players;
        photonView.RPC("SendCameraPosRot", PhotonTargets.All, startRoom.CameraNode.transform.position, startRoom.CameraNode.transform.rotation);
        playerMover.SpawnInRoom(startRoom);
    }

    /// <summary>
    /// Ends the combat instance that has the given CombatManager
    /// </summary>
    public void EndCombat()
    {
        CombatManager cm = m_combatInstance.GetComponent<CombatManager>();

        cm.DestroyAllTeams();
        cm.DestroyAllPages();

        GameObject currentCombatInstance = m_combatInstance;

        PhotonNetwork.Destroy(currentCombatInstance);
        Destroy(currentCombatInstance);

        CameraManager m_camManager = FindObjectOfType<CameraManager>();
        m_camManager.SwitchToOverworldCamera(); // Switch to the overworld camera.
        
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

    /*
    private void _returnToDungeon()
    {
        DungeonMovement dm = FindObjectOfType<DungeonMovement>();
        dm.enabled = true;
        dm.TransitionToDungeon();
    }
    */

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

    [PunRPC]
    public void SendCameraPosRot(Vector3 position, Quaternion rotation)
    {
        Camera.main.transform.position = position;
        Camera.main.transform.rotation = rotation;
    }
}
