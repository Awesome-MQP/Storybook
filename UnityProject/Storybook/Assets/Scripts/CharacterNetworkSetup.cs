using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class CharacterNetworkSetup : NetworkBehaviour {

	// Use this for initialization
	void Start () {
        if (isLocalPlayer)
        {
            GetComponent<CharacterController>().enabled = true;
            GetComponent<CharacterMovement>().enabled = true;
        }
	}

}
