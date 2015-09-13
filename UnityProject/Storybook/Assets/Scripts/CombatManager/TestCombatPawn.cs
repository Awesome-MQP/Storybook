using UnityEngine;
using System.Collections;
using System;

public class TestCombatPawn : CombatPawn {

	// Use this for initialization
	void Start () {
        StartCoroutine(OnThink());
	}

    public override IEnumerator OnThink() {
        yield return new WaitForSeconds(5);
        CManager.SubmitPlayerMove();
    }

    public override void OnAction() {
        //Debug.Log("Test pawn doing action");
    }
}
