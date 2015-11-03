using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExecuteState : CombatState {

    private bool m_isTurnComplete = false;
    private CombatPawn m_currentCombatPawn;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        Debug.Log("Entering Execute State");

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
            CombatMove currentMove = CManager.PawnToMove[m_currentCombatPawn];
            // If the move for the current combat pawn is not complete, call ExecuteMove on the current pawn's move
            if (!currentMove.IsMoveComplete)
            {
                currentMove.ExecuteMove();
            }

            // If the move for the current pawn is complete, set the action complete boolean to true and get the next pawn
            else
            {
                m_currentCombatPawn.MoveForTurn.SetIsMoveComplete(false);
                currentMove.SetIsMoveComplete(false);
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

        // Handle any pawns that have been defeated by the previous move's effect
        _checkForDefeatedPawns();

        // TODO - New targets for moves that include defeated players/enemies

        float currentHighestSpeed = 0;
        CombatPawn fastestCombatPawn = null;
        // Iterate through all of the CombatPawn in the CombatManager
        foreach (CombatPawn combatPawn in CManager.PawnToMove.Keys)
        {
            float currentSpeed = combatPawn.Speed;

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

            // Need to initialize the move in case the same move has been used this turn since it resets the booleans
            CombatMove pawnMove = CManager.PawnToMove[m_currentCombatPawn];
            pawnMove.InitializeMove();
        }

        // Otherwise, all the combat pawns have done their move, so exit the execute state
        else
        {
            ExitState();
        }
    }

    /// <summary>
    /// Checks to see if any of the player's have been defeated
    /// Called after each move
    /// </summary>
    /// <returns>The list of players that were defeated by the previous move</returns>
    private List<CombatPawn> _checkForDefeatedPlayers()
    {
        List<CombatPawn> removedList = new List<CombatPawn>();
        foreach (CombatPawn pawn in CManager.PlayerPawnList)
        {
            if (!pawn.IsAlive)
            {
                removedList.Add(pawn);
            }
        }
        foreach (CombatPawn pawn in removedList)
        {
            CManager.RemovePlayerFromCombat(pawn);
        }
        return removedList;
    }

    /// <summary>
    /// Checks to see if any of the enemies have been defeated 
    /// Called after each move is executed
    /// </summary>
    /// <returns>The list of enemies that have been defeated this turn</returns>
    private List<CombatPawn> _checkForDefeatedEnemies()
    {
        List<CombatEnemy> removedList = new List<CombatEnemy>();
        foreach (CombatEnemy enemy in CManager.EnemyList)
        {
            if (!enemy.IsAlive)
            {
                removedList.Add(enemy);
            }
        }
        foreach (CombatEnemy enemy in removedList)
        {
            CManager.RemoveEnemyFromCombat(enemy);
        }
        return new List<CombatPawn>(removedList.ToArray());
    }

    /// <summary>
    /// Checks to see if any enemies or players have been defeated
    /// Updates move targets if a move contains a unit that has been defeated
    /// Called after each move is executed
    /// </summary>
    private void _checkForDefeatedPawns()
    {
        List<CombatPawn> defeatedEnemies = _checkForDefeatedEnemies();
        List<CombatPawn> defeatedPlayers = _checkForDefeatedPlayers();

        // Update the move targets for all of the enemies if their moves contain a defeated unit
        foreach (CombatEnemy ce in CManager.EnemyList)
        {
            // If the pawn's action is already complete, move to the next one
            if (ce.IsActionComplete)
            {
                continue;
            }
            EnemyMove enemyMove = (EnemyMove)CManager.PawnToMove[ce];
            foreach (CombatPawn enemyTarget in enemyMove.MoveTargets)
            {
                if (defeatedEnemies.Contains(enemyTarget))
                {
                    HashSet<CombatPawn> enemyListSet = new HashSet<CombatPawn>(CManager.EnemyList);
                    enemyMove.ChooseTargets(enemyListSet);
                }
                if (defeatedPlayers.Contains(enemyTarget))
                {
                    HashSet<CombatPawn> playerListSet = new HashSet<CombatPawn>(CManager.PlayerPawnList);
                    enemyMove.ChooseTargets(playerListSet);
                }
            }
        }

        // Select other move targets for all of the players if their moves contain a defeated unit
        foreach (CombatPawn playerPawn in CManager.PlayerPawnList)
        {
            // If the pawn's action is already complete, move to the next one
            if (playerPawn.IsActionComplete)
            {
                continue;
            }
            PlayerMove playerMove = (PlayerMove)CManager.PawnToMove[playerPawn];
            foreach (CombatPawn playerTarget in playerMove.MoveTargets)
            {
                if (defeatedEnemies.Contains(playerTarget))
                {
                    if (playerMove.MoveTargets.Length > 1)
                    {
                        playerMove.RemoveTarget(playerTarget);
                    }
                    else
                    {
                        List<CombatPawn> enemyList = new List<CombatPawn>(CManager.EnemyList);
                        playerMove.ChooseRandomTargets(enemyList);
                    }
                }
                if (defeatedPlayers.Contains(playerTarget))
                {
                    if (playerMove.MoveTargets.Length > 1)
                    {
                        playerMove.RemoveTarget(playerTarget);
                    }
                    else
                    {
                        List<CombatPawn> playerList = new List<CombatPawn>(CManager.PlayerPawnList);
                        playerMove.ChooseRandomTargets(playerList);
                    }
                }
            }
        }
    }
}
