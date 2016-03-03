using UnityEngine;
using UnityEngine.UI;
using Photon;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class JoinGameMenuUIHandler : Photon.PunBehaviour {

    private Text m_availGamesText;

    public void Start()
    {
        _setAvailableGamesText();
    }

    public void Update()
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
        
        InputField search = GetComponentInChildren<InputField>();
        String searchText = search.text;

        // Only search for a game if 
        if(!String.IsNullOrEmpty(searchText))
        {
            bool foundRoom = false;

            foreach (RoomInfo room in PhotonNetwork.GetRoomList())
            {
                if (room.name == searchText)
                {
                    PhotonNetwork.JoinRoom(searchText);
                    foundRoom = true;
                    break;
                }
            }
            // if the room exists, join it
            if (foundRoom == true)
            {
                PhotonNetwork.JoinRoom(searchText);
            }
            // otherwise make a new room
            if (foundRoom == false)
            {
                RoomOptions options = new RoomOptions();
                options.maxPlayers = 4;
                PhotonNetwork.CreateRoom(searchText, options, null);
            }
        }
    }

    public override void OnJoinedRoom()
    {
        if(PhotonNetwork.isMasterClient)
        {
            InputField search = GetComponentInChildren<InputField>();
            String searchText = search.text;
            PhotonNetwork.room.name = searchText;
        }

        SceneManager.LoadScene("PreGameLobby");
    }

    private void _setAvailableGamesText()
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
