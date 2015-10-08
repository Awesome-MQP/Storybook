using UnityEngine;
using System.Collections;

public class StatsManager : MonoBehaviour {

    private float m_positiveWeaponBonus = 1.2f;
    private float m_neutralWeaponBonus = 1f;
    private float m_negativeWeaponBonus = 0.8f;
    private float m_positiveSTABBonus = 1.6f;
    private float m_neutralSTABBonus = 1f;
    private float m_negativeSTABBonus = 0.6f;
    private float m_positiveTypeMatchBonus = 1.5f;
    private float m_neutralTypeMatchBonus = 1f;
    private float m_negativeTypeMatchBonus = 0.5f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    /// <summary>
    /// Does damage calculation based on the parameters sent in.
    /// </summary>
    /// <param name="attackerGenre">The Genre of the attacker; used to determine STAB.</param>
    /// <param name="defenderGenre">The Genre of the defender; used to determine Type Advantage.</param>
    /// <param name="attackerAttackGenre">The Genre of the attacker's attack move; used to determine STAB and Type Advantage.</param>
    /// <param name="attackerWeaponGenre">The Genre of the attacker's weapon; used to determine Weapon Compatibility.</param>
    /// <param name="attackerBaseStrength">The Str stat of the attacker.</param>
    /// <param name="attackerWeaponStrength">The Str modifier of the attacker's weapon.</param>
    /// <param name="defenderDefense">The Def stat of the target.</param>
    /// <returns></returns>
    int DoDamage(Genre attackerGenre, Genre defenderGenre, Genre attackerAttackGenre, Genre attackerWeaponGenre, 
                  float attackerBaseStrength, float attackerWeaponStrength, float defenderDefense)
    {
        float sameTypeAttackBonus = GetSameTypeBonus(attackerGenre, attackerAttackGenre);
        float weaponCompatibleBonus = GetWeaponCompatibilityBonus(attackerGenre, attackerWeaponGenre);
        float typeAdvantageBonus = GetTypeAdvantageBonus(attackerAttackGenre, defenderGenre);

        float totalDmg = ((attackerWeaponStrength + attackerBaseStrength) * weaponCompatibleBonus * sameTypeAttackBonus * typeAdvantageBonus)
            - (defenderDefense + 1);

        if (totalDmg < 0)
        { return 0; }
        else
        {
            return Mathf.FloorToInt(totalDmg);
        }
    }

    /// <summary>
    /// Given a player's Genre and the Genre of their attack, determines what Same Type Attack Bonus the player will get.
    /// </summary>
    /// <param name="ofPlayer">The Genre of the player.</param>
    /// <param name="ofAttack">The Genre of the weapon held by that player</param>
    /// <returns>Some bonus/penalty based on the inputs.</returns>
    float GetSameTypeBonus(Genre ofPlayer, Genre ofAttack)
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
    /// Generates a bonus or penalty depending on the player's genre and that of their weapon.
    /// This bonus is not as great as the STAB, but can stack with it.
    /// </summary>
    /// <param name="ofPlayer">Genre of the player.</param>
    /// <param name="ofWeapon">Genre of the weapon.</param>
    /// <returns>Bonus or penalty based on the two input genres.</returns>
    float GetWeaponCompatibilityBonus(Genre ofPlayer, Genre ofWeapon)
    {
        switch (ofPlayer)
        {
            // Each case returns a Weapon Compatibility bonus based on how the weapon's type compares to the user's.
            // Same type results in a 1.3x mult, the type that either counters or is countered by the player's type results
            // in a 0.7x mult, and the last "neutral" type results in a 1x mult (no change).
            case Genre.Fantasy:
                if (ofWeapon == Genre.Fantasy)
                { return m_positiveWeaponBonus; }
                else if (ofWeapon == Genre.GraphicNovel)
                { return m_neutralWeaponBonus; }
                else
                { return m_negativeWeaponBonus; }

            case Genre.GraphicNovel:
                if (ofWeapon == Genre.GraphicNovel)
                { return m_positiveWeaponBonus; }
                else if (ofWeapon == Genre.Fantasy)
                { return m_neutralWeaponBonus; }
                else
                { return m_negativeWeaponBonus; }

            case Genre.Horror:
                if (ofWeapon == Genre.Horror)
                { return m_positiveWeaponBonus; }
                else if (ofWeapon == Genre.SciFi)
                { return m_neutralWeaponBonus; }
                else
                { return m_negativeWeaponBonus; }

            case Genre.SciFi:
                if (ofWeapon == Genre.SciFi)
                { return m_positiveWeaponBonus; }
                else if (ofWeapon == Genre.Horror)
                { return m_neutralWeaponBonus; }
                else
                { return m_negativeWeaponBonus; }

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
    float GetTypeAdvantageBonus(Genre ofPlayerAttack, Genre ofTarget)
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
