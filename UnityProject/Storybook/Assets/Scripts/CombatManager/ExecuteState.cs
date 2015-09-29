using UnityEngine;
using System.Collections;

public class ExecuteState : CombatState {

    private bool m_isTurnComplete = false;
    private CombatPawn m_currentCombatPawn;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        // Execute state can only be reached from think state, so need to reset the ExecuteToThink trigger
        StateMachine.ResetTrigger("ExecuteToThink");

        // Initialize the turn complete boolean to false
        m_isTurnComplete = false;

        // Set the current combat pawn to the fastest pawn in the combat
        GetNextCombatPawn();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        
        if (!m_isTurnComplete && CManager.CurrentState == this)
        {
            // If the move for the current combat pawn is not complete, call ExecuteMove on the current pawn's move
            if (!m_currentCombatPawn.MoveForTurn.IsMoveComplete)
            {
                m_currentCombatPawn.MoveForTurn.ExecuteMove();
            }

            // If the move for the current pawn is complete, set the action complete boolean to true and get the next pawn
            else
            {
                m_currentCombatPawn.SetIsActionComplete(true);
                GetNextCombatPawn();
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    public override void ExitState()
    {
        m_isTurnComplete = true;

        // If all the players have been defeated, exit this state and enter the lose state
        if (_areAllPlayersDefeated())
        {
            StateMachine.SetTrigger("ExecuteToLose");
            CManager.SetCurrentState(StateMachine.GetBehaviour<LoseState>());
        }

        // If all of the enemies are defeated, exit this state and enter the win state
        else if (_areAllEnemiesDefeated())
        {
            StateMachine.SetTrigger("ExecuteToWin");
            CManager.SetCurrentState(StateMachine.GetBehaviour<WinState>());
        }

        // Otherwise the combat is still active, so return to the think state
        else
        {
            StateMachine.SetTrigger("ExecuteToThink");
            CManager.SetCurrentState(StateMachine.GetBehaviour<ThinkState>());
        }
    }

    /// <summary>
    /// Checks to see if all of the players in the combat have been defeated
    /// </summary>
    /// <returns>True if all the players have been defeated, false otherwise</returns>
    private bool _areAllPlayersDefeated()
    {
        bool areAllPlayersDefeated = true;
        foreach (CombatPawn playerPawn in CManager.PlayerPawnList)
        {
            if (playerPawn.IsAlive)
            {
                areAllPlayersDefeated = false;
                break;
            }
        }
        return areAllPlayersDefeated;
    }

    /// <summary>
    /// Checks to see if all of the enemies in the combat have been defeated
    /// </summary>
    /// <returns>True if all the enemies have been defeated, false otherwise</returns>
    private bool _areAllEnemiesDefeated()
    {
        bool areAllEnemiesDefeated = true;
        foreach (CombatEnemy enemyPawn in CManager.EnemyList)
        {
            if (enemyPawn.IsAlive)
            {
                areAllEnemiesDefeated = false;
                break;
            }
        }
        return areAllEnemiesDefeated;
    }

    /// <summary>
    /// Checks to see if the combat is completed by checking if all the players have been defeated or all the enemies have been defeated
    /// </summary>
    /// <returns>True if the combat is complete, false otherwise</returns>
    private bool _isCombatComplete()
    {
        return (_areAllEnemiesDefeated() || _areAllPlayersDefeated()) ;
    }

    // TODO - How to handle ties with the speed values (players have priority?)
    /// <summary>
    /// Iterates through all of the CombatPawns in the combat, and checks to see which has the highest speed that
    /// has not taken its move yet. Sets the result as the current combat pawn to call OnAction
    /// </summary>
    public void GetNextCombatPawn()
    {

        // If the combat has been completed (one side completely defeated, then exit the state)
        if (_isCombatComplete())
        {
            ExitState();
        }

        int currentHighestSpeed = 0;
        CombatPawn fastestCombatPawn = null;

        // Iterate through all of the CombatPawn in the CombatManager
        foreach (CombatPawn combatPawn in CManager.AllPawns)
        {
            int currentSpeed = combatPawn.Speed;

            // If the speed of the current pawn is higher than the current highest speed and the current pawn has not executed
            // its move for the turn, set the fasted combat pawn to this combat pawn
            if (currentSpeed > currentHighestSpeed && !combatPawn.IsActionComplete)
            {
                currentHighestSpeed = currentSpeed;
                fastestCombatPawn = combatPawn;
            }
        }

        // If the fastest combat pawn is not null, set the current combat pawn to this
        if (fastestCombatPawn != null)
        {
            m_currentCombatPawn = fastestCombatPawn;
        }

        // Otherwise, all the combat pawns have done their move, so exit the execute state
        else
        {
            ExitState();
        }
    }
}
