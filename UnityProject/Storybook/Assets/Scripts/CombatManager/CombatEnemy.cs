using UnityEngine;
using System.Collections.Generic;

public abstract class CombatEnemy : CombatPawn {

    /// <summary>
    /// Value that increases the likelihood of the enemy choosing an attack over a effect boost
    /// Value is out of 100 and represents the probability of an attack move being chosen
    /// </summary>
    [SerializeField]
    private int m_aggressionValue;

    /// <summary>
    /// The amount of points that the enemy has to spend on their move for turn
    /// </summary>
    [SerializeField]
    private int m_currentMana;

    /// <summary>
    /// The amount of mana that the enemy receives at the beginning of each turn
    /// </summary>
    [SerializeField]
    private int m_manaPerTurn;


    /// <summary>
    /// The list of moves that the enemy can use in combat
    /// </summary>
    private List<EnemyMove> m_enemyMoveList = new List<EnemyMove>();

    private System.Random enemyRNG = new System.Random();

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
        EnemyMove mostExpensiveMove = _getMostExpensiveMove();

        // If the enemy currently has twice as much mana as its most expensive move, use the most expensive move this turn
        if (m_currentMana >= mostExpensiveMove.MoveCost * 2)
        {
            return mostExpensiveMove;
        }

        bool isMoveAttack;
        
        // If the enemy only has one type of move, set the IsMoveAttack boolean according to the type of move that it has
        // If the enemy only has attacks, set isMoveAttack to true
        if (_hasOnlyAttacks())
        {
            isMoveAttack = true;
        }
        // If the enemy only has boosts, set isMoveAttack to false
        else if (_hasOnlyBoosts())
        {
            isMoveAttack = false;
        }
        // Otherwise, randomly generate a boolean for isMoveAttack based on its aggression value
        else
        {
            isMoveAttack = _isMoveAttack();
        }

        List<EnemyMove> possibleMoves = new List<EnemyMove>();
        
        foreach (EnemyMove em in EnemyMoves)
        {
            if (isMoveAttack == em.IsMoveAttack && em.MoveCost <= m_currentMana)
            {
                possibleMoves.Add(em);
            }
        }

        int moveIndex = enemyRNG.Next(0, possibleMoves.Count - 1);

        EnemyMove chosenMove = possibleMoves[moveIndex];

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
        Debug.Log("MANA = " + m_currentMana.ToString());
        m_currentMana -= chosenMove.MoveCost;
        Debug.Log("Chosen move cost = " + chosenMove.MoveCost.ToString());
        Debug.Log("Mana after using move = " + m_currentMana.ToString());
        return chosenMove;
    }

    /// <summary>
    /// Randomly generates a boolean that determines whether the enemy will choose an attack or a boost based on 
    /// the enemy's aggression value
    /// </summary>
    /// <returns>True if the enemy will choose an attack, false otherwise</returns>
    private bool _isMoveAttack()
    {
        int randomNumber = enemyRNG.Next(0, 100);
        return (randomNumber <= m_aggressionValue);
    }

    /// <summary>
    /// Checks to see if the enemy only has or can only use (due to mana) attack moves
    /// </summary>
    /// <returns>True if the enemy only has or can only use attack moves, false otherwise</returns>
    private bool _hasOnlyAttacks()
    {
        foreach (EnemyMove em in EnemyMoves)
        {
            if (!em.IsMoveAttack && em.MoveCost <= m_currentMana)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Checks to see if the enemy has only or can only use (due to mana) boost moves
    /// </summary>
    /// <returns>True if the enemy only has or can only use boost moves, false otherwise</returns>
    private bool _hasOnlyBoosts()
    {
        foreach (EnemyMove em in EnemyMoves)
        {
            if (em.IsMoveAttack && em.MoveCost <= m_currentMana)
            {
                return false;
            }
        }
        return true;
    }

    private EnemyMove _getMostExpensiveMove()
    {
        EnemyMove currentHighest = null;
        foreach(EnemyMove cm in EnemyMoves)
        {
            if (currentHighest == null || cm.MoveCost > currentHighest.MoveCost)
            {
                currentHighest = cm;
            }
        }
        return currentHighest;
    }

    public void _initializeManaPerTurn()
    {
        EnemyMove leastExpensiveMove = null;
        foreach(EnemyMove em in EnemyMoves)
        {
            if (leastExpensiveMove == null || em.MoveCost < leastExpensiveMove.MoveCost)
            {
                leastExpensiveMove = em;
            }
        }
        m_manaPerTurn = leastExpensiveMove.MoveCost;
    }

    public void IncrementManaForTurn()
    {
        m_currentMana += m_manaPerTurn;
    }
}
