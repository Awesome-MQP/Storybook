using UnityEngine;
using System.Collections;

public class NetWinState : NetworkState {

    private bool m_exitCombat = false;

    private int m_trigger = 0;

    void Awake()
    {
        SetCombatManager(FindObjectOfType<CombatManager>());
    }

    void Update()
    {
        m_trigger += 1;
        if (m_trigger > 250)
        {
            m_exitCombat = true;
        }
    }

    public bool ExitCombat
    {
        get { return m_exitCombat; }
    }

    public void DeleteCombat()
    {
        CManager.EndCurrentCombat();
    }

}
