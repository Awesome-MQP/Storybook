using UnityEngine;
using System.Collections;
using System;

public class TestCombatPawn : CombatPawn {

    private const int ATTACK_VALUE = 2;
    private PlayerPositionNode m_currentDest;
    private float m_startTime;
    private float m_moveSpeed = 0.5F;
    private float m_destDistance;
    private CombatEnemy m_enemyToAttack;

	// Use this for initialization
	void Start () {
        Speed = 7;
        Health = 10;
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
                int enemiesInCombat = CManager.EnemyList.Count;
                System.Random rnd = new System.Random();
                int enemyToAttackIndex = rnd.Next(0, enemiesInCombat - 1);
                m_enemyToAttack = CManager.EnemyList[enemyToAttackIndex];

                if (!HasSubmittedMove)
                {
                    CManager.SubmitPlayerMove();
                    HasSubmittedMove = true;
                }
            }
            yield return null;
        }
    }

    // Moves the CombatPawn
    public override void OnAction() {
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
        float distCovered = (Time.time - m_startTime) * m_moveSpeed;
        float fracJourney = distCovered / m_destDistance;
        Vector3 lerpVector = Vector3.Lerp(transform.position, m_currentDest.transform.position, fracJourney);
        if (!float.IsNaN(lerpVector.x) && !float.IsNaN(lerpVector.y) && !float.IsNaN(lerpVector.z))
        {
            transform.position = lerpVector;
        }
        // If the pawn is at its destination, notify the CombatManager that a player has finished moving
        if (Vector3.Distance(transform.position, m_currentDest.transform.position) < 0.01)
        {
            Debug.Log("Player dealing damage to enemy");
            m_enemyToAttack.DealDamageToPawn(ATTACK_VALUE);
            IsInAction = false;
            IsActionComplete = true;
            CManager.PlayerFinishedMoving();
        }
    }
}
