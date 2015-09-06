using UnityEngine;
using System.Collections.Generic;

public class CharacterMovement : MonoBehaviour {
	
	public float speed = 20.0F;
	public float gravity = 30.0F;
	public NavArea currentNavArea;
	public bool isTouchControls;

	private Vector3 m_moveDirection = Vector3.zero;
	private bool m_isMoving = false;
	private List<Vector3> m_characterPath = new List<Vector3>();
	private int m_currentPathIndex;
	private Vector3 m_targetPosition;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		CharacterController controller = GetComponent<CharacterController>();
		if (!isTouchControls){
			m_moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		}
		if (isTouchControls && Input.GetMouseButtonDown(0)){
			Debug.Log ("Calculating Path");
			calculateCharacterPath();
			m_isMoving = true;
		}
		if (m_characterPath.Count > 0){
			Vector3 currentDest = m_characterPath[m_currentPathIndex];
			currentDest.y = 0;
			m_moveDirection = currentDest - transform.position;
			Debug.Log ("Distance = " + Vector3.Distance(currentDest, transform.position).ToString ());
			if (Vector3.Distance(currentDest, transform.position) < 1.8){
				m_currentPathIndex += 1;
				if (m_currentPathIndex == m_characterPath.Count){
					m_characterPath = new List<Vector3>();
					m_isMoving = false;
					Debug.Log ("Resetting List");
				}
			}
		}
		if (!isTouchControls || m_isMoving){
			m_moveDirection = transform.TransformDirection(m_moveDirection);
			m_moveDirection = m_moveDirection.normalized * speed;
			m_moveDirection.y -= gravity * Time.deltaTime;
			controller.Move(m_moveDirection * Time.deltaTime);
		}
	}

	private void calculateCharacterPath(){
		Camera mainCamera = FindCamera();

		// Calculate the point where the mouse hits the plane
		RaycastHit hit;
		if (!Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition),  out hit, 100))
			return;

		if (!hit.transform)
			return;

		m_targetPosition = hit.point;
		m_targetPosition.y = transform.position.y;

		Debug.Log ("Mouse position = " + m_targetPosition);


		Vector3 direction = m_targetPosition - transform.position;
		float distance = Vector3.Distance(transform.position, m_targetPosition);
		Ray charToDestRay = new Ray(transform.position, direction);
		Debug.DrawRay(transform.position, direction, Color.cyan, 10000);
		if (!Physics.Raycast(charToDestRay, distance)){
			m_characterPath.Add(m_targetPosition);
			Debug.Log ("Adding target position");
		}
		else {
			NavNode closestDestNode = null;
			float smallestDestDistance = -1;
			NavNode closestCharacterNode = null;
			float smallestCharacterDistance = -1;
			List<NavNode> areaNodes = currentNavArea.getAreaNodes();
			for (int i = 0; i < areaNodes.Count; i++){
				NavNode currentNode = areaNodes[i];
				float nodeToDest = Vector3.Distance(currentNode.transform.position, m_targetPosition);
				if (nodeToDest < smallestDestDistance || smallestDestDistance == -1){
					closestDestNode = currentNode;
					smallestDestDistance = nodeToDest;
				}
				float nodeToCharacter = Vector3.Distance (currentNode.transform.position, transform.position);
				if (nodeToCharacter < smallestCharacterDistance || smallestCharacterDistance == -1){
					closestCharacterNode = currentNode;
					smallestCharacterDistance = nodeToCharacter;
				}
			}
			Debug.Log ("Closest Node to character = " + closestCharacterNode.nodeId.ToString ());
			Debug.Log ("Closest node to destination = " + closestDestNode.nodeId.ToString());
			List<NavNode> characterNodePath = currentNavArea.AStarSearch(closestCharacterNode, closestDestNode);
			if (characterNodePath.Contains(closestCharacterNode)){
				characterNodePath.Insert (0, closestCharacterNode);
			}
			for (int i = 0; i < characterNodePath.Count; i++){
				m_characterPath.Add(characterNodePath[i].transform.position);
			}
			m_characterPath.Add (m_targetPosition);
		}
		m_currentPathIndex = 0;
	}

	Camera FindCamera (){
		if (GetComponent<Camera>())
			return GetComponent<Camera>();
		else
			return Camera.main;
	}
	
}
