using UnityEngine;
using System.Collections;

public class NetLoseState : NetworkState {

    private bool m_exitCombat = false;

    void Awake()
    {
        SetCombatManager(FindObjectOfType<CombatManager>());
    }

	// Use this for initialization
	void Start () {
        m_exitCombat = true;
	}

    public bool ExitCombat
    {
        get { return m_exitCombat; }
    }
}
