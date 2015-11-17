using UnityEngine;
using System.Collections.Generic;

public abstract class CombatTeam : MonoBehaviour {

    private List<CombatPawn> m_allPawnsSpawned = new List<CombatPawn>();

    [SerializeField]
    private List<CombatPawn> m_pawnsOnTeam = new List<CombatPawn>();

    private List<Vector3> m_pawnPositions = new List<Vector3>();

    public virtual void RemovePawnFromTeam(CombatPawn pawnToRemove)
    {
        m_pawnsOnTeam.Remove(pawnToRemove);
    }

    public abstract void SpawnTeam();

    public abstract void StartCombat();

    public abstract void StartNewTurn();

    public bool IsTeamDefeated()
    {
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
}
