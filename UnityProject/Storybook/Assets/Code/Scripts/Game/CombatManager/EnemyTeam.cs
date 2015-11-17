using UnityEngine;
using System.Collections;
using System;

public class EnemyTeam : CombatTeam {

    public override void SpawnTeam()
    {
        int i = 0;
        foreach (CombatPawn pawn in PawnsOnTeam)
        {
            GameObject enemyObject = PhotonNetwork.Instantiate(PawnsOnTeam[i].name, PawnPositions[i], Quaternion.identity, 0);
            PhotonNetwork.Spawn(enemyObject.GetComponent<PhotonView>());
            AddPawnToSpawned(enemyObject.GetComponent<CombatPawn>());
            i++;
        }
    }

    public override void RemovePawnFromTeam(CombatPawn pawnToRemove)
    {
        base.RemovePawnFromTeam(pawnToRemove);
    }

    public override void StartCombat()
    {
        Debug.Log("Enemy team starting combat");
    }

    public override void StartNewTurn()
    {
        foreach(CombatPawn pawn in PawnsOnTeam)
        {
            pawn.DecrementBoosts();
            if (pawn is CombatAI)
            {
                CombatAI ai = (CombatAI)pawn;
                ai.IncrementManaForTurn();
            }
        }
    }
}
