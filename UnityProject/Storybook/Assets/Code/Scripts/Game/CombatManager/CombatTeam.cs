using UnityEngine;
using System.Collections.Generic;

public abstract class CombatTeam : MonoBehaviour {

    [SerializeField]
    private List<CombatPawn> m_pawnsToSpawn = new List<CombatPawn>();

    private List<CombatPawn> m_allPawnsSpawned = new List<CombatPawn>();

    private List<CombatPawn> m_pawnsOnTeam = new List<CombatPawn>();

    private List<Vector3> m_pawnPositions = new List<Vector3>();

    private CombatManager m_combatManager;

    public virtual void RemovePawnFromTeam(CombatPawn pawnToRemove)
    {
        m_pawnsOnTeam.Remove(pawnToRemove);
    }

    public abstract void SpawnTeam();

    public abstract void StartCombat();

    public abstract void StartNewTurn();

    public bool IsTeamDefeated()
    {
        if (m_pawnsOnTeam.Count == 0)
        {
            return true;
        }

        bool isTeamDefeated = true;
        foreach (CombatPawn pawn in m_pawnsOnTeam)
        {
            if (pawn.IsAlive)
            {
                isTeamDefeated = false;
                break;
            }
        }
        return isTeamDefeated;
    }

    public List<CombatPawn> CheckForDefeatedPawns()
    {
        List<CombatPawn> defeatedPawns = new List<CombatPawn>();
        foreach (CombatPawn pawn in m_pawnsOnTeam)
        {
            if (!pawn.IsAlive)
            {
                defeatedPawns.Add(pawn);
            }
        }
        foreach(CombatPawn pawn in defeatedPawns)
        {
            RemovePawnFromTeam(pawn);
        }
        return defeatedPawns;
    }

    protected void AddPawnToSpawned(CombatPawn pawnSpawned)
    {
        m_allPawnsSpawned.Add(pawnSpawned);
    }

    public void AddPawnToTeam(CombatPawn pawnToAdd)
    {
        m_pawnsOnTeam.Add(pawnToAdd);
    }

    public CombatPawn[] PawnsOnTeam
    {
        get { return m_pawnsOnTeam.ToArray(); }
    }

    public void SetPawnsOnTeam(List<CombatPawn> pawnsOnTeam)
    {
        m_pawnsOnTeam = pawnsOnTeam;
    }

    public CombatPawn[] AllPawnsSpawned
    {
        get { return m_allPawnsSpawned.ToArray(); }
    }

    public Vector3[] PawnPositions
    {
        get { return m_pawnPositions.ToArray(); }
    }

    public CombatManager CManager
    {
        get { return m_combatManager; }
    }

    public void RegisterCombatManager(CombatManager combatManager)
    {
        m_combatManager = combatManager;
    }

    public List<CombatPawn> PawnsToSpawn{
        get { return m_pawnsToSpawn; }
    }
}
