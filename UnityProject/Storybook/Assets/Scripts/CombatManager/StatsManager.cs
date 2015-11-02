using UnityEngine;
using System.Collections;

public static class StatsManager {

    private static float m_positiveSTABBonus = 1f;
    private static float m_neutralSTABBonus = 0.5f;
    private static float m_negativeSTABBonus = 0.3f;
    private static float m_positiveTypeMatchBonus = 1f;
    private static float m_neutralTypeMatchBonus = 0.5f;
    private static float m_negativeTypeMatchBonus = 0.25f;

    /// <summary>
    /// Does damage calculation. This version of the formula reflects the design change from late A-term.
    /// </summary>
    /// <param name="attackerGenre">The Genre of the attacker.</param>
    /// <param name="defenderGenre">The Genre of the defender.</param>
    /// <param name="attackerMoveGenre">The Genre of the attacker's move.</param>
    /// <param name="attackerStr">The base Str stat of the attacker.</param>
    /// <param name="defenderDef">The base Def stat of the defender.</param>
    /// <returns></returns>
    public static int CalcDamage(Genre attackerGenre, Genre defenderGenre, Genre attackerMoveGenre, float attackerStr, float defenderDef)
    {
        float sameTypeMoveBonus = GetSameTypeBonus(attackerGenre, attackerMoveGenre);
        float typeAdvantageBonus = GetTypeAdvantageBonus(attackerMoveGenre, defenderGenre);

        float totalDmg = attackerStr * (sameTypeMoveBonus * typeAdvantageBonus) - defenderDef;

        if (totalDmg < 0)
        { return 0; }
        else
        { return Mathf.FloorToInt(totalDmg); }
    }

    /// <summary>
    /// Given a player's Genre and the Genre of their attack, determines what Same Type Attack Bonus the player will get.
    /// </summary>
    /// <param name="ofPlayer">The Genre of the player.</param>
    /// <param name="ofAttack">The Genre of the weapon held by that player</param>
    /// <returns>Some bonus/penalty based on the inputs.</returns>
    public static float GetSameTypeBonus(Genre ofPlayer, Genre ofAttack)
    {
        switch (ofPlayer)
        {
            // Each case returns a STAB bonus based on how the attack's type compares to the user's.
            // Same type results in a 1.7x mult, the type that either counters or is countered by the player's type results
            // in a 0.6x mult, and the last "neutral" type results in a 1x mult (no change).
            case Genre.Fantasy:
                if(ofAttack == Genre.Fantasy)
                { return m_positiveSTABBonus; }
                else if(ofAttack == Genre.GraphicNovel)
                { return m_neutralSTABBonus; }
                else
                { return m_negativeSTABBonus; }

            case Genre.GraphicNovel:
                if(ofAttack == Genre.GraphicNovel)
                { return m_positiveSTABBonus; }
                else if(ofAttack == Genre.Fantasy)
                { return m_neutralSTABBonus; }
                else
                { return m_negativeSTABBonus; }

            case Genre.Horror:
                if (ofAttack == Genre.Horror)
                { return m_positiveSTABBonus; }
                else if (ofAttack == Genre.SciFi)
                { return m_neutralSTABBonus; }
                else
                { return m_negativeSTABBonus; }

            case Genre.SciFi:
                if (ofAttack == Genre.SciFi)
                { return m_positiveSTABBonus; }
                else if (ofAttack == Genre.Horror)
                { return m_neutralSTABBonus; }
                else
                { return m_negativeSTABBonus; }

            default:
                return 1f;
        }
    }

    /// <summary>
    /// Given the player's weapon type and the type of the opponent, calculates the type advantage damage multiplier.
    /// </summary>
    /// <param name="ofPlayer"></param>
    /// <param name="ofTarget"></param>
    /// <returns></returns>
    public static float GetTypeAdvantageBonus(Genre ofPlayerAttack, Genre ofTarget)
    {
        switch (ofPlayerAttack)
        {
            // Each case returns a STAB bonus based on how the weapon's type compares to the user's.
            // Same type results in a 1.5x mult, the type that either counters or is countered by the player's type results
            // in a 0.5x mult, and the last "neutral" type results in a 1x mult (no change).
            case Genre.Fantasy:
                if (ofTarget == Genre.SciFi)
                { return m_positiveTypeMatchBonus; }
                else if (ofTarget == Genre.Horror)
                { return m_neutralTypeMatchBonus; }
                else
                { return m_negativeTypeMatchBonus; }

            case Genre.GraphicNovel:
                if (ofTarget == Genre.Horror)
                { return m_positiveTypeMatchBonus; }
                else if (ofTarget == Genre.SciFi)
                { return m_neutralTypeMatchBonus; }
                else
                { return m_negativeTypeMatchBonus; }

            case Genre.Horror:
                if (ofTarget == Genre.Fantasy)
                { return m_positiveTypeMatchBonus; }
                else if (ofTarget == Genre.GraphicNovel)
                { return m_neutralTypeMatchBonus; }
                else
                { return m_negativeTypeMatchBonus; }

            case Genre.SciFi:
                if (ofTarget == Genre.GraphicNovel)
                { return m_positiveTypeMatchBonus; }
                else if (ofTarget == Genre.Fantasy)
                { return m_neutralTypeMatchBonus; }
                else
                { return m_negativeTypeMatchBonus; }

            default:
                return 1f;
        }
    }
}
