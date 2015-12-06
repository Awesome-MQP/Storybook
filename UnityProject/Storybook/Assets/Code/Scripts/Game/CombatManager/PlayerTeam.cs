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
            GameObject playerObject = PhotonNetwork.Instantiate(PawnsToSpawn[i].name, positionNodes[i].transform.position, Quaternion.identity, 0);
            if ((i + 1) != PhotonNetwork.player.ID)
            {
                Debug.Log("Transferring control");
                playerObject.GetComponent<PhotonView>().TransferController(i + 1);
            }
            PhotonNetwork.Spawn(playerObject.GetComponent<PhotonView>());
            CombatPawn playerPawn = playerObject.GetComponent<CombatPawn>();
            playerPawn.PawnId = i + 1;

            playerPawn.TeamId = TeamId;
            AddPawnToSpawned(playerPawn);
            AddPawnToTeam(playerPawn);
            playerPawn.RegisterTeam(this);
            playerPawn.SendPawnTeam();

            if (playerPawn is CombatPlayer)
            {
                CombatDeck pawnDeck = _initializePlayerDeck();
                CombatPlayer player = (CombatPlayer)playerPawn;
                int[] pageViewIds = pawnDeck.GetPageViewIds();
                player.PlayerDeck = pawnDeck;
                player.DrawStartingHand();
                player.SendDeckPageViewIds(pageViewIds);
            }
            
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
        foreach(CombatPawn pawn in ActivePawnsOnTeam)
        {
            pawn.DecrementBoosts();
            
            if (pawn is CombatPlayer)
            {
                CombatPlayer playerPawn = (CombatPlayer)pawn;
                playerPawn.DrawPageForTurn();
            }
        }
    }

    //TODO: Get the player inventory from the given PlayerEntity
    private CombatDeck _initializePlayerDeck(/*PlayerEntity playerToCreateFor*/)
    {
        Debug.Log("Initializing player deck");
        List<Page> deckPages = new List<Page>();
        for (int i = 0; i < 20; i++)
        {
            GameObject pageObject = PhotonNetwork.Instantiate(m_pageToUse.name, Vector3.zero, Quaternion.identity, 0);
            PhotonNetwork.Spawn(pageObject.GetComponent<PhotonView>());
            Page page = pageObject.GetComponent<Page>();
            int pageViewId = pageObject.GetComponent<PhotonView>().viewID;
            deckPages.Add(page);
        }
        CombatDeck playerDeck = new CombatDeck(deckPages);
        playerDeck.ShuffleDeck();
        return playerDeck;
    }
}
