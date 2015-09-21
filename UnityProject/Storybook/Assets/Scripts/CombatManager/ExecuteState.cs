using UnityEngine;
using System.Collections;

public class ExecuteState : CombatState {

    private int m_currentPawnIndex = 0;
    private bool m_arePlayerMovesComplete = false;

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (!m_arePlayerMovesComplete)
        {
            CombatPawn currentPawn = CManager.PawnList[m_currentPawnIndex];
            currentPawn.OnAction();
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
        m_currentPawnIndex = 0;
        StateMachine.SetTrigger("ExecuteToWin");
    }

    // Do the action for the next pawn that is in the combat
    // If there are no more pawns left to move, move the enemies
    public void IncrementPawnIndex()
    {
        if (m_currentPawnIndex + 1 < CManager.PawnList.Count)
        {
            m_currentPawnIndex += 1;
        }
        else
        {
            m_arePlayerMovesComplete = true;
            ExitState();
        }
    }
}
