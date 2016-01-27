using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

public abstract class GameManager : Photon.PunBehaviour
{
    [SerializeField]
    [Tooltip("The player object to spawn for all players in the game.")]
    private ResourceAsset m_defaultPlayerObject = new ResourceAsset(typeof(PlayerObject));
    
    [SerializeField]
    [Tooltip("When false all old player objects will be destroyed when this game manager starts up.")]
    private bool m_keepOldPlayerObjects = false;

    [SerializeField]
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
        OnStartGame();
        photonView.RPC(nameof(_rpcOnStartGame), PhotonTargets.Others);
    }

    void Start ()
    {
        m_musicMgr = FindObjectOfType<MusicManager>();
    }

    protected virtual void OnStartGame()
    {
        
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

    public void RegisterPlayerObject(PlayerObject playerObject)
    {
        m_playerObjects.Add(playerObject.Player, playerObject);
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

            m_playerObjects.Remove(otherPlayer);
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

    private PlayerObject _setupPlayer(PlayerObject oldPlayer)
    {
        Assert.IsTrue(IsMine);

        PhotonPlayer player = oldPlayer.Player;

        PlayerObject playerObject = CreatePlayerObject(player);
        playerObject.Construct(oldPlayer);
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

        PlayerObject[] existingPlayers = FindObjectsOfType<PlayerObject>();
        foreach (PlayerObject existingPlayer in existingPlayers)
        {
            //If we keep the old player then just add it back to the list, otherwise destroy it and create the new one.
            if (m_keepOldPlayerObjects)
                m_playerObjects.Add(existingPlayer.Player, existingPlayer);
            else
            {
                _setupPlayer(existingPlayer);
                Destroy(existingPlayer);
            }
        }

        //Create a new player for all players that have yet to get a player object
        PhotonPlayer[] players = PhotonNetwork.playerList;
        foreach (PhotonPlayer player in players)
        {
            if (!m_playerObjects.ContainsKey(player))
            {
                _setupPlayer(player);
            }
        }
    }

    [PunRPC]
    private void _rpcOnStartGame()
    {
        OnStartGame();
    }
}
