using UnityEngine;

public class StandardCombatInstance : CombatInstance
{
    private ResourceAsset m_playerTeamPrefab;
    private ResourceAsset m_enemyTeamPrefab;

    public StandardCombatInstance(ResourceAsset playerTeamPrefab, ResourceAsset enemyTeamPrefab)
    {
        m_playerTeamPrefab = playerTeamPrefab;
        m_enemyTeamPrefab = enemyTeamPrefab;
    }

    public override CombatTeam[] CreateTeams()
    {
        CombatTeam playerTeam = PhotonNetwork.Instantiate<CombatTeam>(m_playerTeamPrefab, Vector3.zero,
            Quaternion.identity, 0);
        CombatTeam enemyTeam = PhotonNetwork.Instantiate<CombatTeam>(m_enemyTeamPrefab, Vector3.zero,
            Quaternion.identity, 0);


        CombatTeam[] teams = {playerTeam, enemyTeam};
        return teams;
    }
}
