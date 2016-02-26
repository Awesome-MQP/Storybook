using UnityEngine;
using UnityEngine.UI;
using Photon;
using System.Collections;

public class JoinGameMenuUIHandler : Photon.PunBehaviour {

    private Text m_availGamesText;

    public void Start()
    {
        _setAvailableGamesText();
    }

    public void OnGUI()
    {
        // Populate the available games list
        m_availGamesText.text = "";
        foreach(RoomInfo game in PhotonNetwork.GetRoomList())
        {
            m_availGamesText.text += game.name + " " + game.playerCount + "/" + game.maxPlayers + "\n";
        }
        if(PhotonNetwork.GetRoomList().Length == 0)
        {
            m_availGamesText.text = "No games found!";
        }
    }

    public void OnSearchButtonClick()
    {

    }

    public void _setAvailableGamesText()
    {
        Text[] text = GetComponentsInChildren<Text>();
        foreach(Text t in text)
        {
            if (t.name == "CurrentGamesText")
            {
                m_availGamesText = t;
                return;
            }
        }
    }
}
