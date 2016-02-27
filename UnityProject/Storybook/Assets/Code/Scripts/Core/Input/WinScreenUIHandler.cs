using UnityEngine;
using System.Collections;

public class WinScreenUIHandler : GameMenuUIHandler {

    void Start()
    {
        PlayerObject[] allPlayerObjects = FindObjectsOfType<PlayerObject>();
        foreach (PlayerObject po in allPlayerObjects)
        {
            Destroy(po.photonView);
            Destroy(po.gameObject);
        }
    }
}
