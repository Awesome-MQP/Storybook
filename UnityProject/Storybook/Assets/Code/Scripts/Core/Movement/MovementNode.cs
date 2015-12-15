using UnityEngine;
using System.Collections;
using Photon;

/// <summary>
/// A movement node that represents a point of interest to move to.
/// </summary>
[RequireComponent(typeof(PhotonView))]
public abstract class MovementNode : PunBehaviour
{
    public void Enter(NetworkNodeMover mover)
    {
        if (IsMine)
        {
            photonView.RPC("_OnNetworkEnter", PhotonTargets.Others, mover.photonView.viewID);
            OnEnter(mover);
        }
    }

    public void Leave(NetworkNodeMover mover)
    {
        if (IsMine)
        {
            photonView.RPC("_OnNetworkLeave", PhotonTargets.Others, mover.photonView.viewID);
            OnLeave(mover);
        }
    }

    protected abstract void OnEnter(NetworkNodeMover mover);

    protected abstract void OnLeave(NetworkNodeMover mover);

    [PunRPC]
    public void _OnNetworkEnter(int networkNodeMoverId)
    {
        PhotonView view = PhotonView.Find(networkNodeMoverId);
        OnEnter(view.GetComponent<NetworkNodeMover>());
    }

    [PunRPC]
    public void _OnNetworkLeave(int networkNodeMoverId)
    {
        PhotonView view = PhotonView.Find(networkNodeMoverId);
        OnLeave(view.GetComponent<NetworkNodeMover>());
    }
}
