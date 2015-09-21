using UnityEngine;
using System.Collections;
using System;

public class TestCombatPawn : CombatPawn {

    private PlayerPositionNode m_currentDest;
    private float m_startTime;
    private float m_speed = 0.5F;
    private float m_destDistance;

	// Use this for initialization
	void Start () {
        StartCoroutine(OnThink());
	}

    // Waits for input of a move
    public override IEnumerator OnThink() {
        bool hasReceivedInput = false;
        while (!hasReceivedInput)
        {
            // If the space bar is pressed, submit the move to the CombatManager, and exit the OnThink() function
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Pawn received input");
                hasReceivedInput = true;
                CManager.SubmitPlayerMove();
            }
            yield return null;
        }
    }

    // Moves the CombatPawn
    public override void OnAction() {
        Debug.Log("Test pawn doing action");
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
            IsInAction = true;
            m_destDistance = currentFarthestDist;
        }

        // Lerp to the destination
        float distCovered = (Time.time - m_startTime) * m_speed;
        float fracJourney = distCovered / m_destDistance;
        Vector3 lerpVector = Vector3.Lerp(transform.position, m_currentDest.transform.position, fracJourney);
        if (!float.IsNaN(lerpVector.x) && !float.IsNaN(lerpVector.y) && !float.IsNaN(lerpVector.z))
        {
            transform.position = lerpVector;
        }
        // If the pawn is at its destination, notify the CombatManager that a player has finished moving
        if (Vector3.Distance(transform.position, m_currentDest.transform.position) < 0.05)
        {
            CManager.PlayerFinishedMoving();
            IsInAction = false;
        }
    }
}
