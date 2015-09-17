using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    [SerializeField]
    private GameObject m_combatUIPrefab;

    private GameObject combatInstance;
    private int m_frameNumber = 1;
    private Camera m_mainCamera;
	
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
        else if (m_frameNumber == 300)
        {
            EndCombat();
        }
        m_frameNumber++;
	}

    public void StartCombat()
    {
        Vector3 currentCameraLocation = m_mainCamera.transform.position;
        //Quaternion currentCameraRotation = m_mainCamera.transform.rotation;
        //Vector3 newCameraPosition = new Vector3(currentCameraLocation.x - 100, currentCameraLocation.y, currentCameraLocation.z);
        combatInstance = (GameObject) Instantiate(m_combatUIPrefab);
        Camera combatCamera = combatInstance.GetComponentInChildren<Camera>();
        combatCamera.enabled = true;
        m_mainCamera.enabled = false;
    }

    public void EndCombat()
    {
        m_mainCamera.enabled = true;
        combatInstance.GetComponentInChildren<Camera>().enabled = false;
        Destroy(combatInstance);
    }
}
