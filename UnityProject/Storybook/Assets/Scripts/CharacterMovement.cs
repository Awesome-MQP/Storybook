using UnityEngine;
using System.Collections;

public class CharacterMovement : MonoBehaviour {
	
	public float speed = 8.0F;
	public float gravity = 30.0F;

	private Vector3 m_moveDirection = Vector3.zero;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		CharacterController controller = GetComponent<CharacterController>();
		m_moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		m_moveDirection = transform.TransformDirection(m_moveDirection);
		m_moveDirection *= speed;
		m_moveDirection.y -= gravity * Time.deltaTime;
		controller.Move(m_moveDirection * Time.deltaTime);
	}
}
