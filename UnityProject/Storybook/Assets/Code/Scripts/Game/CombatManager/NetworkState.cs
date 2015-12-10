using UnityEngine;
using System.Collections;

public class NetworkState : Photon.PunBehaviour {

    private CombatManager m_combatManager;

    public CombatManager CManager
    {
        get { return m_combatManager; }
    }

    public void SetCombatManager(CombatManager newCombatManager)
    {
        m_combatManager = newCombatManager;
    }
}
