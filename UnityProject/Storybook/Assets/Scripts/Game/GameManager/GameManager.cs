using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    [SerializeField]
    private GameObject m_combatInstancePrefab;

    [SerializeField]
    private Vector3 m_defaultLocation;

    private List<GameObject> m_combatInstances = new List<GameObject>();
    private float m_timeElapsed = 0;
    private bool test1Done = false;
    private bool test2Done = false;
    private bool test3Done = false;
    private bool test4Done = false;

	// Update is called once per frame
	void Update () {
        _testGameManager();
	}

    // Starts a combat instance and sets the players for the combat manager to the list of players given to this function
    public void StartCombat(List<PlayerEntity> playersEnteringCombat)
    {
        Vector3 combatPosition = new Vector3(m_defaultLocation.x + 1000 * m_combatInstances.Count, m_defaultLocation.y + 1000 * m_combatInstances.Count,
            m_defaultLocation.z + 1000 * m_combatInstances.Count);
        GameObject combatInstance = (GameObject) Instantiate(m_combatInstancePrefab, combatPosition, Quaternion.identity);
        CombatManager combatManager = combatInstance.GetComponent<CombatManager>();
        combatManager.PlayerList = playersEnteringCombat;
        m_combatInstances.Add(combatInstance);
    }

    // Ends the combat instance that has the given CombatManager
    public void EndCombat(CombatManager cm)
    {
        for (int i = 0; i < m_combatInstances.Count; i++)
        {
            GameObject currentCombatInstance = m_combatInstances[i];
            CombatManager currentCombatManager = currentCombatInstance.GetComponent<CombatManager>();
            if (currentCombatManager == cm)
            {
                Destroy(currentCombatInstance);
                break;
            }
        }
    }

    // Ends all the currently running combat instances
    public void EndAllCombat()
    {
        for (int i = 0; i < m_combatInstances.Count; i++)
        {
            GameObject currentCombatInstance = m_combatInstances[i];
            Destroy(currentCombatInstance);
        }
    }

    // Returns the combat instance that the given player is in
    // Returns null if the given player is not in a combat instance
    public GameObject GetCombatForPlayer(PlayerEntity player)
    {
        //TODO: Make a lookup table from player entity to combat manager, this could be done with a variable on PlayerEntity or list in GameManager
        for (int i = 0; i < m_combatInstances.Count; i++)
        {
            GameObject currentCombatInstance = m_combatInstances[i];
            CombatManager currentCombatManager = currentCombatInstance.GetComponent<CombatManager>();
            List<PlayerEntity> playerEntityList = currentCombatManager.PlayerList;
            foreach (PlayerEntity pe in playerEntityList)
            {
                if (pe == player)
                {
                    Debug.Log("Combat with test player index = " + i.ToString());
                    return currentCombatInstance;
                }
            }
        }
        return null;
    }

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
}
