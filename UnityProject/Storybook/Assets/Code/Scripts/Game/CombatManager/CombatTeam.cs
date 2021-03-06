﻿using UnityEngine;
using System.Collections.Generic;

public abstract class CombatTeam : Photon.PunBehaviour {

    [SerializeField]
    private List<CombatPawn> m_pawnsToSpawn = new List<CombatPawn>();

    private List<CombatPawn> m_allPawnsSpawned = new List<CombatPawn>();

    private List<CombatPawn> m_activePawnsOnTeam = new List<CombatPawn>();

    private List<Vector3> m_pawnPositions = new List<Vector3>();

    private CombatManager m_combatManager;

    private int m_teamId;

    private int m_teamLevel;

    /// <summary>
    /// Removes the pawn from the PawnsOnTeam list
    /// </summary>
    /// <param name="pawnToRemove">The pawn to remove from the list</param>
    public virtual void RemovePawnFromTeam(CombatPawn pawnToRemove)
    {
        m_activePawnsOnTeam.Remove(pawnToRemove);
    }

    /// <summary>
    /// Called at the beginning of combat and initializes the teams
    /// </summary>
    public abstract void SpawnTeam();

    /// <summary>
    /// Called when the combat is started
    /// </summary>
    public abstract void StartCombat();

    /// <summary>
    /// Called when a new turn starts in combat
    /// </summary>
    public abstract void StartNewTurn();

    /// <summary>
    /// Returns true if all of the pawns on the team have been defeated, false otherwise
    /// </summary>
    /// <returns>True if all pawns on team defeated, false otherwise</returns>
    public virtual bool IsTeamDefeated()
    {
        if (m_activePawnsOnTeam.Count == 0)
        {
            return true;
        }

        bool isTeamDefeated = true;
        foreach (CombatPawn pawn in m_activePawnsOnTeam)
        {
            if (pawn.IsAlive)
            {
                isTeamDefeated = false;
                break;
            }
            else
            {
                _disablePawnMesh(pawn);
            }
        }
        return isTeamDefeated;
    }

    /// <summary>
    /// Checks to see if any pawns have been defeated
    /// Removes pawn from the list of active pawns if it has been defeated
    /// </summary>
    /// <returns></returns>
    public virtual List<CombatPawn> CheckForDefeatedPawns()
    {
        List<CombatPawn> defeatedPawns = new List<CombatPawn>();
        foreach (CombatPawn pawn in m_activePawnsOnTeam)
        {
            if (!pawn.IsAlive)
            {
                defeatedPawns.Add(pawn);
            }
        }
        foreach(CombatPawn pawn in defeatedPawns)
        {
            RemovePawnFromTeam(pawn);
            m_combatManager = FindObjectOfType<CombatManager>();
            CManager.RemovePawnMove(pawn);
            _disablePawnMesh(pawn);
        }
        return defeatedPawns;
    }

    private void _disablePawnMesh(CombatPawn pawnToDisable)
    {
        pawnToDisable.GetComponent<Animator>().enabled = false;
        pawnToDisable.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
        MeshRenderer[] allMeshRenderers = pawnToDisable.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer m in allMeshRenderers)
        {
            m.enabled = false;
        }
    }

    public virtual void EndCombat()
    {
        foreach(CombatPawn pawn in m_allPawnsSpawned)
        {
            // TODO: Change back to just calling Destroy when that is fixed
            PhotonNetwork.Destroy(pawn.gameObject);
            Destroy(pawn.gameObject);
        }
    }

    /// <summary>
    /// Adds a pawn to the list of pawns that have been spawned by this team
    /// </summary>
    /// <param name="pawnSpawned">The pawn that has been spawned</param>
    protected void AddPawnToSpawned(CombatPawn pawnSpawned)
    {
        m_allPawnsSpawned.Add(pawnSpawned);
    }

    /// <summary>
    /// Adds a pawn to the on team list
    /// </summary>
    /// <param name="pawnToAdd">The pawn to add to the list of pawns on the team</param>
    public void AddPawnToTeam(CombatPawn pawnToAdd)
    {
        m_activePawnsOnTeam.Add(pawnToAdd);
    }

    /// <summary>
    /// Adds a pawn to the list of pawns that will be spawned when the team is initialized
    /// </summary>
    /// <param name="pawnToSpawn">The pawn to add to the list</param>
    public void AddPawnToSpawn(CombatPawn pawnToSpawn)
    {
        m_pawnsToSpawn.Add(pawnToSpawn);
    }

    public List<CombatPawn> ActivePawnsOnTeam
    {
        get { return m_activePawnsOnTeam; }
    }

    public void SetPawnsOnTeam(List<CombatPawn> pawnsOnTeam)
    {
        m_activePawnsOnTeam = pawnsOnTeam;
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

    [SyncProperty]
    public int TeamId
    {
        get { return m_teamId; }
        set
        {
            m_teamId = value;
            PropertyChanged();
        }
    }

    /// <summary>
    /// Registers the team with the CombatManager, called on clients
    /// </summary>
    /// <param name="teamId">The id of the team to register with the combat manager</param>
    [PunRPC]
    public void RegisterTeamLocal(int teamId)
    {
        m_teamId = teamId;
        FindObjectOfType<CombatManager>().RegisterTeamLocal(this);
    }

    public int TeamLevel
    {
        get { return m_teamLevel; }
        set { m_teamLevel = value; }
    }
}
