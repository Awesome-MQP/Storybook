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

        // If it is not the master client, need to add this player to the player pawn list in the combat manager
        // The master client adds the player pawn to its combat manager when it spawns the pawns
        if (!PhotonNetwork.isMasterClient)
        {
            CombatManager combatManager = FindObjectOfType<CombatManager>();
            CombatPawn[] allPawns = combatManager.AllPawns;
            List<CombatPawn> allPawnsList = new List<CombatPawn>(allPawns);
            allPawnsList.Add(this);
            combatManager.SetAllPawns(allPawnsList);
        }
    }
}
