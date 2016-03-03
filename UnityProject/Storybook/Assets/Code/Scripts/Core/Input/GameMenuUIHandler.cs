using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public abstract class GameMenuUIHandler : UIHandler {

    public void StartNewGame()
    {
        SceneFading fader = SceneFading.Instance();
        fader.LoadScene("CharacterSelect");
    }

    public void JoinGame()
    {
        SceneManager.LoadScene("JoinGameMenu");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ReturnToMainMenu()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("GameStartup");
    }

    public void ReturnToLobby()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("JoinGameMenu");
    }

}
