using UnityEngine;
using System.Collections;

public class WinState : CombatState {

    private GameObject m_netWinStateObject = null;
    private NetWinState m_netWinState;
    private bool m_isExiting = false;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        m_isExiting = false;

        // If it is the master client, instantiate a NetWinState for the other clients to receive
        if (PhotonNetwork.isMasterClient)
        {
            m_netWinStateObject = PhotonNetwork.Instantiate("CombatStateMachine/NetWinState", Vector3.zero, Quaternion.identity, 0);
            PhotonNetwork.Spawn(m_netWinStateObject.GetComponent<PhotonView>());
            m_netWinState = m_netWinStateObject.GetComponent<NetWinState>();
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        // If it is the master client and the NetWinState says to exit the combat, call ExitState()
        if (PhotonNetwork.isMasterClient)
        {
            if (m_netWinState.ExitCombat && !m_isExiting)
            {
                Debug.Log("Exiting win state");
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

        // Delete the current combat
        m_netWinState.DeleteCombat();

        // Delete the NetWinState from all clients
        if (m_netWinStateObject != null)
        {
            // TODO: Change back to just calling Destroy when that is fixed
            PhotonNetwork.Destroy(m_netWinStateObject);
            Destroy(m_netWinStateObject);
        }
        m_isExiting = true;
        ResetBools();
        StateMachine.SetBool("ExitCombat", true);
    }
}
