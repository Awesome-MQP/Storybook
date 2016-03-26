using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(MapManager))]
[RequireComponent(typeof(DungeonMaster))]
public class TutorialGame : BaseStorybookGame, TutorialEventDispatcher.ITutorialEventListener {

    [SerializeField]
    TutorialUIHandler m_tutorialUIPrefab;

    private bool m_pageForRoomIsComplete = false;
    private bool m_deckManagementIsComplete = false;
    private bool m_hasShownStartTutorial = false;

    public EventDispatcher TutorialDispatcher { get { return EventDispatcher.GetDispatcher<TutorialEventDispatcher>(); } }

    protected override void Awake()
    {
        EventDispatcher.GetDispatcher<TutorialEventDispatcher>().RegisterEventListener(this);

        base.Awake();
    }

    void OnDestroy()
    {
        EventDispatcher.GetDispatcher<TutorialEventDispatcher>().RemoveListener(this);
    }

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

        OnTutorialStart();

        base.OnStartOwner(wasSpawn);
    }

    [PunRPC]
    public void InitializeCamera(Vector3 position, Quaternion rotation)
    {
        Camera.main.transform.position = position;
        Camera.main.transform.rotation = rotation;
    }

    /// <summary>
    /// Creates the tutorial UI that appears at the start of the tutorial
    /// </summary>
    [PunRPC]
    public void CreateStartMenu()
    {
        if (!m_hasShownStartTutorial)
        {
            List<string> tutorialStrings = new List<string>();
            tutorialStrings.Add("Welcome to Storybook!");
            tutorialStrings.Add("Welcome to your first dungeon in Storybook. Throughout the game, you will be using mystical pages as attacks and to build the rooms of the dungeons");
            tutorialStrings.Add("To start, choose the door that you would like to move through.");

            _instantiateTutorialUI("Welcome to Storybook!", tutorialStrings);
            m_hasShownStartTutorial = true;
        }
    }

    /// <summary>
    /// Creates the tutorial UI that appears when the player is going to choose a page to create a room
    /// </summary>
    [PunRPC]
    public void CreatePageForRoomTutorial()
    {
        List<string> tutorialStrings = new List<string>();
        tutorialStrings.Add("When entering a new room, you must place down a page to create the room.");
        tutorialStrings.Add("Which page to choose is important as its stats will affect the room. The higher the level, the more powerful the enemies will be, but there will also be higher rewards");
        tutorialStrings.Add("The colors of the pages act as types called genres, and the genre of the page chosen will be the most likely enemy.");

        _instantiateTutorialUI("Creating a Room", tutorialStrings);
    }

    /// <summary>
    /// Creates the tutorial UI that appears at the start of combat
    /// </summary>
    [PunRPC]
    public void CreateCombatTutorial()
    {
        List<string> tutorialStrings = new List<string>();
        tutorialStrings.Add("In combat, you will draw pages from your deck and use them as attacks or boosts. Note, using pages in combat do not cause you to lose them.");
        tutorialStrings.Add("The power of attack pages are more powerful the higher their level, and damage depends on the type of the user and the type of the character being attacked.");
        tutorialStrings.Add("Boosts pages temporarily increase stats. Red horror pages heal players, green fantasy pages speed up players, yellow comic book pages increase attack, and blue sci-fi pages increase defense.");

        _instantiateTutorialUI("Fighting Enemies", tutorialStrings);
    }

    /// <summary>
    /// Creates the tutorial UI that appears when the enemies are defeated
    /// </summary>
    [PunRPC]
    public void CombatClearTutorial()
    {
        List<string> tutorialStrings = new List<string>();
        tutorialStrings.Add("Upon clearing a combat, you can select one new page from the drops to add to your inventory.");
        tutorialStrings.Add("The higher the level of the enemies, the better the rewards will be. Also, the dropped pages are more likely to match the genre of the enemies");

        _instantiateTutorialUI("Reaping Rewards of Combat", tutorialStrings);
    }

    /// <summary>
    /// Creates the tutorial UI that appears the first time the deck management menu is opened
    /// </summary>
    [PunRPC]
    public void DeckManagementTutorial()
    {
        List<string> tutorialStrings = new List<string>();
        tutorialStrings.Add("Deck management text 1");
        tutorialStrings.Add("Deck management text 2");

        _instantiateTutorialUI("Managing your Inventory and Deck", tutorialStrings);
    }

    /// <summary>
    /// Creates the tutorial UI that appears when the player enters the shop
    /// </summary>
    [PunRPC]
    public void ShopTutorial()
    {
        List<string> tutorialStrings = new List<string>();
        tutorialStrings.Add("At the shop, you can trade up your pages for more powerful pages. To trade, select pages from your inventory to put them up.");
        tutorialStrings.Add("Then select the page from you shop that you would like to trade for. If the level of your pages put up for trade match or exceed the level of the shop trade, you can do the trade.");

        _instantiateTutorialUI("Trading Pages at the Shop", tutorialStrings);
    }

    /// <summary>
    /// Creates the tutorial UI that appears when the player starts the boss fight
    /// </summary>
    [PunRPC]
    public void BossFightTutorial()
    {
        List<string> tutorialStrings = new List<string>();
        tutorialStrings.Add("Boss fight text 1");
        tutorialStrings.Add("Boss fight text 2");

        _instantiateTutorialUI("Boss Fights", tutorialStrings);
    }

    /// <summary>
    /// Creates the tutorial message that appears when the tutorial is complete
    /// </summary>
    [PunRPC]
    public void DemoCompleteTutorial()
    {
        List<string> tutorialStrings = new List<string>();
        tutorialStrings.Add("Tutorial complete text 1");
        tutorialStrings.Add("Tutorial complete text 2");

        _instantiateTutorialUI("Tutorial Complete!", tutorialStrings);
    }

    private void _instantiateTutorialUI(string tutorialTitle, List<string> tutorialStrings)
    {
        GameObject tutorialUI = (GameObject)Instantiate(m_tutorialUIPrefab.gameObject, Vector3.zero, Quaternion.identity);
        TutorialUIHandler tutorialUIHandler = tutorialUI.GetComponent<TutorialUIHandler>();

        tutorialUIHandler.PopulateMenu(tutorialTitle, tutorialStrings);
    }

    public void OnTutorialStart()
    {
        photonView.RPC(nameof(CreateStartMenu), PhotonTargets.All);
    }

    public void OnPageForRoomUIOpened()
    {
        if (!m_pageForRoomIsComplete)
        {
            photonView.RPC(nameof(CreatePageForRoomTutorial), PhotonTargets.All);
            m_pageForRoomIsComplete = true;
        }
    }

    public void OnCombatStarted()
    {
        photonView.RPC(nameof(CreateCombatTutorial), PhotonTargets.All);
    }

    public void OnCombatCleared()
    {
        photonView.RPC(nameof(CombatClearTutorial), PhotonTargets.All);
    }

    public void OnDeckManagementOpened()
    {
        if (!m_deckManagementIsComplete)
        {
            photonView.RPC(nameof(DeckManagementTutorial), PhotonTargets.All);
            m_deckManagementIsComplete = true;
        }
    }

    public void OnShopEntered()
    {
        photonView.RPC(nameof(ShopTutorial), PhotonTargets.All);
    }

    public void OnBossFightStarted()
    {
        photonView.RPC(nameof(BossFightTutorial), PhotonTargets.All);
    }

    public void OnDemoCompleted()
    {
        photonView.RPC(nameof(DemoCompleteTutorial), PhotonTargets.All);
    }
}
