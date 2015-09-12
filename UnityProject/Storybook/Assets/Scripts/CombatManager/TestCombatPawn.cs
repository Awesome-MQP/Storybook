using UnityEngine;
using System.Collections;
using System;

public class TestCombatPawn : CombatPawn {

	// Use this for initialization
	void Start () {
        StartCoroutine(OnThink());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public override IEnumerator OnThink() {
        Debug.Log("Test pawn is thinking");
        yield return new WaitForSeconds(5);
        Debug.Log("Test pawn is done thinking");
        combatManager.SubmitPlayerMove();
    }

    public override void OnAction() {
        Debug.Log("Test pawn doing action");
    }
}
