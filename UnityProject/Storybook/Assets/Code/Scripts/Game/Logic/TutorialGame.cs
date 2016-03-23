using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MapManager))]
[RequireComponent(typeof(DungeonMaster))]
public class TutorialGame : BaseStorybookGame {

    [SerializeField]
    TutorialUIHandler m_tutorialUIPrefab;

    public override void OnStartOwner(bool wasSpawn)
    {
        //Startup the map manager
        m_mapManager = GetComponent<MapManager>();
        m_mapManager.GenerateTutorialMap();
        RoomObject startRoom = m_mapManager.PlaceStartRoom();

        //Spawn the player mover on the map
        GameObject moverObject = PhotonNetwork.Instantiate("Rooms/RoomFeatures/" + m_playerMoverPrefab.name, Vector3.zero, Quaternion.identity, 0);
        BasePlayerMover mover = moverObject.GetComponent<BasePlayerMover>();
        m_mover = mover;
        m_mover.Construct(startRoom);
        PhotonNetwork.Spawn(mover.photonView);

        InitializeCamera(startRoom.CameraNode.position, startRoom.CameraNode.rotation);
        photonView.RPC(nameof(InitializeCamera), PhotonTargets.Others, startRoom.CameraNode.position, startRoom.CameraNode.rotation);

        m_hasStarted = true;

        photonView.RPC(nameof(CreateTutorialMenu), PhotonTargets.All);

        base.OnStartOwner(wasSpawn);
    }

    [PunRPC]
    public void InitializeCamera(Vector3 position, Quaternion rotation)
    {
        Camera.main.transform.position = position;
        Camera.main.transform.rotation = rotation;
    }

    [PunRPC]
    public void CreateTutorialMenu()
    {
        GameObject tutorialUI = (GameObject)Instantiate(m_tutorialUIPrefab.gameObject, Vector3.zero, Quaternion.identity);
        TutorialUIHandler tutorialUIHandler = tutorialUI.GetComponent<TutorialUIHandler>();

        List<string> tutorialStrings = new List<string>();
        tutorialStrings.Add("Welcome to Storybook!");
        tutorialStrings.Add("Choose a direction");

        tutorialUIHandler.PopulateMenu("Welcome to Storybook!", tutorialStrings);
    }
}
