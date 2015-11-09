using UnityEngine;
using System.Collections;

public class NetWinState : NetworkState {

    private bool m_exitCombat = false;
    private bool m_isClientReady = false;
    private int m_playersReady = 0;
    private int m_trigger = 0;

    void Awake()
    {
        SetCombatManager(FindObjectOfType<CombatManager>());
    }

    void Update()
    {
        if (!m_isClientReady)
        {
            m_trigger += 1;
            if (m_trigger > 250)
            {
                m_isClientReady = true;
                GetComponent<PhotonView>().RPC("IncrementPlayersReady", PhotonTargets.All);
            }
        }
        if (m_playersReady >= PhotonNetwork.playerList.Length)
        {
            m_exitCombat = true;
        }
    }

    public bool ExitCombat
    {
        get { return m_exitCombat; }
    }

    // Ends the current combat
    public void DeleteCombat()
    {
        CManager.EndCurrentCombat();
    }

    /// <summary>
    /// Increments the number of players ready on all clients
    /// </summary>
    [PunRPC]
    private void IncrementPlayersReady()
    {
        m_playersReady += 1;
    }

}
