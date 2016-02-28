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

    public void ReturnToMainMenu()
    {
        PhotonNetwork.LeaveRoom();
        SceneFading fader = SceneFading.Instance();
        fader.LoadScene("GameStartup");
    }

    public void ReturnToLobby()
    {
        string roomName = PhotonNetwork.room.name;
        PhotonNetwork.LeaveRoom();
        SceneFading fader = SceneFading.Instance();
        fader.LoadScene("JoinGameMenu");
    }

}
