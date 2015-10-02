using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Experimental.Director;

public class ThinkState : CombatState {

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        // Tell the combat manager to start a new turn
        CManager.StartNewTurn();

        // Reset the ThinkToExecute trigger so that it does not immediately return to the execute state
        StateMachine.ResetTrigger("ThinkToExecute");
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
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
            ExitState();
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
        StateMachine.SetTrigger("ThinkToExecute");
        CManager.SetCurrentState(StateMachine.GetBehaviour<ExecuteState>());
    }
}
