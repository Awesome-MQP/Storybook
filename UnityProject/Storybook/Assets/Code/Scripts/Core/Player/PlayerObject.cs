using Photon;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// An object that represents a player.
/// </summary>
public class PlayerObject : PunBehaviour, IConstructable<PhotonPlayer>, IConstructable<PlayerObject>
{
    private PhotonPlayer m_player;

    protected int m_floorNumber = 0;

    [SyncProperty]
    public PhotonPlayer Player
    {
        get { return m_player; }
        private set
        {
            Assert.IsNull(m_player);
            m_player = value;
            PropertyChanged();
        }
    }
    
    public string Name
    {
        get { return m_player.name; }
        set
        {
            Assert.IsTrue(m_player.isLocal);
            m_player.name = value;
        }
    }

    protected override void Awake()
    {
        DontDestroyOnLoad(gameObject);
        base.Awake();
    }

    public void Construct(PhotonPlayer player)
    {
        Assert.IsTrue(IsMine);

        Player = player;
        photonView.TransferController(player);
    }

    public void Construct(PlayerObject oldPlayer)
    {
        Assert.IsTrue(IsMine);

        Player = oldPlayer.Player;
        photonView.TransferController(Player);

        OnTakeOver(oldPlayer);
    }

    public override void OnStartPeer(bool wasSpawn)
    {
        if (wasSpawn)
        {
            GameManager gameManager = GameManager.GetInstance<GameManager>();
            gameManager.RegisterPlayerObject(this);
        }
    }

    public override void OnStartController(bool wasSpawn)
    {
        if (wasSpawn)
        {
            GameManager gameManager = GameManager.GetInstance<GameManager>();
            gameManager.RegisterPlayerObject(this);
        }
    }

    /// <summary>
    /// Get the player that this player considers next.
    /// </summary>
    /// <returns>The player that this player considers next.</returns>
    /// <remarks>This code should be network safe.</remarks>
    public virtual PlayerObject GetNext()
    {
        PhotonPlayer player = m_player.GetNext();
        return GameManager.GetInstance<GameManager>().GetPlayerObject(player);
    }

    /// <summary>
    /// Get the player that this player considers next.
    /// </summary>
    /// <returns>The player that this player considers next.</returns>
    /// <remarks>This code should be network safe.</remarks>
    public virtual PlayerObject GetPrev()
    {
        return GameManager.GetInstance<GameManager>().GetPlayerObject(m_player.GetPrev());
    }

    protected virtual void OnTakeOver(PlayerObject oldPlayer)
    {
        m_floorNumber = oldPlayer.m_floorNumber + 1;
    }

    public int FloorNumber
    {
        get { return m_floorNumber; }
    }
}
