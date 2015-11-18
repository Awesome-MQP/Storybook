using UnityEngine;
using System.Collections.Generic;
using System;

public class EnemyTeam : CombatTeam {

    public override void SpawnTeam()
    {
        GetComponent<PhotonView>().RPC("RegisterTeamLocal", PhotonTargets.Others, TeamId);
        int i = 0;
        List<EnemyPositionNode> positionNodes = new List<EnemyPositionNode>(FindObjectsOfType<EnemyPositionNode>()); 
        foreach (CombatPawn pawn in PawnsToSpawn)
        {
            GameObject enemyObject = PhotonNetwork.Instantiate(PawnsToSpawn[i].name, positionNodes[i].transform.position, Quaternion.identity, 0);
            PhotonNetwork.Spawn(enemyObject.GetComponent<PhotonView>());
            CombatPawn enemyPawn = enemyObject.GetComponent<CombatPawn>();
            enemyPawn.PawnId = i + 1;
            enemyPawn.TeamId = TeamId;
            enemyPawn.RegisterTeam(this);
            AddPawnToSpawned(enemyPawn);
            AddPawnToTeam(enemyPawn);
            enemyPawn.AddPawnToTeamLocal();
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
