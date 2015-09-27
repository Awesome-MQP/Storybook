using UnityEngine;
using System.Collections.Generic;

public abstract class CombatEnemy : CombatPawn {

    /// <summary>
    /// The list of moves that the enemy can use in combat
    /// </summary>
    private List<EnemyMove> m_enemyMoveList = new List<EnemyMove>();

    /// <summary>
    /// Property getter for the list of enemy moves
    /// </summary>
    public EnemyMove[] EnemyMoves
    {
        get { return m_enemyMoveList.ToArray(); }
    }

    /// <summary>
    /// Setter for the list of enemy moves
    /// </summary>
    /// <param name="newEnemyMoveList">The new list of enemy moves</param>
    public void SetEnemyMoves(List<EnemyMove> newEnemyMoveList)
    {
        m_enemyMoveList = newEnemyMoveList;
    }

    /// <summary>
    /// Chooses a move for the enemy pawn to use in the current turn of combat
    /// Also sets the targets of the selected move
    /// </summary>
    /// <returns>The EnemyMove that will be used for the current turn</returns>
    public EnemyMove ChooseMove()
    {
        // TODO - System to select which move to do for the turn
        // Currently just grab the first enemy move
        EnemyMove chosenMove = EnemyMoves[0];

        // Select the targets for the move
        List<CombatPawn> targets = new List<CombatPawn>();

        // If the move is an attack, select players as the targets
        if (chosenMove.IsMoveAttack)
        {

            // If the number of targets is equal to the number of players or greater, set the targets to all of the players
            if (chosenMove.NumberOfTargets >= CManager.PlayerPawnList.Length)
            {
                targets = new List<CombatPawn>(CManager.PlayerPawnList);
            }

            // Otherwise, randomly choose the players that will be targeted
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

        // If the move is not an attack, select other enemies as the targets
        else
        {

            // If the number of targets is equal to the number of enemies or greater, set the targets to all the enemies 
            if (chosenMove.NumberOfTargets >= CManager.EnemyList.Length)
            {
                targets = new List<CombatPawn>(CManager.EnemyList);
            }

            // Otherwise, randomly choose the enemies that will be targeted
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
