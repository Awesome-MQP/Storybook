using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Experimental.Director;

public class ThinkState : CombatState {

    private int m_trigger = 0;
    private GameObject m_netThinkStateObject = null;
    private NetThinkState m_netThinkState;
    private bool m_isExiting = false;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Entering think state");

        m_isExiting = false;

        base.OnStateEnter(animator, stateInfo, layerIndex);

        StateMachine.SetBool("ExecuteToThink", false);

        if (PhotonNetwork.isMasterClient)
        {
            m_netThinkStateObject = PhotonNetwork.Instantiate("NetThinkState", Vector3.zero, Quaternion.identity, 0);
            m_netThinkState = m_netThinkStateObject.GetComponent<NetThinkState>();
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
 
        if (PhotonNetwork.isMasterClient && !m_isExiting)
        {
            if (m_netThinkState.GoToExecuteState)
            {
                ExitState();
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
        m_isExiting = true;
        if (m_netThinkStateObject != null)
        {
            PhotonNetwork.Destroy(m_netThinkStateObject);
        }
        m_netThinkStateObject = null;
        m_netThinkState = null;
        Debug.Log("Exiting Think State");
        StateMachine.SetBool("ThinkToExecute", true);
        //CManager.SetCurrentState(StateMachine.GetBehaviour<ExecuteState>());
    }
}
