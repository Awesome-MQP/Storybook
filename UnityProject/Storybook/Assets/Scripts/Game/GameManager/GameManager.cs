using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    [SerializeField]
    private GameObject m_combatInstancePrefab;

    [SerializeField]
    private Vector3 m_defaultLocation;

    private int m_frameNumber = 1;
    private Camera m_mainCamera;
    private List<GameObject> m_combatInstances = new List<GameObject>();

    void Start()
    {
        m_mainCamera = Camera.main;
    }

	// Update is called once per frame
	void Update () {
        if (m_frameNumber == 100)
        {
            StartCombat();
        }
        if (m_frameNumber == 200)
        {
            StartCombat();
        }
        else if (m_frameNumber == 300)
        {
            EndCombat(m_combatInstances[0].GetComponent<CombatManager>());
        }
        m_frameNumber++;
	}

    public void StartCombat()
    {
        Vector3 combatPosition = new Vector3(m_defaultLocation.x + 1000 * m_combatInstances.Count, m_defaultLocation.y + 1000 * m_combatInstances.Count,
            m_defaultLocation.z + 1000 * m_combatInstances.Count);
        GameObject combatInstance = (GameObject) Instantiate(m_combatInstancePrefab, combatPosition, Quaternion.identity);
        m_combatInstances.Add(combatInstance);
        Camera combatCamera = combatInstance.GetComponentInChildren<Camera>();
    }

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
}
