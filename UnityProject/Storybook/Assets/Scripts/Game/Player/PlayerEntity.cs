using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;
using UnityEngine.Networking;
using Photon;

/// <summary>
/// The representation of the real world player and there stats.
/// </summary>
public class PlayerEntity : Photon.MonoBehaviour
{
    public string Name
    {
        get { return m_name; }
    }
    
    //[SyncProperty]
    public Genre PlayerGenre
    {
        get { return m_playerGenre; }
        set
        {
            if(gameObject.GetPhotonView().isMine)
            {
                m_playerGenre = value;
                //OnPropertyChanged();
            }
            else
            {
                // Can do something here if we're not the owner, but I'm not sure what we can do at the moment.
            }
        }
    }

    //[SyncProperty]
    public int HitPointsMod
    {
        get { return m_hitPointsMod; }
        set
        {
            if (gameObject.GetPhotonView().isMine)
            {
                m_hitPointsMod = value;
                //OnPropertyChanged();
            }
        }
    }

    //[SyncProperty]
    public int HitPoints
    {
        get { return m_hitPoints; }
        set
        {
            if (gameObject.GetPhotonView().isMine)
            {
                m_hitPoints = value;
                //OnPropertyChanged();
            }
        }
    }

    //[SyncProperty]
    public int MaxHitPoints
    {
        get { return m_maxHitPoints; }
        set
        {
            if (gameObject.GetPhotonView().isMine)
            {
                m_maxHitPoints = value;
                //OnPropertyChanged();
            }
        }
    }

    //[SyncProperty]   
    public int AttackMod
    {
        get { return m_attackMod; }
        set
        {
            if(gameObject.GetPhotonView().isMine)
            {
                m_attackMod = value;
                //OnPropertyChanged();
            }
        }
    }

    //[SyncProperty]
    public int Attack
    {
        get { return m_attack; }
        set
        {
            if (gameObject.GetPhotonView().isMine)
            {
                m_attack = value;
                //OnPropertyChanged();
            }
        }
    }

    //[SyncProperty]
    public int DefenseMod
    {
        get { return m_defenseMod; }
        set
        {
            if (gameObject.GetPhotonView().isMine)
            {
                m_defenseMod = value;
                //OnPropertyChanged();
            }
        }
    }

    //[SyncProperty]
    public int Defense
    {
        get { return m_defense; }
        set
        {
            if (gameObject.GetPhotonView().isMine)
            {
                m_defense = value;
                //OnPropertyChanged();
            }
        }
    }

    //[SyncProperty]
    public int SpeedMod
    {
        get { return m_speedMod; }
        set
        {
            if (gameObject.GetPhotonView().isMine)
            {
                m_speedMod = value;
                //OnPropertyChanged();
            }
        }
    }

    //[SyncProperty]
    public int Speed
    {
        get { return m_speed; }
        set
        {
            if (gameObject.GetPhotonView().isMine)
            {
                m_speed = value;
                //OnPropertyChanged();
            }
        }
    }

    //[SyncProperty]
    public PlayerInventory Inventory
    {
        get { return m_playerInventory; }
    }

    /// <summary>
    /// Will rename the player. This will only work either on the client who this object represents or on the server.
    /// On the owning player the name will not be set immedietly but instead will have a delay of one round trip to the server.
    /// </summary>
    /// <param name="newName"></param>
    public void Rename(string newName)
    {
        if (gameObject.GetPhotonView().isMine)//PhotonNetwork.isNonMasterClientInRoom) // isLocalPlayer)
        {
            CmdInternalRenameServer(newName);
        }
    }

    [PunRPC]
    private void CmdInternalRenameServer(string newName)
    {
        m_name = newName;
        //OnPropertyChanged();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.isWriting)
        {
            stream.SendNext(m_attack);
            stream.SendNext(m_attackMod);
            stream.SendNext(m_defense);
            stream.SendNext(m_defenseMod);
            stream.SendNext(m_hitPoints);
            stream.SendNext(m_hitPointsMod);
            stream.SendNext(m_maxHitPoints);
            stream.SendNext(m_name);
            stream.SendNext(m_playerGenre);
            stream.SendNext(m_playerInventory);
            stream.SendNext(m_speed);
            stream.SendNext(m_speedMod);
        }
        else
        {
            this.m_attack = (int)stream.ReceiveNext();
            this.m_attackMod = (int)stream.ReceiveNext();
            this.m_defense = (int)stream.ReceiveNext();
            this.m_defenseMod = (int)stream.ReceiveNext();
            this.m_hitPoints = (int)stream.ReceiveNext();
            this.m_hitPointsMod = (int)stream.ReceiveNext();
            this.m_maxHitPoints = (int)stream.ReceiveNext();
            this.m_name = (string)stream.ReceiveNext();
            this.m_playerGenre = (Genre)stream.ReceiveNext();
            this.m_playerInventory = (PlayerInventory)stream.ReceiveNext();
            this.m_speed = (int)stream.ReceiveNext();
            this.m_speedMod = (int)stream.ReceiveNext();
        }
    }

    [SerializeField]
    private Genre m_playerGenre = Genre.None;

    [SerializeField]
    private string m_name = string.Empty;

    [SerializeField]
    private int m_hitPoints = 10;

    [SerializeField]
    private int m_maxHitPoints = 10;

    [SerializeField]
    private int m_hitPointsMod = 0;

    [SerializeField]
    private int m_attackMod = 0;

    [SerializeField]
    private int m_attack = 1;

    [SerializeField]
    private int m_defenseMod = 0;

    [SerializeField]
    private int m_defense = 1;

    [SerializeField]
    private int m_speedMod = 0;

    [SerializeField]
    private int m_speed = 1;

    [SerializeField]
    private PlayerInventory m_playerInventory;
}
