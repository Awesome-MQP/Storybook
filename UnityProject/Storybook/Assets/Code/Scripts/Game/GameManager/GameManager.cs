﻿using UnityEngine;
using System.Collections.Generic;

public class GameManager : Photon.PunBehaviour {

    [SerializeField]
    private CombatManager m_combatInstancePrefab;

    [SerializeField]
    private Vector3 m_defaultLocation;

    [SerializeField]
    private CombatPawn[] m_enemiesForCombat;

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
            List<PlayerEntity> playersEnteringCombat = new List<PlayerEntity>(FindObjectsOfType<PlayerEntity>());
            Vector3 combatPosition = new Vector3(m_defaultLocation.x + 1000 * m_combatInstances.Count, m_defaultLocation.y + 1000 * m_combatInstances.Count,
                m_defaultLocation.z + 1000 * m_combatInstances.Count);
            GameObject m_combatInstance = PhotonNetwork.Instantiate("CombatInstance", combatPosition, Quaternion.identity, 0);
            PhotonNetwork.Spawn(m_combatInstance.GetComponent<PhotonView>());
            CombatManager combatManager = m_combatInstance.GetComponent<CombatManager>();
            combatManager.SetPlayerEntityList(playersEnteringCombat);
            combatManager.SetEnemiesToSpawn(m_enemiesForCombat);
            combatManager.SetPlayersToSpawn(PhotonNetwork.playerList.Length);

            // Get all the player position nodes and set it in the combat manager
            PlayerPositionNode[] playerPositions = m_combatInstance.GetComponentsInChildren<PlayerPositionNode>() as PlayerPositionNode[];
            List<PlayerPositionNode> playerPositionsList = new List<PlayerPositionNode>(playerPositions);
            combatManager.SetPlayerPositions(playerPositionsList);

            // Get all the enemy position nodes and set it in the combat manager
            EnemyPositionNode[] enemyPositions = m_combatInstance.GetComponentsInChildren<EnemyPositionNode>() as EnemyPositionNode[];
            List<EnemyPositionNode> enemyPositionsList = new List<EnemyPositionNode>(enemyPositions);
            combatManager.SetEnemyPositions(enemyPositionsList);

            m_combatInstances.Add(m_combatInstance);
            //combatManager.StartCombat();
        }
    }

    /// <summary>
    /// Ends the combat instance that has the given CombatManager
    /// </summary>
    /// <param name="cm">The CombatManager whose combat instance will be destroyed</param>
    public void EndCombat()
    {
        CombatManager cm = m_combatInstances[0].GetComponent<CombatManager>();

        cm.DestroyAllPawns();

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
