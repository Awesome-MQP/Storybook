using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TestEnemy : CombatEnemy
{
    private EnemyPositionNode m_currentDest;
    private float m_startTime;
    private float m_moveSpeed = 0.5F;
    private float m_destDistance;

    void Awake()
    {
        EnemyMove testEnemyMove = new TestEnemyMove();
        List<EnemyMove> enemyMoveList = new List<EnemyMove>();
        enemyMoveList.Add(testEnemyMove);
        SetEnemyMoves(enemyMoveList);
    }

    void Start()
    {
        SetSpeed(5);
        SetHealth(10);
    }

    public override IEnumerator OnAction()
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
            SetIsInAction(true);
            m_destDistance = currentFarthestDist;
        }

        while (!(Vector3.Distance(transform.position, m_currentDest.transform.position) < 0.01)) { 
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
        CManager.EnemyFinishedMoving();
    }

    public override IEnumerator OnThink()
    {
        // Submit the move to the combat manager
        if (!HasSubmittedMove)
        {
            // Randomly select a player pawn to attack
            EnemyMove moveSelected = ChooseMove();
            SetMoveForTurn(moveSelected);

            CManager.SubmitEnemyMove(moveSelected);
            SetHasSubmittedMove(true);
        }
        yield return null;
    }
}
