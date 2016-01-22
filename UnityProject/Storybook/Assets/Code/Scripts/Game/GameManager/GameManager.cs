using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

public class GameManager : Photon.PunBehaviour
{

    [SerializeField]
    private CombatManager m_combatInstancePrefab;

    [SerializeField]
    private Vector3 m_defaultLocation;

    [SerializeField]
    private EnemyTeam m_enemyTeamForCombat;

    [SerializeField]
    private PlayerTeam m_playerTeamForCombat;

    [SerializeField]
    private CombatPlayer m_playerPawn;

    [SerializeField]
    private DungeonMaster m_dungeonMaster;

    [Tooltip("The player object to spawn for all players in the game.")]
    private ResourceAsset m_defaultPlayerObject = new ResourceAsset(typeof(PlayerObject));

    private int m_deckSize = 20;

    private GameObject m_combatInstance;
    private MusicManager m_musicMgr;

    private Dictionary<PhotonPlayer, PlayerObject> m_playerObjects = new Dictionary<PhotonPlayer, PlayerObject>();

    private static GameManager s_instance;

    public static T GetInstance<T>() where T : GameManager
    {
        return s_instance as T;
    }

    protected override void Awake()
    {
        base.Awake();
        s_instance = this;
    }

    public override void OnStartOwner(bool wasSpawn)
    {
        _startup();
        StartGame();
    }

    void Start ()
    {
        m_musicMgr = FindObjectOfType<MusicManager>();
    }

    /// <summary>
    /// Starts a combat instance and sets the players for the combat manager to the list of players given to this function
    /// </summary>
    public void StartCombat()
    {
        if (PhotonNetwork.isMasterClient)
        {
            /*
            GameObject dungeonMaster = PhotonNetwork.Instantiate(m_dungeonMaster.name, Vector3.zero, Quaternion.identity, 0);
            PhotonNetwork.Spawn(dungeonMaster.GetComponent<PhotonView>());
            */

            GameObject playerTeam = PhotonNetwork.Instantiate(m_playerTeamForCombat.name, Vector3.zero, Quaternion.identity, 0);
            PhotonNetwork.Spawn(playerTeam.GetComponent<PhotonView>());

            GameObject enemyTeam = PhotonNetwork.Instantiate(m_enemyTeamForCombat.name, Vector3.zero, Quaternion.identity, 0);
            PhotonNetwork.Spawn(enemyTeam.GetComponent<PhotonView>());

            for(int i = 0; i < PhotonNetwork.playerList.Length; i++)
            {
                playerTeam.GetComponent<CombatTeam>().AddPawnToSpawn(m_playerPawn);
            }

            playerTeam.GetComponent<CombatTeam>().TeamId = 1;
            enemyTeam.GetComponent<CombatTeam>().TeamId = 2;

            List<CombatTeam> combatTeams = new List<CombatTeam>
            {
                playerTeam.GetComponent<CombatTeam>(),
                enemyTeam.GetComponent<CombatTeam>()
            };

            List<PlayerObject> playersEnteringCombat = new List<PlayerObject>(FindObjectsOfType<PlayerEntity>());
            Vector3 combatPosition = new Vector3(m_defaultLocation.x + 1000, m_defaultLocation.y + 1000, m_defaultLocation.z + 1000);
            m_combatInstance = PhotonNetwork.Instantiate("CombatInstance", combatPosition, Quaternion.identity, 0);
            PhotonNetwork.Spawn(m_combatInstance.GetComponent<PhotonView>());
            CombatManager combatManager = m_combatInstance.GetComponent<CombatManager>();
            combatManager.SetCombatTeamList(combatTeams);

            //CameraManager m_camManager = FindObjectOfType<CameraManager>();
            //m_camManager.SwitchToCombatCamera(); // Switch to combat camera.

        }
    }

    public void StartGame()
    {
        MapManager mapManager = FindObjectOfType<MapManager>();
        mapManager.GenerateMap();
        RoomObject startRoom = mapManager.PlaceStartRoom();
        StorybookPlayerMover playerMover = FindObjectOfType<StorybookPlayerMover>();
        playerMover.SpawnInRoom(startRoom);
        List<PlayerWorldPawn> players = new List<PlayerWorldPawn>(FindObjectsOfType<PlayerWorldPawn>());
        int i = 0;
        foreach(PlayerWorldPawn player in players)
        {
            player.transform.position = playerMover.PlayerPositions[i].transform.position;
            playerMover.RegisterPlayerWorldPawn(player);
            i++;
        }
        Camera.main.transform.position = startRoom.CameraNode.position;
        Camera.main.transform.rotation = startRoom.CameraNode.rotation;
    }

    public void TransitionToCombat()
    {
        Debug.Log("Transitioning to combat");
        photonView.RPC("EnableMovementComponents", PhotonTargets.All, false);
        EnableMovementComponents(false);
        StartCombat();
    }

