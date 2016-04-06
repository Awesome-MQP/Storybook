using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {

    [SerializeField]
    private Camera m_combatCamera;

    [SerializeField]
    private Camera m_overworldCamera;

    // Move the combatCamera to the top of the camera stack, effectively rendering it "above" the other cameras
    public void SwitchToCombatCamera()
    {
        Debug.Log("Switching to CombatCamera");
        m_combatCamera.depth = 1;
        m_overworldCamera.depth = 0;
    }

    // Move the overworldCamera to the top of the camera stack, effectively rendering it "above" the other cameras
    public void SwitchToOverworldCamera()
    {
        Debug.Log("Switching to OverworldCamera");
        m_combatCamera.depth = 0;
        m_overworldCamera.depth = 1;
    }
}
