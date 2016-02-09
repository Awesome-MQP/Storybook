using UnityEngine;
using System.Collections;
using System;

public class GameOverUIHandler : GameMenuUIHandler
{
    
    void Start()
    {
        PlayerObject[] allPlayerObjects = FindObjectsOfType<PlayerObject>();
        foreach(PlayerObject po in allPlayerObjects)
        {
            Destroy(po.photonView);
            Destroy(po.gameObject);
        }
    }
}
