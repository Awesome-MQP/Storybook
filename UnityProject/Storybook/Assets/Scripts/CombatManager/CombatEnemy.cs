using UnityEngine;
using System.Collections.Generic;

public abstract class CombatEnemy : CombatPawn {

    /// <summary>
    /// Value that increases the likelihood of the enemy choosing an attack over a effect boost
    /// Value is out of 100 and represents the probability of an attack move being chosen
    /// </summary>
    [SerializeField]
    private int m_aggressionValue = -1;

    /// <summary>
    /// The amount of points that the enemy has to spend on their move for turn
    /// </summary>
    [SerializeField]
    private int m_currentMana = -1;

    /// <summary>
    /// The amount of mana that the enemy receives at the beginning of each turn
    /// </summary>
    [SerializeField]
    private int m_manaPerTurn = -1;

    /// <summary>
    /// The list of moves that the enemy can use in combat
    /// </summary>
    [SerializeField]
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
    /// Chooses a move for the enemy pawn to use in the current turn of combat and sets the targets of the move
    /// </summary>
    /// <returns>The EnemyMove that will be used for the current turn</returns>
    public EnemyMove CreateMove()
    {
        // Initalize all of the IsMoveAttack booleans for all of the enemy's moves
        foreach (EnemyMove em in m_enemyMoveList)
        {
            em.InitializeIsMoveAttack();
        }
        EnemyMove chosenMove = _chooseMove();
        CombatPawn[] possibleTargets;

        // If the chosen move is an attack, the possible targets are the players
        if (chosenMove.IsMoveAttack) {
            possibleTargets = CManager.PlayerPawnList;
        }
        // Otherwise the possible targets are the enemies
        else
        {
            possibleTargets = CManager.EnemyList;
        }

        chosenMove.SetMoveTargets(new List<CombatPawn>());
        chosenMove.ChooseTargets(possibleTargets);
        m_currentMana -= chosenMove.MoveCost;
        return chosenMove;
    }

    /// <summary>
    /// Chooses which move to use for the turn, move chosen is determined by current mana and the aggression value of the enemy
    /// </summary>
    /// <returns>The EnemyMove to use for the turn</returns>
    private EnemyMove _chooseMove()
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

        EnemyMove chosenMove = null;
        EnemyMove currentMove = null;

        while (chosenMove == null)
        {
            int moveIndex = enemyRNG.Next(0, possibleMoves.Count - 1);
            currentMove = possibleMoves[moveIndex];
            int willSelectMove = enemyRNG.Next(0, 100);
            if (willSelectMove <= currentMove.MoveFrequency * 100)
            {
                chosenMove = currentMove;
            }
        }
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

    /// <summary>
    /// Searches the enemy move list for the most expensive move
    /// </summary>
    /// <returns>The most expensive move that the enemy has</returns>
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

    /// <summary>
    /// Searches the enemy move list for the least expensive move
    /// </summary>
    /// <returns>The least expensive move that the enemy has</returns>
    private EnemyMove _getLeastExpensiveMove()
    {
        EnemyMove leastExpensiveMove = null;
        foreach (EnemyMove em in EnemyMoves)
        {
            if (leastExpensiveMove == null || em.MoveCost < leastExpensiveMove.MoveCost)
            {
                leastExpensiveMove = em;
            }
        }
        return leastExpensiveMove;
    }

    /// <summary>
    /// Initializes the mana per turn for the enemy
    /// Mana per turn defaults to 1 more than its least expensive move
    /// </summary>
    private void _initializeManaPerTurn()
    {
        EnemyMove leastExpensiveMove = _getLeastExpensiveMove();
        m_manaPerTurn = leastExpensiveMove.MoveCost + 1;
    }

    /// <summary>
    /// Initializes the enemy's starting mana
    /// Defaults to twice the cost of the enemy's least expensive move
    /// </summary>
    private void _initializeStartingMana()
    {
        EnemyMove leastExpensiveMove = _getLeastExpensiveMove();
        m_currentMana = leastExpensiveMove.MoveCost * 2;
    }

    /// <summary>
    /// Initializes the enemy's aggression value
    /// Defaults to 50
    /// </summary>
    private void _initializeAggressionValue()
    {
        m_aggressionValue = 50;
    }

    /// <summary>
    /// Called when an enemy is spawned, initializes variables that have not been set in the editor to their default values
    /// </summary>
    public void InitializeVariables()
    {
        if (ManaPerTurn <= 0)
        {
            _initializeManaPerTurn();
        }
        if (CurrentMana < 0)
        {
            _initializeStartingMana();
        }
        if (AggressionValue < 0)
        {
            _initializeAggressionValue();
        }
    }

    /// <summary>
    /// Increases the enemy's mana by it's manaPerTurn value
    /// </summary>
    public void IncrementManaForTurn()
    {
        m_currentMana += m_manaPerTurn;
    }

    public int CurrentMana
    {
        get { return m_currentMana; }
    }

    public int ManaPerTurn
    {
        get { return m_manaPerTurn; }
    }

    public int AggressionValue
    {
        get { return m_aggressionValue; }
    }
}
