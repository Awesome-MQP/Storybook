using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class PlayerMove : CombatMove {

    /// <summary>
    /// Chooses random targets for the player move from the given list of possible targets
    /// Called when a player move has no targets after being submitted due to targets being defeated in combat
    /// </summary>
    /// <param name="possibleTargets">The list of possible targets for the player move</param>
    public void ChooseRandomTargets(List<CombatPawn> possibleTargets)
    {
        SetMoveTargets(new List<CombatPawn>());
        List<CombatPawn> newMoveTargets = new List<CombatPawn>();
        while (newMoveTargets.Count < NumberOfTargets && possibleTargets.Count > 0)
        {
            System.Random random = new System.Random();
            int targetIndex = random.Next(0, possibleTargets.Count - 1);
            CombatPawn target = possibleTargets[targetIndex];
            newMoveTargets.Add(target);
            possibleTargets.Remove(target);
        }
        SetMoveTargets(newMoveTargets);
    }

}
