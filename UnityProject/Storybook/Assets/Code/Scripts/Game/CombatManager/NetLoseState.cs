using UnityEngine;
using System.Collections;

public class NetLoseState : NetworkState {

    private bool m_exitCombat = false;
    private bool m_isClientReady = false;
    private int m_playersReady = 0;
    private int m_trigger = 0;

    override protected void Awake()
    {
        Debug.Log("Entering lose state");
        SetCombatManager(FindObjectOfType<CombatManager>());
    }

    void Update()
    {
        if (!m_isClientReady)
        {
            m_trigger += 1;
            if (m_trigger > 10)
            {
                m_isClientReady = true;
                GetComponent<PhotonView>().RPC("IncrementPlayersReady", PhotonTargets.All);
            }
        }

        // If all players are ready to exit the lose state, set exitCombat to true
        if (m_playersReady >= PhotonNetwork.playerList.Length)
        {
            m_exitCombat = true;
        }
    }

    public bool ExitCombat
    {
        get { return m_exitCombat; }
    }

    /// <summary>
    /// Increments the number of players ready on all clients
    /// </summary>
    [PunRPC]
    private void IncrementPlayersReady()
    {
        m_playersReady += 1;
    }

    /// <summary>
    /// Calls EndCurrentCombat on the current CombatManager
    /// </summary>
    public void DeleteCombat()
    {
        CManager.EndCurrentCombat();
    }
}
