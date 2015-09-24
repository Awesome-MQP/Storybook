using UnityEngine;
using System.Collections;
using System;

public class TestCombatPawn : CombatPawn {

    // Dummy attack value for testing the combat scene scene
    private const int ATTACK_VALUE = 2;

    private PlayerPositionNode m_currentDest;
    private float m_startTime;
    private float m_moveSpeed = 0.5F;
    private float m_destDistance;
    private CombatEnemy m_enemyToAttack;

	// Use this for initialization
	void Start () {
        SetSpeed(7);
        SetHealth(10);
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

                // Randomly select an enemy pawn to attack
                int enemiesInCombat = CManager.EnemyList.Length;
                System.Random rnd = new System.Random();
                int enemyToAttackIndex = rnd.Next(0, enemiesInCombat - 1);
                m_enemyToAttack = CManager.EnemyList[enemyToAttackIndex];

                if (!HasSubmittedMove)
                {
                    CManager.SubmitPlayerMove();
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
        m_enemyToAttack.DealDamageToPawn(ATTACK_VALUE);
        SetIsInAction(false);
        SetIsActionComplete(true);
        CManager.PlayerFinishedMoving();
    }
}
