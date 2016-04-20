using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public abstract class GameMenuUIHandler : Photon.PunBehaviour {

    private bool m_isTutorial = false;

    private bool m_isLoadingJoinScreen = false;

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
    }

    public void ReturnToLobby()
    {
        m_isLoadingJoinScreen = true;
        PhotonNetwork.LeaveRoom();
    }

    public bool IsTutorial
    {
        get { return m_isTutorial; }
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        SceneFading.DestroyInstance();
        if (!m_isLoadingJoinScreen)
        {
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            SceneManager.LoadScene("JoinGameMenu");
        }
    }
}