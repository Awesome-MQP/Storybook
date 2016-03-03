using System;
using UnityEngine;

public class StandardCombatInstance : CombatInstance
{
    private ResourceAsset m_playerTeamPrefab;
    private ResourceAsset m_enemyTeamPrefab;
    private Genre m_combatGenre;
    private int m_combatLevel;
    private AudioClip m_combatMusic;
    private AudioClip m_previousMusic;

    public StandardCombatInstance(ResourceAsset playerTeamPrefab, ResourceAsset enemyTeamPrefab, Genre combatGenre, int combatLevel)
    {
        m_playerTeamPrefab = playerTeamPrefab;
        m_enemyTeamPrefab = enemyTeamPrefab;
        m_combatGenre = combatGenre;
        m_combatLevel = combatLevel;
    }

    public override CombatTeam[] CreateTeams()
    {
        CombatTeam playerTeam = PhotonNetwork.Instantiate<CombatTeam>(m_playerTeamPrefab, Vector3.zero,
            Quaternion.identity, 0);
        CombatTeam enemyTeam = PhotonNetwork.Instantiate<CombatTeam>(m_enemyTeamPrefab, Vector3.zero,
            Quaternion.identity, 0);


        CombatTeam[] teams = { playerTeam, enemyTeam };
        return teams;
    }

    public override Genre GetGenre()
    {
        return m_combatGenre;
    }

    public override int GetLevel()
    {
        return  m_combatLevel;
    }

    public override AudioClip GetPreviousMusic()
    {
        return m_previousMusic;
    }

    public override AudioClip GetCombatMusic()
    {
        return m_combatMusic;
    }
}
