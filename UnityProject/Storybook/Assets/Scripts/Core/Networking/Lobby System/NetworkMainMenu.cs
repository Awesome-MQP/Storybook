﻿using UnityEngine;
using System.Collections;

public class NetworkMainMenu : Photon.MonoBehaviour {

	// Use this for initialization
	void Awake () {
        //Connect to the main photon server. This is the only IP and port we ever need to set(!)
        if (!PhotonNetwork.connected)
            PhotonNetwork.ConnectUsingSettings("v1.0"); // version of the game/demo. used to separate older clients from newer ones (e.g. if incompatible)

        //Load name from PlayerPrefs
        PhotonNetwork.playerName = PlayerPrefs.GetString("playerName", "Guest" + Random.Range(1, 9999));

        //Set camera clipping for nicer "main menu" background
        Camera.main.farClipPlane = Camera.main.nearClipPlane + 0.1f;
    }

    private string _roomName = "myRoom";
    private Vector2 _scrollPos = Vector2.zero;

    void OnGUI()
    {
            if (!PhotonNetwork.connected)
            {
                ShowConnectingGUI();
                return;   //Wait for a connection
            }


        if (PhotonNetwork.room != null)
        {
            return; //Only when we're not in a Room
        }

            GUILayout.BeginArea(new Rect((Screen.width - 400) / 2, (Screen.height - 300) / 2, 400, 300));

            GUILayout.Label("Main Menu");

            //Player name
            GUILayout.BeginHorizontal();
            GUILayout.Label("Player name:", GUILayout.Width(150));
            PhotonNetwork.playerName = GUILayout.TextField(PhotonNetwork.playerName);
            if (GUI.changed)//Save name
                PlayerPrefs.SetString("playerName", PhotonNetwork.playerName);
            GUILayout.EndHorizontal();

            GUILayout.Space(15);


            //Join room by title
            GUILayout.BeginHorizontal();
            GUILayout.Label("JOIN ROOM:", GUILayout.Width(150));
            _roomName = GUILayout.TextField(_roomName);
            if (GUILayout.Button("GO"))
            {
                PhotonNetwork.JoinRoom(_roomName);
            }
            GUILayout.EndHorizontal();

            //Create a room (fails if exist!)
            GUILayout.BeginHorizontal();
            GUILayout.Label("CREATE ROOM:", GUILayout.Width(150));
            _roomName = GUILayout.TextField(_roomName);
            if (GUILayout.Button("GO"))
            {
                // using null as TypedLobby parameter will also use the default lobby
                PhotonNetwork.CreateRoom(_roomName, new RoomOptions() { maxPlayers = 4 }, TypedLobby.Default);
        }
        GUILayout.EndHorizontal();

            //Join random room
            GUILayout.BeginHorizontal();
            GUILayout.Label("JOIN RANDOM ROOM:", GUILayout.Width(150));
            if (PhotonNetwork.GetRoomList().Length == 0)
            {
                GUILayout.Label("..no games available...");
            }
            else
            {
                if (GUILayout.Button("GO"))
                {
                    PhotonNetwork.JoinRandomRoom();
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(30);
            GUILayout.Label("ROOM LISTING:");
            if (PhotonNetwork.GetRoomList().Length == 0)
            {
                GUILayout.Label("..no games available..");
            }
            else
            {
                //Room listing: simply call GetRoomList: no need to fetch/poll whatever!
                _scrollPos = GUILayout.BeginScrollView(_scrollPos);
                foreach (RoomInfo game in PhotonNetwork.GetRoomList())
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(game.name + " " + game.playerCount + "/" + game.maxPlayers);
                    if (GUILayout.Button("JOIN"))
                    {
                        PhotonNetwork.JoinRoom(game.name);
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndScrollView();
            }

            GUILayout.EndArea();
    }

    void ShowConnectingGUI()
    {
        GUILayout.BeginArea(new Rect((Screen.width - 400) / 2, (Screen.height - 300) / 2, 400, 300));

        GUILayout.Label("Connecting to Photon server.");

        GUILayout.EndArea();
    }
}

