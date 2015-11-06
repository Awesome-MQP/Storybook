using UnityEngine;
using System.Collections.Generic;

public class GameManager : Photon.PunBehaviour {

    [SerializeField]
    private GameObject m_combatInstancePrefab;

    [SerializeField]
    private Vector3 m_defaultLocation;

    [SerializeField]
    private CombatPawn[] m_enemiesForCombat;

    [SerializeField]
    private int m_playersInCombat;

    private List<GameObject> m_combatInstances = new List<GameObject>();
    private float m_timeElapsed = 0;
    private bool test1Done = false;
    private bool test2Done = false;
    private bool test3Done = false;
    private bool test4Done = false;

	// Update is called once per frame
	void Start () {
        List<PlayerEntity> playerList = new List<PlayerEntity>();
        Camera.main.GetComponent<AudioListener>().enabled = false;
        if (PhotonNetwork.isMasterClient)
        {
            StartCombat(playerList);
        }
        //_testNetworkedAnimator();
	}

    /// <summary>
    /// Starts a combat instance and sets the players for the combat manager to the list of players given to this function
    /// </summary>
    /// <param name="playersEnteringCombat"></param>
    public void StartCombat(List<PlayerEntity> playersEnteringCombat)
    {
        Vector3 combatPosition = new Vector3(m_defaultLocation.x + 1000 * m_combatInstances.Count, m_defaultLocation.y + 1000 * m_combatInstances.Count,
            m_defaultLocation.z + 1000 * m_combatInstances.Count);
        GameObject combatInstance = PhotonNetwork.Instantiate("CombatInstance", combatPosition, Quaternion.identity, 0);
        CombatManager combatManager = combatInstance.GetComponent<CombatManager>();
        combatManager.SetPlayerEntityList(playersEnteringCombat);
        combatManager.SetEnemiesToSpawn(m_enemiesForCombat);
        combatManager.SetPlayersToSpawn(PhotonNetwork.playerList.Length);

        // Get all the player position nodes and set it in the combat manager
        PlayerPositionNode[] playerPositions = combatInstance.GetComponentsInChildren<PlayerPositionNode>() as PlayerPositionNode[];
        List<PlayerPositionNode> playerPositionsList = new List<PlayerPositionNode>(playerPositions);
        combatManager.SetPlayerPositions(playerPositionsList);

        // Get all the enemy position nodes and set it in the combat manager
        EnemyPositionNode[] enemyPositions = combatInstance.GetComponentsInChildren<EnemyPositionNode>() as EnemyPositionNode[];
        List<EnemyPositionNode> enemyPositionsList = new List<EnemyPositionNode>(enemyPositions);
        combatManager.SetEnemyPositions(enemyPositionsList);

        // TODO - Remove this

        m_combatInstances.Add(combatInstance);
        combatManager.StartCombat();
    }

    /// <summary>
    /// Ends the combat instance that has the given CombatManager
    /// </summary>
    /// <param name="cm">The CombatManager whose combat instance will be destroyed</param>
    public void EndCombat(CombatManager cm)
    {
        // Iterate through all of the combat instances
        for (int i = 0; i < m_combatInstances.Count; i++)
        {
            GameObject currentCombatInstance = m_combatInstances[i];
            CombatManager currentCombatManager = currentCombatInstance.GetComponent<CombatManager>();

            // If the CombatManager of the current combat matches the given CombatManager, destroy the combat instance
            if (currentCombatManager == cm)
            {
                PhotonNetwork.Destroy(currentCombatInstance);
                break;
            }
        }

        CombatPawn[] allPawns = FindObjectsOfType<CombatPawn>();
        for (int i = 0; i < allPawns.Length; i++)
        {
            PhotonNetwork.Destroy(allPawns[i].GetComponent<PhotonView>());
        }
    }

    /// <summary>
    /// Ends all the currently running combat instances
    /// </summary>
    public void EndAllCombat()
    {
        // Iterate through all the combat instances and destroy each of them
        for (int i = 0; i < m_combatInstances.Count; i++)
        {
            GameObject currentCombatInstance = m_combatInstances[i];
            Destroy(currentCombatInstance);
        }
    }


    /// <summary>
    /// Returns the combat instance that the given player is in
    /// Returns null if the given player is not in a combat instance
    /// </summary>
    /// <param name="player">The player whose combat will be returned</param>
    /// <returns>The combat instance that the given player is a part of</returns>
    public GameObject GetCombatForPlayer(PlayerEntity player)
    {
        //TODO: Make a lookup table from player entity to combat manager, this could be done with a variable on PlayerEntity or list in GameManager
        // Iterate through all of the combat instances
        for (int i = 0; i < m_combatInstances.Count; i++)
        {
            // Get the PlayerEntity list from the current combat instance
            GameObject currentCombatInstance = m_combatInstances[i];
            CombatManager currentCombatManager = currentCombatInstance.GetComponent<CombatManager>();

            // Iterate through all of the PlayerEntity in the combat
            foreach (PlayerEntity pe in currentCombatManager.PlayerEntityList)
            {
                // If the current PlayerEntity matches the given PlayerEntity, return the combat instance
                if (pe == player)
                {
                    Debug.Log("Combat with test player index = " + i.ToString());
                    return currentCombatInstance;
                }
            }
        }

        // If the given player is not in a combat, return null
        return null;
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

    /// <summary>
    /// Function to test the game manager
    /// </summary>
    private void _testGameManager()
    {
        m_timeElapsed += Time.deltaTime;
        if (m_timeElapsed >= 3 && !test1Done)
        {
            List<PlayerEntity> playerList = new List<PlayerEntity>();
            StartCombat(playerList);
            test1Done = true;
        }
        else if (m_timeElapsed >= 6 && !test2Done)
        {
            List<PlayerEntity> playerList = new List<PlayerEntity>();
            StartCombat(playerList);
            test2Done = true;
        }
        else if (m_timeElapsed >= 9 && !test3Done)
        {
            EndCombat(m_combatInstances[0].GetComponent<CombatManager>());
            test3Done = true;
        }
        else if (m_timeElapsed >= 10 && !test4Done)
        {
            EndAllCombat();
            test4Done = true;
        }
    }

    private void _testNetworkedAnimator()
    {
        if (PhotonNetwork.isMasterClient)
        {
            GameObject animatorObject = PhotonNetwork.Instantiate("CombatAnimator", Vector3.zero, Quaternion.identity, 0);
            Animator animator = animatorObject.GetComponent<Animator>();
        }
    }
}
