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

    // Set the stat mods from pages in a player's hand to their stats
    public void CalculateStatMods()
    {
        foreach(PlayerMove p in m_playerHand)
        {
            switch(p.MoveGenre)
            {
                case Genre.Horror:
                    AttackMod = (AttackMod + p.MoveLevel);
                    break;
                case Genre.SciFi:
                    DefenseMod = (DefenseMod + p.MoveLevel);
                    break;
                case Genre.GraphicNovel:
                    SpeedMod = (SpeedMod + p.MoveLevel);
                    break;
                case Genre.Fantasy:
                    HitpointsMod = (HitpointsMod + p.MoveLevel);
                    break;
                default:
                    break;
            }
        }
    }
}
