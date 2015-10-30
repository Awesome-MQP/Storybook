using UnityEngine;
using System.Collections;

public class DungeonMovement : Photon.PunBehaviour {

    private int hostPlayerId;
    private static PhotonView ScenePhotonView;
    private MapManager mapManager;
    private RoomObject currentRoom;
    private Location currentLoc;

    // Use this for initialization
    void Start () {
        ScenePhotonView = this.GetComponent<PhotonView>();
        ScenePhotonView.RPC("SelectHostPlayer", PhotonTargets.MasterClient);
        mapManager = FindObjectOfType<MapManager>();
        currentLoc = new Location(0, 0);
        currentRoom = mapManager.PlaceRoom(currentLoc);
    }

    void OnGUI()
    {
        if (hostPlayerId == PhotonNetwork.player.ID)
        {
            if (currentRoom.RoomDoors[currentRoom.NORTH_DOOR_INDEX].IsDoorEnabled && GUILayout.Button("Move Up"))
            {
                Debug.Log("Moving up");
                Location newRoomLoc = new Location(currentLoc.X + 1, currentLoc.Y);
                ScenePhotonView.RPC("CreateNewRoom", PhotonTargets.All, newRoomLoc.X, newRoomLoc.Y);
            }
            if (currentRoom.RoomDoors[currentRoom.WEST_DOOR_INDEX].IsDoorEnabled && GUILayout.Button("Move Left"))
            {
                Debug.Log("Moving left");
                Location newRoomLoc = new Location(currentLoc.X, currentLoc.Y - 1);
                ScenePhotonView.RPC("CreateNewRoom", PhotonTargets.All, newRoomLoc.X, newRoomLoc.Y);
            }
            if (currentRoom.RoomDoors[currentRoom.EAST_DOOR_INDEX].IsDoorEnabled && GUILayout.Button("Move Right"))
            {
                Debug.Log("Moving right");
                Location newRoomLoc = new Location(currentLoc.X, currentLoc.Y + 1);
                ScenePhotonView.RPC("CreateNewRoom", PhotonTargets.All, newRoomLoc.X, newRoomLoc.Y);
            }
            if (currentRoom.RoomDoors[currentRoom.SOUTH_DOOR_INDEX].IsDoorEnabled && GUILayout.Button("Move Down"))
            {
                Debug.Log("Moving down");
                Location newRoomLoc = new Location(currentLoc.X - 1, currentLoc.Y);
                ScenePhotonView.RPC("CreateNewRoom", PhotonTargets.All, newRoomLoc.X, newRoomLoc.Y);
            }
        }
    }

    [PunRPC]
    private void SelectHostPlayer()
    {
        hostPlayerId = UnityEngine.Random.Range(1, PhotonNetwork.playerList.Length);
        ScenePhotonView.RPC("SendOutHostId", PhotonTargets.Others, hostPlayerId);
        Debug.Log("Host player = " + hostPlayerId);
    }

    [PunRPC]
    private void SendOutHostId(int hostId)
    {
        hostPlayerId = hostId;
    }

    [PunRPC]
    private void CreateNewRoom(int newRoomLocX, int newRoomLocY)
    {
        Location newRoomLoc = new Location(newRoomLocX, newRoomLocY);
        currentRoom = mapManager.PlaceRoom(newRoomLoc);
        currentLoc = newRoomLoc;
        mapManager.MoveCamera(newRoomLoc);
        Transform roomCamNode = currentRoom.CameraNode;
        Camera.main.transform.position = roomCamNode.transform.position;
    }

}
