using UnityEngine;
using System.Collections;
using System;

public class TestEnemy : CombatEnemy
{
    // Dummy attack value for testing the combat scene
    private const int ATTACK_VALUE = 1;

    private EnemyPositionNode m_currentDest;
    private float m_startTime;
    private float m_moveSpeed = 0.5F;
    private float m_destDistance;
    private CombatPawn m_playerToAttack;

    void Start()
    {
        Speed = 5;
        Health = 10;
    }

    public override void OnAction()
    {
        // If this is the first call to OnAction, figure out the position to move to
        if (!IsInAction)
        {
            m_startTime = Time.time;
            float currentFarthestDist = 0;
            foreach (EnemyPositionNode epn in CManager.EnemyPositions)
            {
                if (Vector3.Distance(transform.position, epn.transform.position) > currentFarthestDist)
                {
                    m_currentDest = epn;
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
            m_playerToAttack.DealDamageToPawn(ATTACK_VALUE);
            IsInAction = false;
            IsActionComplete = true;
            CManager.EnemyFinishedMoving();
        }
    }

    public override IEnumerator OnThink()
    {
        // Randomly select a player pawn to attack
        int playerPawnsInCombat = CManager.PlayerPawnList.Count;
        System.Random rnd = new System.Random();
        int playerPawnIndex = rnd.Next(0, playerPawnsInCombat - 1);
        m_playerToAttack = CManager.PlayerPawnList[playerPawnIndex];

        // Submit the move to the combat manager
        if (!HasSubmittedMove)
        {
            CManager.SubmitEnemyMove();
            HasSubmittedMove = true;
        }
        yield return null;
    }
}
