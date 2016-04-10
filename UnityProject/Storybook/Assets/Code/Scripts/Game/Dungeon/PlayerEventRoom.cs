
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class PlayerEventRoom : RoomObject
{
    private bool m_inRoomEvent;

    private HashSet<PhotonPlayer> m_readyPlayers = new HashSet<PhotonPlayer>();

    [SyncProperty]
    protected bool IsInRoomEvent
    {
        get { return m_inRoomEvent; }
        set
        {
            Assert.IsTrue(ShouldBeChanging);
            m_inRoomEvent = value;

            if (value)
                OnNetworkEvent();

            PropertyChanged();
        }
    }


    protected override IEnumerable OnRoomEvent(RoomMover mover)
    {
        if (!(mover is BasePlayerMover))
            yield break;

        m_readyPlayers.Clear();

        IsInRoomEvent = true;

        while (m_readyPlayers.Count < PhotonNetwork.playerList.Length)
        {
            yield return null;
        }

        IsInRoomEvent = false;
    }

    protected void LocalPlayerFinished()
    {
        photonView.RPC(nameof(_playerFinished), Owner, PhotonNetwork.player);
    }

    /// <summary>
    /// Called to tell the room that the event has finished for a player.
    /// </summary>
    /// <param name="player">The player that the room has finished for.</param>
    [PunRPC(AllowFullCommunication = true)]
    protected void _playerFinished(PhotonPlayer player)
    {
        m_readyPlayers.Add(player);
        OnPlayerFinished(player);
    }
    
    /// <summary>
    /// Called when the network event takes place.
    /// </summary>
    protected abstract void OnNetworkEvent();

    protected virtual void OnPlayerFinished(PhotonPlayer player)
    {
        
    }
}