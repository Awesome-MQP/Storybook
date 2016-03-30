using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameInfoButton : MonoBehaviour {
    private string m_lobbyname = "";

    public string LobbyName
    {
        get { return m_lobbyname; }
        set { m_lobbyname = value; }
    }

    void Start()
    {
        //_setLobbyName();
    }

    void InitializeGameInfoButton(string gameInfo)
    {

    }

    private void _setLobbyName()
    {
        // Main thing we want to do is store the lobby name.
        Text lobbyInfoText = GetComponentInChildren<Text>();
        string lobbyInfo = lobbyInfoText.text;
        int lobbyNameStopCharIndex = lobbyInfo.IndexOf("|");
        m_lobbyname = lobbyInfo.Substring(0, lobbyNameStopCharIndex - 1);
    }

    public void _joinLobbyByName()
    {
        GameObject.FindObjectOfType<JoinGameMenuUIHandler>().OnLobbyButtonClick(LobbyName);
    }

}
