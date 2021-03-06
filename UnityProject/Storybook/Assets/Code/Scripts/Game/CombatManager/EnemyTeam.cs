﻿using UnityEngine;
using System.Collections.Generic;
using System;

public class EnemyTeam : CombatTeam {

    [SerializeField]
    private bool m_isBossTeam = false;

    public override void SpawnTeam()
    {
        DungeonMaster dm = FindObjectOfType<DungeonMaster>();
        GetComponent<PhotonView>().RPC("RegisterTeamLocal", PhotonTargets.Others, TeamId);
        int i = 0;
        List<EnemyPositionNode> positionNodes = new List<EnemyPositionNode>(FindObjectsOfType<EnemyPositionNode>()); 
        foreach (CombatPawn pawn in PawnsToSpawn)
        {
            EnemyPositionNode nodeToUse = _getPositionNodeById(positionNodes, i + 1);

            GameObject enemyObject = null;

            if (!m_isBossTeam)
            {
                enemyObject = PhotonNetwork.Instantiate("Enemies/EnemyTypes/" + PawnsToSpawn[i].PawnGenre + "/" + PawnsToSpawn[i].name, nodeToUse.transform.position, nodeToUse.transform.rotation, 0);
            }
            else
            {
                enemyObject = PhotonNetwork.Instantiate("Enemies/EnemyTypes/" + PawnsToSpawn[i].PawnGenre + "/Bosses/" + PawnsToSpawn[i].name, nodeToUse.transform.position, nodeToUse.transform.rotation, 0);
            }

            CombatPawn enemyPawn = enemyObject.GetComponent<CombatPawn>();
            enemyPawn.transform.SetParent(nodeToUse.transform);
            PhotonNetwork.Spawn(enemyObject.GetComponent<PhotonView>());
            dm.ScalePawnByLevel(enemyPawn, TeamLevel);
            enemyPawn.PawnId = i + 1;
            enemyPawn.TeamId = TeamId;
            enemyPawn.RegisterTeam(this);
            AddPawnToSpawned(enemyPawn);
            AddPawnToTeam(enemyPawn);
            enemyPawn.SendPawnTeam(TeamId);
            i++;
        }
    }

    private EnemyPositionNode _getPositionNodeById(List<EnemyPositionNode> enemyPositions, int positionId)
    {
        foreach(EnemyPositionNode ep in enemyPositions)
        {
            if (ep.PositionId == positionId)
            {
                return ep;
            }
        }
        return null;
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
        foreach(CombatPawn pawn in ActivePawnsOnTeam)
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
