using UnityEngine;
using System.Collections;

public class WinScreenUIHandler : GameMenuUIHandler {

    void Start()
    {
        Camera gameCamera = Camera.main;
        gameCamera.transform.SetParent(null);
        GameManager.GetInstance<GameManager>().CleanupForNewGame();
    }
}
