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
    private EnemyMove m_moveForTurn;

    void Awake()
    {
        EnemyMove testEnemyMove = new TestEnemyMove();
        EnemyMoves.Add(testEnemyMove);
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
        m_moveForTurn.DoMove();
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
            m_moveForTurn = moveSelected;

            CManager.SubmitEnemyMove(moveSelected);
            SetHasSubmittedMove(true);
        }
        yield return null;
    }

    public override EnemyMove ChooseMove()
    {
        // Currently just grab the first enemy move
        EnemyMove chosenMove = EnemyMoves[0];
        List<CombatPawn> targets = new List<CombatPawn>();
        if (chosenMove.IsMoveAttack)
        {
            if (chosenMove.NumberOfTargets >= CManager.PlayerPawnList.Length)
            {
                targets = new List<CombatPawn>(CManager.PlayerPawnList);
            }
            else
            {
                while (targets.Count < chosenMove.NumberOfTargets)
                {
                    System.Random rnd = new System.Random();
                    int playerIndex = rnd.Next(0, CManager.PlayerPawnList.Length - 1);
                    CombatPawn selectedPlayer = CManager.PlayerPawnList[playerIndex];
                    if (!targets.Contains(selectedPlayer))
                    {
                        targets.Add(selectedPlayer);
                    }
                }
            }
        }
        else
        {
            if (chosenMove.NumberOfTargets >= CManager.EnemyList.Length)
            {
                targets = new List<CombatPawn>(CManager.EnemyList);
            }
            else
            {
                while (targets.Count < chosenMove.NumberOfTargets)
                {
                    System.Random rnd = new System.Random();
                    int enemyIndex = rnd.Next(0, CManager.EnemyList.Length - 1);
                    CombatPawn selectedEnemy = CManager.EnemyList[enemyIndex];
                    if (!targets.Contains(selectedEnemy))
                    {
                        targets.Add(selectedEnemy);
                    }
                }
            }
        }
        chosenMove.SetMoveTargets(targets);
        return chosenMove;
    }
}
