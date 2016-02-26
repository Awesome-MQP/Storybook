using UnityEngine;
using System.Collections;

public abstract class GameMenuUIHandler : UIHandler {

    public void StartNewGame()
    {
        SceneFading fader = SceneFading.Instance();
        fader.LoadScene("CharacterSelect");
    }

    public void JoinGame()
    {
        SceneFading fader = SceneFading.Instance();
        fader.LoadScene("JoinGameMenu");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

}
