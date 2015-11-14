using UnityEngine;
using System.Collections;

public class LoseState : CombatState {

    private GameObject m_netLoseStateObject = null;
    private NetLoseState m_netLoseState;
    private bool m_isExiting = false;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        m_isExiting = false;

        // If it is the master client, instantiate the NetLoseState to all players
        if (PhotonNetwork.isMasterClient)
        {
            m_netLoseStateObject = PhotonNetwork.Instantiate("NetLoseState", Vector3.zero, Quaternion.identity, 0);
            m_netLoseState = m_netLoseStateObject.GetComponent<NetLoseState>();
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (PhotonNetwork.isMasterClient)
        {
            if (m_netLoseState.ExitCombat && !m_isExiting)
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
        m_netLoseState.DeleteCombat();

        // Destroy the NetLoseState when exiting and end the combat
        if (m_netLoseStateObject != null)
        {
            PhotonNetwork.Destroy(m_netLoseStateObject);
        }
        m_isExiting = true;
        ResetBools();
        StateMachine.SetBool("ExitCombat", true);
    }
}
