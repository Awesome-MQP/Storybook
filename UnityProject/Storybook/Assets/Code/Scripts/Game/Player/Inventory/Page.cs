using UnityEngine;
using System.Collections;
using System;

public class Page : Item {

    // Genre page
    [SerializeField]
    private Genre m_pageGenre;

    // Determines the move type
    [SerializeField]
    private Move m_moveType;

    // Rare pages will have area of effect instead of a single target
    [SerializeField]
    private bool m_isRare = false;

    // Page level
    [SerializeField]
    private int m_pageLevel = 0;

    // Initialize mods to 0
    [SerializeField]
    private int m_strengthMod = 0;
    [SerializeField]
    private int m_hitpointsMod = 0;
    [SerializeField]
    private int m_speedMod = 0;
    [SerializeField]
    private int m_defenseMod = 0;

	// Use this for initialization
	protected override void Awake () {
        base.Awake();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    protected override void OnPickup()
    {
        throw new NotImplementedException();
    }

    protected override void OnDrop()
    {
        throw new NotImplementedException();
    }

    protected override void OnMoved()
    {
        throw new NotImplementedException();
    }
}
