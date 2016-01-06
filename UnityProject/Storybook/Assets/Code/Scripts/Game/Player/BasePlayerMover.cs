
using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

public class BasePlayerMover : RoomMover
{
    private HashSet<PlayerEntity> m_registeredPlayers = new HashSet<PlayerEntity>();

    /// <summary>
    /// Gets the next photon player to be the leader.
    /// </summary>
    /// <returns>The next player to be the leader.</returns>
    public virtual PlayerEntity GetNextLeader()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Changes to the next leader.
    /// </summary>
    public void ChangeLeader()
    {
        Assert.IsTrue(IsMine);

        photonView.TransferController(GetNextLeader().RepresentedPlayer);
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

    public void ChangeLeader(PlayerEntity newLeader)
    {
        ChangeLeader(newLeader.RepresentedPlayer);
    }

    /// <summary>
    /// Registers a player entity with this player mover.
    /// </summary>
    /// <param name="player">The player to register with this mover.</param>
    public void RegisterPlayer(PlayerEntity player)
    {
        if(OnRegisterPlayer(player))
            m_registeredPlayers.Add(player);
    }

    /// <summary>
    /// Called when a new player is registered.
    /// </summary>
    /// <param name="player">The player to register.</param>
    /// <returns>True if the player should be registered.</returns>
    protected virtual bool OnRegisterPlayer(PlayerEntity player)
    {
        return true;
    }
}
