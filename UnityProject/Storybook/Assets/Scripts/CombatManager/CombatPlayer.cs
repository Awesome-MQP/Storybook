using UnityEngine;
using System.Collections;

public abstract class CombatPlayer : CombatPawn {

    [SerializeField]
    private PlayerMove[] m_playerHand;

    public PlayerMove[] PlayerHand
    {
        get { return m_playerHand; }
    }
}
