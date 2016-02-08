using UnityEngine;
using System.Collections;

public abstract class GameMenuUIHandler : UIHandler {

    public void StartNewGame()
    {
        SceneFading fader = SceneFading.Instance();
        fader.LoadScene("WorldMovementTest");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

}
