
using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

/// <summary>
/// Base class for player movers.
/// </summary>
public abstract class BasePlayerMover : RoomMover
{
    private HashSet<PlayerObject> m_registeredPlayers = new HashSet<PlayerObject>();

    public PlayerObject Leader
    {
        get { return GameManager.GetInstance<GameManager>().GetPlayerObject<PlayerEntity>(photonView.Controller); }
    }

    /// <summary>
    /// Gets the next photon player to be the leader.
    /// </summary>
    /// <returns>The next player to be the leader.</returns>
    public virtual PlayerObject GetNextLeader()
    {
        return Leader.GetNext() as PlayerEntity;
    }

    /// <summary>
    /// Changes to the next leader.
    /// </summary>
    public void ChangeLeader()
    {
        Assert.IsTrue(IsMine);

        photonView.TransferController(GetNextLeader().Player);
    }

    /// <summary>
    /// Changes to a specified leader.
    /// </summary>
    /// <param name="newLeader">The player to become the new leader.</param>
    public void ChangeLeader(PhotonPlayer newLeader)
    {
        Assert.IsTrue(IsMine);

        photonView.TransferController(newLeader);
    }

    public void ChangeLeader(PlayerObject newLeader)
    {
        ChangeLeader(newLeader.Player);
    }

    /// <summary>
    /// Registers a player entity with this player mover.
    /// </summary>
    /// <param name="player">The player to register with this mover.</param>
    public void RegisterPlayer(PlayerObject player)
    {
        if(OnRegisterPlayer(player))
            m_registeredPlayers.Add(player);
    }

    /// <summary>
    /// Called when a new player is registered.
    /// </summary>
    /// <param name="player">The player to register.</param>
    /// <returns>True if the player should be registered.</returns>
    protected virtual bool OnRegisterPlayer(PlayerObject player)
    {
        return true;
    }

    protected override IEnumerable<StateDelegate> OnEnterRoom()
    {
        ChangeLeader();

        return base.OnEnterRoom();
    }
}
