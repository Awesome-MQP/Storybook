using UnityEngine;
using System.Collections.Generic;
using System;

public class PlayerTeam : CombatTeam
{

    [SerializeField]
    Page m_pageToUse;

    [SerializeField]
    private ResourceAsset m_comicCombatPawn = new ResourceAsset(typeof(CombatPlayer));

    [SerializeField]
    private ResourceAsset m_horrorCombatPawn = new ResourceAsset(typeof(CombatPlayer));

    [SerializeField]
    private ResourceAsset m_fantasyCombatPawn = new ResourceAsset(typeof(CombatPlayer));

    [SerializeField]
    private ResourceAsset m_scifiCombatPawn = new ResourceAsset(typeof(CombatPlayer));

    public override void SpawnTeam()
    {
        GetComponent<PhotonView>().RPC("RegisterTeamLocal", PhotonTargets.Others, TeamId);
        int i = 0;
        List<PlayerPositionNode> positionNodes = new List<PlayerPositionNode>(FindObjectsOfType<PlayerPositionNode>());

        foreach (PlayerEntity pe in GameManager.GetInstance<GameManager>().IteratePlayers<PlayerEntity>())
        {
            ResourceAsset prefab = _getCombatPawnResourceForGenre(pe.Genre);

            //TODO: change to use node rotation
            PlayerPositionNode playerNode = _getPositionNodeById(positionNodes, i + 1);
            Quaternion rotation = Quaternion.identity;
            rotation.y = 120;
            rotation.w = 120;
            GameObject playerObject = PhotonNetwork.Instantiate(prefab, playerNode.transform.position, rotation, 0);
            if (!pe.Player.isLocal)
            {
                playerObject.GetComponent<PhotonView>().TransferController(pe.Player);
            }
            CombatPawn playerPawn = playerObject.GetComponent<CombatPawn>();
            playerPawn.transform.SetParent(playerNode.transform);
            playerPawn.PawnId = i + 1;

            playerPawn.TeamId = TeamId;

            PhotonNetwork.Spawn(playerObject.GetComponent<PhotonView>());

            if (playerPawn is CombatPlayer)
            {
                CombatPlayer player = (CombatPlayer)playerPawn;
                GameManager gameManager = GameManager.GetInstance<GameManager>();
                PlayerEntity currentPlayerEntity = _findPlayerEntity(playerPawn.PawnId);
                player.InitializePlayerPawn(currentPlayerEntity);
                PlayerInventory currentPlayerInventory = gameManager.GetLocalPlayer<PlayerEntity>().OurInventory;
                player.CreateDeck(currentPlayerInventory);
            }

            AddPawnToSpawned(playerPawn);
            AddPawnToTeam(playerPawn);
            playerPawn.RegisterTeam(this);
            playerPawn.SendPawnTeam(TeamId);
            i++;
        }
    }

    private PlayerPositionNode _getPositionNodeById(List<PlayerPositionNode> enemyPositions, int positionId)
    {
        foreach (PlayerPositionNode playerNode in enemyPositions)
        {
            Debug.Log("Node Id = " + playerNode.PositionId);
            if (playerNode.PositionId == positionId)
            {
                return playerNode;
            }
        }
        return null;
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

    private ResourceAsset _getCombatPawnResourceForGenre(Genre genre)
    {
        switch (genre)
        {
            case Genre.Horror:
                return m_horrorCombatPawn;
            case Genre.SciFi:
                return m_scifiCombatPawn;
            case Genre.Fantasy:
                return m_fantasyCombatPawn;
            case Genre.GraphicNovel:
                return m_comicCombatPawn;
            default:
                throw new ArgumentOutOfRangeException(nameof(genre), genre, null);
        }
    }
}
