using UnityEngine;
using System.Collections.Generic;
using System;

public class PlayerTeam : CombatTeam {

    public override void SpawnTeam()
    {
        int i = 0;
        List<PlayerPositionNode> positionNodes = new List<PlayerPositionNode>(FindObjectsOfType<PlayerPositionNode>());
        foreach (CombatPawn pawn in PawnsToSpawn)
        {
            GameObject playerObject = PhotonNetwork.Instantiate(PawnsToSpawn[i].name, positionNodes[i].transform.position, Quaternion.identity, 0);
            PhotonNetwork.Spawn(playerObject.GetComponent<PhotonView>());
            CombatPawn playerPawn = playerObject.GetComponent<CombatPawn>();
            playerPawn.SetPawnId(i + 1);
            AddPawnToSpawned(playerPawn);
            RemovePawnFromTeam(pawn);
            AddPawnToTeam(playerPawn);
            playerPawn.RegisterTeam(this);
            i++;
        }
    }

    public override void RemovePawnFromTeam(CombatPawn pawnToRemove)
    {
        base.RemovePawnFromTeam(pawnToRemove);
    }

    public override void StartCombat()
    {
        Debug.Log("Player team starting combat");
    }

    public override void StartNewTurn()
    {
        foreach(CombatPawn pawn in PawnsOnTeam)
        {
            pawn.DecrementBoosts();
        }
    }
}
