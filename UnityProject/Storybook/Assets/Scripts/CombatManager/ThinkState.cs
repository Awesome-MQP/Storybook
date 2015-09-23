using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Experimental.Director;

public class ThinkState : CombatState {

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        CManager.StartNewTurn();
        StateMachine.ResetTrigger("ThinkToExecute");
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        // If all of the players have submitted their moves, exit the think state and move to execute
        if (CManager.MovesSubmitted == CManager.PlayerPawnList.Count && CManager.MovesSubmitted > 0)
        {
            ExitState();
        }
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	    
	}

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
        CManager.CurrentState = StateMachine.GetBehaviour<ExecuteState>();
    }
}
