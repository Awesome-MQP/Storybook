using System;
using UnityEngine;
using UnityEditor;

public class StandardCombatInstance : CombatInstance
{
    private ResourceAsset m_playerTeamPrefab;
    private ResourceAsset m_enemyTeamPrefab;
    private AudioClip m_combatMusic;
    private AudioClip m_previousMusic;

    public StandardCombatInstance(ResourceAsset playerTeamPrefab, ResourceAsset enemyTeamPrefab, AudioClip combatMusic, AudioClip previousMusic)
    {
        m_playerTeamPrefab = playerTeamPrefab;
        m_enemyTeamPrefab = enemyTeamPrefab;
        m_previousMusic = previousMusic;
        m_combatMusic = combatMusic;
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

    public override AudioClip GetPreviousMusic()
    {
        return m_previousMusic;
    }

    public override AudioClip GetCombatMusic()
    {
        return m_combatMusic;
    }
}
