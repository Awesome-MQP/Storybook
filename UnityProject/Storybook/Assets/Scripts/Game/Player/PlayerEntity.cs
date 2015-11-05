using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;
using UnityEngine.Networking;

/// <summary>
/// The representation of the real world player and there stats.
/// </summary>
public class PlayerEntity : Photon.MonoBehaviour
{
    public string Name
    {
        get { return m_name; }
    }

    public int HitPoints
    {
        get { return m_hitPoints; }
        set
        {
            if (PhotonNetwork.isMasterClient)
                m_hitPoints = value;
        }
    }

    public int MaxHitPoints
    {
        get { return m_maxHitPoints; }
        set
        {
            if (PhotonNetwork.isMasterClient)
                m_maxHitPoints = value;
        }
    }

    public int Attack
    {
        get { return m_attack; }
        set
        {
            if(PhotonNetwork.isMasterClient)
                m_attack = value;
        }
    }

    public int Defense
    {
        get { return m_defense; }
        set
        {
            if (PhotonNetwork.isMasterClient)
                m_defense = value;
        }
    }

    public int Speed
    {
        get { return m_speed; }
        set
        {
            if(PhotonNetwork.isMasterClient)
                m_speed = value;
        }
    }

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
        if(PhotonNetwork.isMasterClient || PhotonNetwork.isNonMasterClientInRoom) // isLocalPlayer)
            CmdInternalRenameServer(newName);
    }

    [Command]
    private void CmdInternalRenameServer(string newName)
    {
        m_name = newName;
    }

    [SyncVar, SerializeField]
    private string m_name = string.Empty;

    [SyncVar, SerializeField]
    private int m_hitPoints = 10;

    [SyncVar, SerializeField]
    private int m_maxHitPoints = 10;

    [SyncVar, SerializeField]
    private int m_attack = 1;

    [SyncVar, SerializeField]
    private int m_defense = 1;

    [SyncVar, SerializeField]
    private int m_speed = 1;

    [SyncVar, SerializeField]
    private int m_luck = 1;

    [SyncVar, SerializeField]
    private PlayerInventory m_playerInventory;
}
