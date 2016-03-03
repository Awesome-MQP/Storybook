using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class PreGameLobbyUIHandler : Photon.PunBehaviour {

    private Text m_currentPlayersInRoomText;
    private String m_startGameText = "Start the Game!";
    private String m_waitForHostText = "Waiting for Host to Start...";

    public void Start()
    {
        _setPlayersText();
        _setNameOfLobbyText();
        Button startButton = GetComponentInChildren<Button>();
        if(!PhotonNetwork.isMasterClient)
        {
            // if not the host, disable the Start Game button
            startButton.interactable = false;
            startButton.GetComponentInChildren<Text>().text = m_waitForHostText;
        }
        else
        {
            // if the host, enable the ability to start game
            startButton.interactable = true;
            startButton.GetComponentInChildren<Text>().text = m_startGameText;
        }
    }

    private void _setNameOfLobbyText()
    {
        Text[] text = GetComponentsInChildren<Text>();
        foreach (Text t in text)
        {
            if (t.name == "GameName")
            {
                t.text = PhotonNetwork.room.name;
                return;
            }
        }
    }

    public override void OnJoinedRoom()
    {

    }

    private void _setPlayersText()
    {
        Text[] text = GetComponentsInChildren<Text>();
        foreach (Text t in text)
        {
            if (t.name == "PlayersInLobby")
            {
                m_currentPlayersInRoomText = t;
                return;
            }
        }
    }

    public void onStartButtonClicked()
    {
        PhotonNetwork.LoadLevel("CharacterSelect");
    }

    public void OnGUI()
    {
        // Populate the current players in lobby list
        // Do this once per cycle to keep the list up to date
        m_currentPlayersInRoomText.text = "";
        foreach (PhotonPlayer player in PhotonNetwork.playerList)
        {
            m_currentPlayersInRoomText.text += player.ID + " " + player.name + "\n";
        }
    }

    // Update is called once per frame
    void Update () {
	
	}
}
