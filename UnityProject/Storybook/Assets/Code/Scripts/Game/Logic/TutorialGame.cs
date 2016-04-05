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
    private bool m_hasShownCombatTutorial = false;
    private bool m_hasShownShopTutorial = false;
    private bool m_hasShownCombatClear = false;

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
            tutorialStrings.Add("Welcome to your first dungeon in Storybook. Throughout the game, you will be using mystical pages as attacks and to build the rooms of the dungeons.");
            tutorialStrings.Add("Explore the dungeon looking for the exit which evil bosses will be guarding");
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
        if (!m_hasShownCombatTutorial)
        {
            List<string> tutorialStrings = new List<string>();
            tutorialStrings.Add("In combat, you will draw pages from your deck and use them as attacks or boosts. Note, using pages in combat do not cause you to lose them.");
            tutorialStrings.Add("The power of attack pages are more powerful the higher their level, and damage depends on the type of the user and the type of the character being attacked.");
            tutorialStrings.Add("Boosts pages temporarily increase stats. Red horror pages heal players, green fantasy pages speed up players...");
            tutorialStrings.Add("...yellow comic book pages increase attack, and blue sci-fi pages increase defense.");

            _instantiateTutorialUI("Fighting Enemies", tutorialStrings);
            m_hasShownCombatTutorial = true;
        }
    }

    /// <summary>
    /// Creates the tutorial UI that appears when the enemies are defeated
    /// </summary>
    [PunRPC]
    public void CombatClearTutorial()
    {
        if (!m_hasShownCombatClear)
        {
            List<string> tutorialStrings = new List<string>();
            tutorialStrings.Add("Upon clearing a combat, you can select one new page from the drops to add to your inventory.");
            tutorialStrings.Add("The higher the level of the enemies, the better the rewards will be. Also, the dropped pages are more likely to match the genre of the enemies");

            _instantiateTutorialUI("Reaping Rewards of Combat", tutorialStrings);
            m_hasShownCombatClear = true;
        }
    }

    /// <summary>
    /// Creates the tutorial UI that appears the first time the deck management menu is opened
    /// </summary>
    [PunRPC]
    public void DeckManagementTutorial()
    {
        if (!m_deckManagementIsComplete)
        {
            List<string> tutorialStrings = new List<string>();
            tutorialStrings.Add("To fight monsters in Storybook, you must construct a deck of the mythical pages.");
            tutorialStrings.Add("Using the deck management screen, you can move pages around between your deck, the left side, and the rest of your inventory.");
            tutorialStrings.Add("Deck building is an important aspect of the game, so build it how you would like your character to play.");

            _instantiateTutorialUI("Managing your Inventory and Deck", tutorialStrings);
            m_deckManagementIsComplete = true;
        }
    }

    /// <summary>
    /// Creates the tutorial UI that appears when the player enters the shop
    /// </summary>
    [PunRPC]
    public void ShopTutorial()
    {
        if (!m_hasShownShopTutorial)
        {
            List<string> tutorialStrings = new List<string>();
            tutorialStrings.Add("At the shop, you can trade up your pages for more powerful pages. To trade, select pages from your inventory to put them up.");
            tutorialStrings.Add("Then select the page from you shop that you would like to trade for. If the level of your pages put up for trade match or exceed the level of the shop trade, you can do the trade.");

            _instantiateTutorialUI("Trading Pages at the Shop", tutorialStrings);
            m_hasShownShopTutorial = true;
        }
    }

    /// <summary>
    /// Creates the tutorial UI that appears when the player starts the boss fight
    /// </summary>
    [PunRPC]
    public void BossFightTutorial()
    {
        List<string> tutorialStrings = new List<string>();
        tutorialStrings.Add("Before being able to use the stairs to move to the next floor, you must defeat the boss guarding it.");
        tutorialStrings.Add("Boss characters are more powerful than regular enemies and often have a few tricks up their sleeves that the regular enemies do not have.");

        _instantiateTutorialUI("Boss Fights", tutorialStrings);
    }

    /// <summary>
    /// Creates the tutorial message that appears when the tutorial is complete
    /// </summary>
    [PunRPC]
    public void DemoCompleteTutorial()
    {
        List<string> tutorialStrings = new List<string>();
        tutorialStrings.Add("Good job, you have finished the tutorial! Now that you have learned the basics, tackle the full solo game or join a game with some friends!");

        TutorialUIHandler uiHandler = _instantiateTutorialUI("Tutorial Complete!", tutorialStrings);
        uiHandler.changeFinishButtonOnClick();
    }

    private TutorialUIHandler _instantiateTutorialUI(string tutorialTitle, List<string> tutorialStrings)
    {
        GameObject tutorialUI = (GameObject)Instantiate(m_tutorialUIPrefab.gameObject, Vector3.zero, Quaternion.identity);
        TutorialUIHandler tutorialUIHandler = tutorialUI.GetComponent<TutorialUIHandler>();

        tutorialUIHandler.PopulateMenu(tutorialTitle, tutorialStrings);
        return tutorialUIHandler;
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

    public void OnCharacterSelect()
    {
    }
}
