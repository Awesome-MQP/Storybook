using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TestCombatPawn : CombatPlayer {

    private PlayerPositionNode m_currentDest;
    private float m_startTime;
    private float m_moveSpeed = 0.5F;
    private float m_destDistance;

    // Give the player one move for testing that is triggered when the space bar is pressed
    private PlayerMove m_testMove;

	// Use this for initialization
	void Start () {
        SetSpeed(7);
        SetHealth(10);
        m_testMove = new TestPageMove();
	}

    // Waits for input of a move
    public override IEnumerator OnThink() {
        bool hasReceivedInput = false;
        while (!hasReceivedInput)
        {
            // If the space bar is pressed, submit the move to the CombatManager, and exit the OnThink() function
            if (Input.GetKeyDown(KeyCode.Space))
            {
                hasReceivedInput = true;

                if (!HasSubmittedMove)
                {
                    // TODO - Way for player to select targets
                    List<CombatPawn> targetList = new List<CombatPawn>(CManager.EnemyList);
                    m_testMove.SetMoveTargets(targetList);
                    SetMoveForTurn(m_testMove);
                    CManager.SubmitPlayerMove(m_testMove);
                    SetHasSubmittedMove(true);
                }
            }
            yield return null;
        }
    }

    // Moves the CombatPawn
    public override IEnumerator OnAction() {
        // If this is the first call to OnAction, figure out the position to move to
        if (!IsInAction)
        {
            m_startTime = Time.time;
            float currentFarthestDist = 0;
            foreach(PlayerPositionNode ppn in CManager.PlayerPositions)
            {
                if (Vector3.Distance(transform.position, ppn.transform.position) > currentFarthestDist){
                    m_currentDest = ppn;
                }
            }
            SetIsInAction(true); ;
            m_destDistance = currentFarthestDist;
        }

        while (!(Vector3.Distance(transform.position, m_currentDest.transform.position) < 0.01))
        {
            // Lerp to the destination
            float distCovered = (Time.time - m_startTime) * m_moveSpeed;
            float fracJourney = distCovered / m_destDistance;
            Vector3 lerpVector = Vector3.Lerp(transform.position, m_currentDest.transform.position, fracJourney);
            if (!float.IsNaN(lerpVector.x) && !float.IsNaN(lerpVector.y) && !float.IsNaN(lerpVector.z))
            {
                transform.position = lerpVector;
            }
            yield return null;
        }
        MoveForTurn.DoMove();
        SetIsInAction(false);
        SetIsActionComplete(true);
        CManager.PlayerFinishedMoving();
    }
}
