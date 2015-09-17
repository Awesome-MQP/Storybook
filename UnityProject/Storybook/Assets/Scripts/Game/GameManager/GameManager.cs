using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    [SerializeField]
    private GameObject m_combatUIPrefab;

    private GameObject combatInstance;
    private int m_frameNumber = 1;
	
	// Update is called once per frame
	void Update () {
        if (m_frameNumber == 100)
        {
            StartCombat();
        }
        else if (m_frameNumber == 300)
        {
            EndCombat();
        }
        m_frameNumber++;
	}

    public void StartCombat()
    {
        combatInstance = Instantiate(m_combatUIPrefab);
    }

    public void EndCombat()
    {
        Destroy(combatInstance);
    }
}
