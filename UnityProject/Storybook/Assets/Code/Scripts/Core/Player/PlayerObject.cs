using Photon;
using UnityEngine.Assertions;

/// <summary>
/// An object that represents a player.
/// </summary>
public class PlayerObject : PunBehaviour, IConstructable<PhotonPlayer>
{
    private PhotonPlayer m_player;

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

    public void Construct(PhotonPlayer player)
    {
        Assert.IsTrue(IsMine);

        Player = player;
        photonView.TransferController(player);
    }

    /// <summary>
    /// Get the player that this player considers next.
    /// </summary>
    /// <returns>The player that this player considers next.</returns>
    /// <remarks>This code should be network safe.</remarks>
    public virtual PlayerObject GetNext()
    {
        return GameManager.GetInstance<GameManager>().GetPlayerObject(m_player.GetNext());
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
}
