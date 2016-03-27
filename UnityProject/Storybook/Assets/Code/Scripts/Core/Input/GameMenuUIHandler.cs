using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public abstract class GameMenuUIHandler : UIHandler {

    private bool m_isTutorial = false;

    public void StartNewGame()
    {
        PhotonNetwork.CreateRoom(null);
    }

    public void JoinGame()
    {
        SceneManager.LoadScene("JoinGameMenu");
    }

    public void TutorialGame()
    {
        m_isTutorial = true;
        PhotonNetwork.CreateRoom(null);
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

    public bool IsTutorial
    {
        get { return m_isTutorial; }
    }
}
