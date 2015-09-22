using UnityEngine;
using System.Collections;

public class ExecuteState : CombatState {

    private bool m_areCombatMovesComplete = false;
    private CombatPawn m_currentCombatPawn;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        GetNextCombatPawn();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (!m_currentCombatPawn.IsActionComplete)
        {
            m_currentCombatPawn.OnAction();
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
        StateMachine.SetTrigger("ExecuteToWin");
    }

    // TODO - How to handle ties with the speed values (players have priority?)
    public void GetNextCombatPawn()
    {
        int currentHighestSpeed = 0;
        CombatPawn fastestCombatPawn = null;
        foreach (CombatPawn combatPawn in CManager.AllPawns)
        {
            int currentSpeed = combatPawn.Speed;
            if (currentSpeed > currentHighestSpeed && !combatPawn.IsActionComplete)
            {
                currentHighestSpeed = currentSpeed;
                fastestCombatPawn = combatPawn;
            }
        }

        if (fastestCombatPawn != null)
        {
            m_currentCombatPawn = fastestCombatPawn;
        }
        else
        {
            ExitState();
        }
    }
}
