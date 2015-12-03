using System;
using UnityEngine;
using System.Collections;
using Photon;
using UnityEngine.Assertions;
using UnityEngine.Networking;

/// <summary>
/// The representation of the real world player and there stats.
/// </summary>
public class PlayerEntity : PunBehaviour
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
}
