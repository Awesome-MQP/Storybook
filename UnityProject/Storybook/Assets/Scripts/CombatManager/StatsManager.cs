using UnityEngine;
using System.Collections;

public class StatsManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    /// <summary>
    /// Given a player's Genre and the Genre of their weapon, determines what Same Type Attack Bonus the player will get.
    /// </summary>
    /// <param name="ofPlayer">The Genre of the player.</param>
    /// <param name="ofWeapon">The Genre of the weapon held by that player</param>
    /// <returns>Some bonus/penalty based on the inputs.</returns>
    float GetSameTypeBonus(Genre ofPlayer, Genre ofWeapon)
    {
        switch (ofPlayer)
        {
            // Each case returns a STAB bonus based on how the weapon's type compares to the user's.
            // Same type results in a 2x mult, the type that either counters or is countered by the player's type results
            // in a 0.5x mult, and the last "neutral" type results in a 1x mult (no change).
            case Genre.Fantasy:
                if(ofWeapon == Genre.Fantasy)
                { return 2f; }
                else if(ofWeapon == Genre.GraphicNovel)
                { return 1f; }
                else
                { return 0.5f; }

            case Genre.GraphicNovel:
                if(ofWeapon == Genre.GraphicNovel)
                { return 2f; }
                else if(ofWeapon == Genre.Fantasy)
                { return 1f; }
                else
                { return 0.5f; }

            case Genre.Horror:
                if (ofWeapon == Genre.Horror)
                { return 2f; }
                else if (ofWeapon == Genre.SciFi)
                { return 1f; }
                else
                { return 0.5f; }

            case Genre.SciFi:
                if (ofWeapon == Genre.SciFi)
                { return 2f; }
                else if (ofWeapon == Genre.Horror)
                { return 1f; }
                else
                { return 0.5f; }

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
    float GetTypeAdvantageBonus(Genre ofPlayerWeapon, Genre ofTarget)
    {
        switch (ofPlayerWeapon)
        {
            // Each case returns a STAB bonus based on how the weapon's type compares to the user's.
            // Same type results in a 2x mult, the type that either counters or is countered by the player's type results
            // in a 0.5x mult, and the last "neutral" type results in a 1x mult (no change).
            case Genre.Fantasy:
                if (ofTarget == Genre.SciFi)
                { return 2f; }
                else if (ofTarget == Genre.Horror)
                { return 0.5f; }
                else
                { return 1f; }

            case Genre.GraphicNovel:
                if (ofTarget == Genre.Horror)
                { return 2f; }
                else if (ofTarget == Genre.SciFi)
                { return 0.5f; }
                else
                { return 1f; }

            case Genre.Horror:
                if (ofTarget == Genre.Fantasy)
                { return 2f; }
                else if (ofTarget == Genre.GraphicNovel)
                { return 0.5f; }
                else
                { return 1f; }

            case Genre.SciFi:
                if (ofTarget == Genre.GraphicNovel)
                { return 2f; }
                else if (ofTarget == Genre.Fantasy)
                { return 0.5f; }
                else
                { return 1f; }

            default:
                return 1f;
        }
    }
}
