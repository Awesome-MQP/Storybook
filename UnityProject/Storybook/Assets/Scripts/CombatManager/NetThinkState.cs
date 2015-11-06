using UnityEngine;
using System.Collections;

public class NetThinkState : NetworkState {

    private bool m_goToExecuteState = false;
    private bool m_hasCombatManager = false;

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
        foreach (CombatPawn combatPawn in CManager.AllPawns)
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

        // If all of the players have submitted their moves, exit the think state and move to execute
        if (areAllMovesSubmitted)
        {
            m_goToExecuteState = true;
        }
    }

    public bool GoToExecuteState
    {
        get { return m_goToExecuteState; }
    }
}
