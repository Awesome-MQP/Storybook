using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExecuteState : CombatState
{

    private bool m_isTurnComplete = false;
    private CombatPawn m_currentCombatPawn;
    private int m_trigger = 0;
    private GameObject m_netExecuteStateObject = null;
    private NetExecuteState m_netExecuteState;
    private bool m_isExiting = false;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_isExiting = false;

        Debug.Log("Entering execute state");
        base.OnStateEnter(animator, stateInfo, layerIndex);

        // If is the master client, instantiate a NetExecuteState to all clients upon entering
        if (PhotonNetwork.isMasterClient)
        {
            m_netExecuteStateObject = PhotonNetwork.Instantiate("NetExecuteState", Vector3.zero, Quaternion.identity, 0);
            PhotonNetwork.Spawn(m_netExecuteStateObject.GetComponent<PhotonView>());
            m_netExecuteState = m_netExecuteStateObject.GetComponent<NetExecuteState>();
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        if (PhotonNetwork.isMasterClient && !m_isExiting)
        {
            // If the NetExecuteState execute to win boolean is true, transition to the win state
            if (m_netExecuteState.ExecuteToWin)
            {
                ExitState();
                StateMachine.SetBool("ExecuteToWin", true);
                CManager.SetCurrentState(StateMachine.GetBehaviour<WinState>());
            }

            // If the NetExecuteState execute to lose boolean is true, transition to the lose state
            else if (m_netExecuteState.ExecuteToLose)
            {
                ExitState();
                StateMachine.SetBool("ExecuteToLose", true);
                CManager.SetCurrentState(StateMachine.GetBehaviour<LoseState>());
            }

            // If the NetExecuteState execute to think boolean is true, transition to the think state
            else if (m_netExecuteState.ExecuteToThink)
            {
                ExitState();
                StateMachine.SetBool("ThinkToExecute", false);
                StateMachine.SetBool("ExecuteToThink", true);
                CManager.SetCurrentState(StateMachine.GetBehaviour<ThinkState>());
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

        if (m_netExecuteStateObject != null)
        {
            Destroy(m_netExecuteStateObject);
        }
        
        m_netExecuteStateObject = null;
        m_netExecuteState = null;
    }
}