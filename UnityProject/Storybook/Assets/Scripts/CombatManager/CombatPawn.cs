using UnityEngine;
using System.Collections;

public abstract class CombatPawn : MonoBehaviour {

    protected CombatManager combatManager;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public abstract IEnumerator OnThink();

    public abstract void OnAction();

    public void SetCombatManager(CombatManager newCombatManager) {
        combatManager = newCombatManager;
    }
}
