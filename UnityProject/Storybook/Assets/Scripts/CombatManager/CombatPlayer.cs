using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class CombatPlayer : CombatPawn {

    /// <summary>
    /// The moves that are currently in the player's hand
    /// </summary>
    [SerializeField]
    private PlayerMove[] m_playerHand;

    public PlayerMove[] PlayerHand
    {
        get { return m_playerHand; }
    }

    void Awake()
    {
        if (!PhotonNetwork.isMasterClient)
        {
            CombatManager combatManager = FindObjectOfType<CombatManager>();
            CombatPawn[] playerPawns = combatManager.PlayerPawnList;
            List<CombatPawn> playerPawnsList = new List<CombatPawn>(playerPawns);
            playerPawnsList.Add(this);
            combatManager.SetPlayerPawnList(playerPawnsList);
        }
    }
}
