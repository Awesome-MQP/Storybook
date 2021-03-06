﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class JoinGameMenuUIHandler : UnityEngine.MonoBehaviour {

    [SerializeField]
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

    private ArrayList games = new ArrayList();

    private Text m_availGamesText;

    public void Update()
    {
        // Now load all 
        foreach (RoomInfo game in PhotonNetwork.GetRoomList())
        {
            // If the game list already contains this lobby, merely update the number of players
            if (games.Contains(game.name))
            {
                GameInfoButton existingGame = m_availGamesRect.content.Find(game.name).GetComponent<GameInfoButton>();
                existingGame.GetComponentInChildren<Text>().text = game.name + " | Players in lobby: " + game.playerCount + " / " + game.maxPlayers;
            }
            else // make a new lobby button
            {
                games.Add(game.name);
                String gameInfo = game.name + " | Players in lobby: " + game.playerCount + " / " + game.maxPlayers;
                Debug.Log(gameInfo);
                GameInfoButton lobbyButton = _instantiateGameInfoButton(gameInfo, game.name);
                lobbyButton.gameObject.name = game.name;
                lobbyButton.transform.SetParent(m_availGamesRect.content.transform, false);
            }
        }
    }

    private GameInfoButton _instantiateGameInfoButton(String gameInfo, String gameName)
    {
        GameInfoButton button = Instantiate(m_gameInfoButton) as GameInfoButton;

        button.LobbyName = gameName;
        button.GetComponentInChildren<Text>().text = gameInfo;

        return button;
    }

    public void OnLobbyButtonClick(String lobbyname)
    {
        _joinLobby(lobbyname);
    }

    public void OnSearchButtonClick()
    {
        String searchText = "";
        InputField[] search = GetComponentsInChildren<InputField>();
        foreach(InputField i in search)
        {
            switch(i.name)
            {
                case "GameNameSearch":
                    searchText = i.text;
                    break;
            }
        }
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

    public void OnJoinedRoom()
    {
        if(PhotonNetwork.isMasterClient)
        {
            InputField[] search = GetComponentsInChildren<InputField>();
            foreach (InputField i in search)
            {
                switch (i.name)
                {
                    case "GameNameSearch":
                        PhotonNetwork.room.name = i.text;
                        break;
                }
            }
            /*
            InputField search = GetComponentInChildren<InputField>();
            String searchText = search.text;
            PhotonNetwork.room.name = searchText;
            */
        }
        InputField[] inputs = GetComponentsInChildren<InputField>();
        foreach (InputField i in inputs)
        {
            switch (i.name)
            {
                case "EnterNickname":
                    PhotonNetwork.player.name = i.text;
                    break;
            }
        }
        SceneFading.Instance().LoadScene("PreGameLobby");
    }

    public void ReturnToMainMenu()
    {
        //SceneFading.Instance().LoadScene("MainMenu");
        SceneManager.LoadScene("MainMenu");
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
