using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;

public class CharacterMovement : NetworkBehaviour {

    [SerializeField]
	private float m_speed = 20.0F;

	[SerializeField]
	private float m_gravity = 30.0F;

    private Vector3 m_moveDirection = Vector3.zero;

	// Use this for initialization
	void Start () {
        transform.position = new Vector3(1.7451f, 0.3097f, 3.3220f);
	}
	
	// Update is called once per frame
	void Update () {
		CharacterController controller = GetComponent<CharacterController>();
		m_moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

		// Set the move direction based on speed, gravity and time if the character is moving or if it isn't touch controls
		m_moveDirection = transform.TransformDirection(m_moveDirection);
		m_moveDirection = m_moveDirection.normalized * m_speed;
		m_moveDirection.y -= m_gravity * Time.deltaTime;
		controller.Move(m_moveDirection * Time.deltaTime);
	}
}