using UnityEngine;
using System.Collections;

//TODO: Make Server Client rather than Peer-Peer

public class NetThinkState : NetworkState {

    private bool m_goToExecuteState = false;
    private bool m_hasCombatManager = false;
    private int m_playersReady = 0;
    private bool m_isClientReady = false;

    // Use this for initialization
    void Start () {
        // Tell the combat manager to start a new turn
        SetCombatManager(FindObjectOfType<CombatManager>());
        if (CManager != null)
        {
            m_hasCombatManager = true;
            CManager.StartNewTurn();
        }
    }

    // Update is called once per frame
    void Update () {

        bool areAllMovesSubmitted = true;

        // Iterate through all of the pawns in the combat and submit their move to the combat manager if they have chosen one
        foreach (CombatPawn combatPawn in CManager.AllPawnsActive)
        {
            if (!combatPawn.HasSubmittedMove)
            {
                combatPawn.OnThink();
                CombatMove pawnMove = combatPawn.MoveForTurn;
                if (pawnMove != null)
                {
                    CManager.SubmitMove(combatPawn, pawnMove);
                }
                areAllMovesSubmitted = false;
            }
            else
            {
                CombatMove pawnMove = combatPawn.MoveForTurn;
                CManager.SubmitMove(combatPawn, pawnMove);
            }
        }

        // If all moves are submitted, then this client is ready so increment the players ready value
        if (areAllMovesSubmitted && !m_isClientReady)
        {
            Debug.Log("This client is ready");
            GetComponent<PhotonView>().RPC("IncrementPlayersReady", PhotonTargets.All);
            m_isClientReady = true;
        }

        // Go to the execute state if all players are ready
        if (m_playersReady >= PhotonNetwork.playerList.Length)
        {
            m_goToExecuteState = true;
        }
    }

    /// <summary>
    /// True if all players are ready to move to the execute state
    /// </summary>
    public bool GoToExecuteState
    {
        get { return m_goToExecuteState; }
    }

    /// <summary>
    /// Increments the number of players ready on all clients
    /// </summary>
    [PunRPC]
    private void IncrementPlayersReady()
    {
        m_playersReady += 1;
    }
}
