using UnityEngine;
using UnityEngine.UI;
using Photon;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class JoinGameMenuUIHandler : Photon.PunBehaviour {

    private ScrollRect m_availGamesRect;
    [SerializeField]
    private float m_gridXPadding;
    [SerializeField]
    private float m_gridYPadding;
    [SerializeField]
    private float m_buttonWidth;
    [SerializeField]
    private float m_buttonHeight;
    [SerializeField]
    private GameInfoButton m_gameInfoButton;

    private Text m_availGamesText;

    public void Start()
    {
        //_setAvailableGamesText();
        _setAvailableGamesScrollRect();
    }

    public void Update()
    {
        // Populate the available games list
        //m_availGamesText.text = "";
        RectTransform gamesContent = m_availGamesRect.content;
        foreach(RoomInfo game in PhotonNetwork.GetRoomList())
        {
            String gameInfo = game.name + " | Players in lobby: " + game.playerCount + " / " + game.maxPlayers;
            GameInfoButton lobbyButton = Instantiate(m_gameInfoButton);
            lobbyButton.GetComponent<Text>().text = gameInfo;
            lobbyButton.transform.SetParent(gamesContent, false);
            //m_availGamesText.text += game.name + " " + game.playerCount + "/" + game.maxPlayers + "\n";
        }
        /*
        if(PhotonNetwork.GetRoomList().Length == 0)
        {
            m_availGamesText.text = "No games found!";
        }
        */
    }

    public void OnLobbyButtonClick(String lobbyname)
    {
        _joinLobby(lobbyname);
    }

    public void OnSearchButtonClick()
    {
        InputField search = GetComponentInChildren<InputField>();
        String searchText = search.text;
        _joinLobby(searchText);
        
        // Only search for a game if 
        /*
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
        */
    }

    private void _joinLobby(String lobbyName)
    {
        // Only search for a game if 
        if (!String.IsNullOrEmpty(lobbyName))
        {
            bool foundRoom = false;

            foreach (RoomInfo room in PhotonNetwork.GetRoomList())
            {
                if (room.name == lobbyName)
                {
                    PhotonNetwork.JoinRoom(lobbyName);
                    foundRoom = true;
                    break;
                }
            }
            // if the room exists, join it
            if (foundRoom == true)
            {
                PhotonNetwork.JoinRoom(lobbyName);
            }
            // otherwise make a new room
            if (foundRoom == false)
            {
                RoomOptions options = new RoomOptions();
                options.maxPlayers = 4;
                PhotonNetwork.CreateRoom(lobbyName, options, null);
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

    private void _setAvailableGamesScrollRect()
    {
        ScrollRect scrollrect = GetComponentInChildren<ScrollRect>();
        GridLayoutGroup gridGroup = scrollrect.GetComponentInChildren<GridLayoutGroup>();
        if (gridGroup != null)
        {
            gridGroup.cellSize = new Vector2(m_buttonWidth, m_buttonHeight);
            gridGroup.spacing = new Vector2(m_gridXPadding, m_gridYPadding);
        }

        m_availGamesRect = scrollrect;
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
