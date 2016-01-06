using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Code.Scripts.Game.Player;
using ExitGames.Client.Photon;
using Mono.Collections.Generic;
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
            s_playerLookup.Remove(m_photonPlayer);
            m_photonPlayer = value;
            s_playerLookup.Add(m_photonPlayer, this);
            s_playerQueue.AddLast(this);
            PropertyChanged();
        }
    }

    public static PlayerEntity GetPlayerEntity(PhotonPlayer player)
    {
        PlayerEntity entity;
        if(s_playerLookup.TryGetValue(player, out entity))
            return entity;

        return null;
    }

    public void Construct(PhotonPlayer player)
    {
        Assert.IsTrue(IsMine);
        Assert.IsFalse(m_hasConstructed);

        RepresentedPlayer = player;

        m_hasConstructed = true;
    }

    protected virtual void OnDestroy()
    {
        s_playerLookup.Remove(m_photonPlayer);
        s_playerQueue.Remove(this);
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

    private bool m_hasConstructed;

    private static Dictionary<PhotonPlayer, PlayerEntity> s_playerLookup = new Dictionary<PhotonPlayer, PlayerEntity>();
    private static LinkedList<PlayerEntity> s_playerQueue = new LinkedList<PlayerEntity>();
}
