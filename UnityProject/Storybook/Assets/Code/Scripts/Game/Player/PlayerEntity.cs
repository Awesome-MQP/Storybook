using System;
using UnityEngine;
using System.Collections;
using Assets.Code.Scripts.Game.Player;
using Photon;
using UnityEngine.Assertions;
using UnityEngine.Networking;

/// <summary>
/// The representation of the real world player and there stats.
/// </summary>
public class PlayerEntity : PunBehaviour, IConstructable<PhotonPlayer>
{
    [SyncProperty]
    public string Name
    {
        get { return m_name; }
        protected set
        {
            m_name = value;
            PropertyChanged();
        }
    }

    [SyncProperty]
    public int HitPoints
    {
        get { return m_hitPoints; }
        protected set
        {
            m_hitPoints = value;
            PropertyChanged();
        }
    }

    [SyncProperty]
    public int MaxHitPoints
    {
        get { return m_maxHitPoints; }
        protected set
        {
            m_maxHitPoints = value;
            PropertyChanged();
        }
    }

    [SyncProperty]
    public int Attack
    {
        get { return m_attack; }
        protected set
        {
            m_attack = value;
            PropertyChanged();
        }
    }

    [SyncProperty]
    public int Defense
    {
        get { return m_defense; }
        protected set
        {
            m_defense = value;
            PropertyChanged();
        }
    }

    [SyncProperty]
    public int Speed
    {
        get { return m_speed; }
        protected set
        {
            m_speed = value;
            PropertyChanged();
        }
    }

    [SyncProperty]
    public PhotonPlayer RepresentedPlayer
    {
        get { return m_photonPlayer; }
        protected set
        {
            m_photonPlayer = value;
            PropertyChanged();
        }
    }

    public void Construct(PhotonPlayer player)
    {
        Assert.IsTrue(IsMine);
        Assert.IsFalse(m_hasConstructed);

        m_photonPlayer = player;
        PlayerManager.Instance.RegisterPlayer(this);

        PhotonNetwork.Spawn(photonView);
    }

    protected virtual void OnDestroy()
    {
        PlayerManager.Instance.UnregisterPlayer(this);
    }

    [SerializeField]
    private string m_name = string.Empty;

    [SerializeField]
    private int m_hitPoints = 10;

    [SerializeField]
    private int m_maxHitPoints = 10;

    [SerializeField]
    private int m_attack = 1;

    [SerializeField]
    private int m_defense = 1;

    [SerializeField]
    private int m_speed = 1;

    [SerializeField]
    private int m_luck = 1;

    private PhotonPlayer m_photonPlayer;

    public bool m_hasConstructed;
}
