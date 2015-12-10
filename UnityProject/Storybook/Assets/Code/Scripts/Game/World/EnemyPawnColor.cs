using UnityEngine;
using System.Collections;

public class EnemyPawnColor : MonoBehaviour {

	// Use this for initialization
	void Awake () {
        MeshRenderer goRenderer = this.GetComponent<MeshRenderer>();
        goRenderer.material.color = Color.red;
    }
}
