using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {

    [SerializeField]
    private Camera m_combatCamera;

    [SerializeField]
    private Camera m_overworldCamera;

    // Update is called once per frame
    // TEST METHOD: Based on keyboard input, swap the camera
    void Update () {
        if(Input.GetKeyDown(KeyCode.Alpha8))
        {
            SwitchToCombatCamera();
        }
        if(Input.GetKeyDown(KeyCode.Alpha9))
        {
            SwitchToOverworldCamera();
        }
    }


    // Move the combatCamera to the top of the camera stack, effectively rendering it "above" the other cameras
    void SwitchToCombatCamera()
    {
        Debug.Log("Switching to CombatCamera");
        m_combatCamera.depth = 1;
        m_overworldCamera.depth = 0;
    }

    // Move the overworldCamera to the top of the camera stack, effectively rendering it "above" the other cameras
    void SwitchToOverworldCamera()
    {
        Debug.Log("Switching to OverworldCamera");
        m_combatCamera.depth = 0;
        m_overworldCamera.depth = 1;
    }
}
