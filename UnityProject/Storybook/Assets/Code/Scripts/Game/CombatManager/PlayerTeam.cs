using UnityEngine;
using System.Collections.Generic;
using System;

public class PlayerTeam : CombatTeam {

    [SerializeField]
    Page m_pageToUse;

    public override void SpawnTeam()
    {
        GetComponent<PhotonView>().RPC("RegisterTeamLocal", PhotonTargets.Others, TeamId);
        int i = 0;
        List<PlayerPositionNode> positionNodes = new List<PlayerPositionNode>(FindObjectsOfType<PlayerPositionNode>());
        foreach (CombatPawn pawn in PawnsToSpawn)
        {
            Quaternion rotation = Quaternion.identity;
            rotation.y = 120;
            rotation.w = 120;
            GameObject playerObject = PhotonNetwork.Instantiate(PawnsToSpawn[i].name, positionNodes[i].transform.position, rotation, 0);
            if ((i + 1) != PhotonNetwork.player.ID)
            {
                playerObject.GetComponent<PhotonView>().TransferController(i + 1);
            }
            PhotonNetwork.Spawn(playerObject.GetComponent<PhotonView>());
            CombatPawn playerPawn = playerObject.GetComponent<CombatPawn>();
            playerPawn.PawnId = i + 1;

            playerPawn.TeamId = TeamId;

            if (playerPawn is CombatPlayer)
            {
                CombatPlayer player = (CombatPlayer)playerPawn;
                GameManager gameManager = FindObjectOfType<GameManager>();
                PlayerEntity currentPlayerEntity = _findPlayerEntity(playerPawn.PawnId);
                player.InitializePlayerPawn(currentPlayerEntity);
                PlayerInventory currentPlayerInventory = gameManager.GetLocalPlayerInventory();
                player.CreateDeck(currentPlayerInventory);
            }

            AddPawnToSpawned(playerPawn);
            AddPawnToTeam(playerPawn);
            playerPawn.RegisterTeam(this);
            playerPawn.SendPawnTeam(TeamId);
            i++;
        }
    }

    private PlayerEntity _findPlayerEntity(int photonPlayerId)
    {
        PlayerEntity[] allPlayerEntity = FindObjectsOfType<PlayerEntity>();
        foreach(PlayerEntity pe in allPlayerEntity)
        {
            if (pe.Player.ID == photonPlayerId)
            {
                return pe;
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
        Debug.Log("Player team starting combat");
    }

    /// <summary>
    /// When combat ends, update the HP for all of the player entities based on the ending HP of their corresponding combat pawns
    /// </summary>
    public override void EndCombat()
    {
        foreach (CombatPawn pawn in AllPawnsSpawned)
        {
            PlayerEntity currentPlayerEntity = _findPlayerEntity(pawn.PawnId);
            if (pawn.Health > 0)
            {
                currentPlayerEntity.UpdateHitPoints((int)pawn.Health);
            }
            else
            {
                currentPlayerEntity.UpdateHitPoints(1);
            }
        }
        base.EndCombat();
    }

    public override void StartNewTurn()
    {
        foreach(CombatPawn pawn in ActivePawnsOnTeam)
        {
            pawn.DecrementBoosts();
            
            if (pawn is CombatPlayer)
            {
                CombatPlayer playerPawn = (CombatPlayer)pawn;
                playerPawn.DrawPageForTurn();
                if (playerPawn.PawnId == PhotonNetwork.player.ID)
                {
                    playerPawn.PrintPlayerHand();
                }
            }
        }
    }

    private PlayerInventory _findPlayerInventory(int pawnId)
    {
        PlayerInventory[] allInventories = FindObjectsOfType<PlayerInventory>();
        foreach(PlayerInventory inventory in allInventories)
        {
            if (inventory.PlayerId == pawnId)
            {
                return inventory;
            }
        }
        return null;
    }
}