    private void _TransitionToOverworld()
    {
        Debug.Log("Transitioning to overworld");
        photonView.RPC("EnableMovementComponents", PhotonTargets.All, true);
    }

    [PunRPC]
    protected void EnableMovementComponents(bool isEnable)
    {
        Debug.Log("Disabling movement components");
        MapManager mapManager = FindObjectOfType<MapManager>();
        mapManager.LoadMap(isEnable);
        mapManager.enabled = isEnable;

        StorybookPlayerMover playerMover = FindObjectOfType<StorybookPlayerMover>();
        if (!isEnable)
        {
            playerMover.EnterCombat();
        }
        else
        {
            playerMover.ExitCombat();
        }
    }

    /// <summary>
    /// Ends the combat instance that has the given CombatManager
    /// </summary>
    public void EndCombat()
    {
        CombatManager cm = m_combatInstance.GetComponent<CombatManager>();

        Debug.Log("Destroying all teams");

        cm.DestroyAllTeams();

        Debug.Log("Destroying combat instance");

        GameObject currentCombatInstance = m_combatInstance;

        PhotonNetwork.Destroy(currentCombatInstance);
        Destroy(currentCombatInstance);

        StartCoroutine(m_musicMgr.Fade(m_musicMgr.MusicTracks[0], 5, true));

        _TransitionToOverworld();

        /*
        CameraManager m_camManager = FindObjectOfType<CameraManager>();
        m_camManager.SwitchToOverworldCamera(); // Switch to the overworld camera.
        */
    }

    public PlayerInventory GetLocalPlayerInventory()
    {
        PlayerInventory[] allInventories = FindObjectsOfType<PlayerInventory>();
        foreach(PlayerInventory pi in allInventories)
        {
            if (pi.PlayerId == PhotonNetwork.player.ID)
            {
                return pi;
            }
        }
        return null;
    }
    
    /// <summary>
    /// Gets all of the PlayerObject in the game
    /// </summary>
    /// <returns>An array of all the PlayerObject currently in the game</returns>
    public PlayerObject[] GetAllPlayers()
    {
        return m_playerObjects.Values.ToArray();
    }

    /// <summary>
    /// Gets all of the PlayerObject in the game
    /// </summary>
    /// <typeparam name="T">The type of player object to get.</typeparam>
    /// <returns>An array of all the PlayerObject currently in the game</returns>
    public T[] GetAllPlayers<T>() where T : PlayerObject
    {
        return m_playerObjects.Values.OfType<T>().ToArray();
    }

    public PlayerObject GetPlayerObject(PhotonPlayer player)
    {
        return GetPlayerObject<PlayerObject>(player);
    }

    public T GetPlayerObject<T>(PhotonPlayer player) where T : PlayerObject
    {
        PlayerObject playerObject;
        m_playerObjects.TryGetValue(player, out playerObject);
        return playerObject as T;
    }

    /// <summary>
    /// Creates the player object for a player.
    /// </summary>
    /// <param name="player">The photon player to create the player object for.</param>
    /// <returns>The player object for a player.</returns>
    protected virtual PlayerObject CreatePlayerObject(PhotonPlayer player)
    {
        PlayerObject playerObj = PhotonNetwork.Instantiate(m_defaultPlayerObject, Vector3.zero, Quaternion.identity, 0).GetComponent<PlayerObject>();
        return playerObj;
    }

    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        //When a player connects setup the player object.
        if(IsMine)
            _setupPlayer(newPlayer);
    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        if (IsMine)
        {
            PlayerObject playerObject;
            m_playerObjects.TryGetValue(otherPlayer, out playerObject);

            Assert.IsNotNull(playerObject);

            Destroy(playerObject);
        }
    }

    /// <summary>
    /// Sets up a single player object.
    /// </summary>
    /// <param name="player">The player to use for the setup.</param>
    /// <returns>The player object that was created as a result.</returns>
    private PlayerObject _setupPlayer(PhotonPlayer player)
    {
        Assert.IsTrue(IsMine);

        PlayerObject playerObject = CreatePlayerObject(player);
        playerObject.Construct(player);
        PhotonNetwork.Spawn(playerObject.photonView);

        m_playerObjects.Add(player, playerObject);

        return playerObject;
    }

    /// <summary>
    /// Runs the startup code.
    /// </summary>
    private void _startup()
    {
        Assert.IsTrue(IsMine);

        PhotonPlayer[] players = PhotonNetwork.playerList;
        foreach (PhotonPlayer player in players)
        {
            PlayerObject playerObject = _setupPlayer(player);
        }
    }

    public EnemyTeam EnemyTeamForCombat
    {
        get { return m_enemyTeamForCombat; }
        set { m_enemyTeamForCombat = value; }
    }

    public PlayerTeam PlayerTeamForCombat
    {
        get { return m_playerTeamForCombat; }
        set { m_playerTeamForCombat = value; }
    }

    public int DeckSize
    {
        get { return m_deckSize; }
    }
}
