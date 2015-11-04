using UnityEngine;
using System.Collections;

public class RoomMainMenu : Photon.MonoBehaviour {

	// Use this for initialization
	void Awake () {

	}

    void OnGUI()
    {
        if(PhotonNetwork.room == null)
        {
            Debug.Log("null room");
            return;
        }
        // display the players in the room
        GUILayout.BeginArea(new Rect((Screen.width - 400) / 2, (Screen.height - 300) / 2, 400, 300));

        GUILayout.Label("Welcome to the room!");

        GUILayout.BeginHorizontal();
        GUILayout.Label("Player List:");
        // list players
        for(int i = 0; i < PhotonNetwork.playerList.Length; i++)
        {
            GUILayout.Label(PhotonNetwork.playerList[i].ToString());
        }

        // button to start game
        if(PhotonNetwork.player.ID == 1)
        {
            GUILayout.Button("Start game!");
        }

        GUILayout.EndArea();
        return;
    }
}
