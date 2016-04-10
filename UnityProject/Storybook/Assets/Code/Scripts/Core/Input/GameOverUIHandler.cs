using UnityEngine;
using System.Collections;
using System;

public class GameOverUIHandler : GameMenuUIHandler
{
    
    void Start()
    {
        Camera gameCamera = Camera.main;
        gameCamera.transform.SetParent(null);
        GameManager.GetInstance<GameManager>().CleanupForNewGame();
    }
}
