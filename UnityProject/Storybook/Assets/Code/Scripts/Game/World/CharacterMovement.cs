﻿using UnityEngine;
using System.Collections.Generic;

// CLASS IS DEPRECATED

public class CharacterMovement : MonoBehaviour {

    /*

	[SerializeField]
	private float m_speed = 20.0F;

	[SerializeField]
	private float m_gravity = 30.0F;

	[SerializeField]
	private NavArea m_currentNavArea;

	private Vector3 m_moveDirection = Vector3.zero;
	private List<Vector3> m_characterPath = new List<Vector3>();
	private int m_currentPathIndex;
	private Vector3 m_targetPosition;
	private Vector3[] m_previousPositions = new Vector3[10];
	private int m_prevPositionIndex = 0;
    private Location m_playerLoc;

	// Use this for initialization
	void Start () {
        Vector3 nodePos = FindObjectOfType<NavNode>().transform.position;
        nodePos.y += 2;
        transform.position = nodePos;
		for (int i = 0; i < 10; i++){
			m_previousPositions[i] = Vector3.zero;
		}
		FindCurrentNavArea();
        m_playerLoc = new Location(0, 0);
	}
	
	// Update is called once per frame
	void Update () {
		CharacterController controller = GetComponent<CharacterController>();
		m_moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

		// If it is touch controls and the mouse has been clicked, calculate the path for the character
		if (Input.GetMouseButtonDown(0)){
			_calculateCharacterPath();
		}

		// If the character path is not empty, move to the current destination
		if (m_characterPath.Count > 0){
			// Save the current position to the previous positions array and increment the index
			m_previousPositions[m_prevPositionIndex] = transform.position;
			m_prevPositionIndex++;
			bool hasStopped = true;

			// If the position index has reached the length of the positions array, reset it and check to see if 
			// all the positions in the array are the same
			if (m_prevPositionIndex == m_previousPositions.Length){
				m_prevPositionIndex = 0;
				Vector3 previous;
				previous = m_previousPositions[0];

				// Iterate through all the previous positions to see if they are all the same
				int previousPositionsLength = m_previousPositions.Length;
				for (int i = 1; i < previousPositionsLength; i++){
					if (previous != m_previousPositions[i]){
						hasStopped = false;
					}
					previous = m_previousPositions[i];
				}
			}

			// Otherwise, the character may still be moving so set hasStopped to false
			else {
				hasStopped = false;
			}

			Vector3 currentDest = m_characterPath[m_currentPathIndex];
			currentDest.y = transform.position.y;
			m_moveDirection = currentDest - transform.position;

			// If the character is within range of the destination or has stopped moving, increment te path index
			if (Vector3.Distance(currentDest, transform.position) < 0.2 || hasStopped){
				m_currentPathIndex += 1;

				// If the character has reached the end of the path, reset the path and set isMoving to false
				// since the character will be stopped
				if (m_currentPathIndex == m_characterPath.Count){
					m_characterPath = new List<Vector3>();
					m_previousPositions = new Vector3[10];
				}
			}
		}
		// Set the move direction based on speed, gravity and time if the character is moving or if it isn't touch controls
		m_moveDirection = transform.TransformDirection(m_moveDirection);
		m_moveDirection = m_moveDirection.normalized * m_speed;
		m_moveDirection.y -= m_gravity * Time.deltaTime;
		controller.Move(m_moveDirection * Time.deltaTime);

        // Check to see if the player is at a door
        Door d = _isAtRoomDoor();

        // If the player is at a door, spawn the room for the door if it has not already been spawned
        if (d != null)
        {
            MapManager mapManager = FindObjectOfType<MapManager>();
            if (!d.IsDoorRoomSpawned)
            {
                Location newRoomLoc = d.RoomThroughDoorLoc;
                RoomObject newRoom = mapManager.PlaceRoom(newRoomLoc);
                d.SetIsDoorRoomSpawned(true);
            }

            // If the room as already been spawned, move the player to the new room
            else
            {             
                // Find the door that is paired to the one being entered
                Door exitDoor = mapManager.GetDoorPartner(m_playerLoc, d);
                DoorNode doorNode = exitDoor.DoorNode;
                transform.position = doorNode.transform.position;
                m_playerLoc = d.RoomThroughDoorLoc;
                mapManager.MoveCamera(m_playerLoc);

                // Reset movement variables so the player stops moving when it enters the new room
                m_characterPath = new List<Vector3>();
                m_previousPositions = new Vector3[10];

                // Find the nav area for the new room
                FindCurrentNavArea();
            }
        }
	}

	// Used with touch controls, calculates a path the clicked location
	private void _calculateCharacterPath(){

		// Get the position of the click in the game world
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		Debug.DrawRay(ray.origin, ray.direction, Color.black, 10000);
		Plane xy = new Plane(Vector3.up, new Vector3(0, 0, 0));
		float point;
		xy.Raycast(ray, out point);
		m_targetPosition = ray.GetPoint (point);
		m_targetPosition.y = transform.position.y;

		Vector3 direction = m_targetPosition - transform.position;
		float distance = Vector3.Distance(transform.position, m_targetPosition);
		Ray charToDestRay = new Ray(transform.position, direction);
		Debug.DrawRay(transform.position, direction, Color.cyan, 10000);

		GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");

		// Check to make sure that the point is not located in a wall
		int wallsLength = walls.Length;
		for (int i = 0; i < wallsLength; i++){
			GameObject currentWall = walls[i];
			BoxCollider wallCollider = currentWall.GetComponent(typeof(BoxCollider)) as BoxCollider;
			if (wallCollider != null){
				// If the clicked point is in a wall, the character can not move to here, so return
				if (wallCollider.bounds.Contains(m_targetPosition)){;
					return;
				}
			}
		}

		// If the character can move the clicked point without hitting anything, the path will just contain the destination
		if (!Physics.Raycast(charToDestRay, distance)){
			m_characterPath.Add(m_targetPosition);
		}

		// Otherwise, need to use A* to determine a path
		else {
			NavNode closestDestNode = null;
			float smallestDestDistance = -1;
			NavNode closestCharacterNode = null;
			float smallestCharacterDistance = -1;
			List<NavNode> areaNodes = m_currentNavArea.getAreaNodes();

			// Iterate through the area nodes to find the node closest to the character and the node
			// closest to the destination
			int areaNodesCount = areaNodes.Count;
			for (int i = 0; i < areaNodesCount; i++){
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

			// Use A* to calculate a path
			List<NavNode> characterNodePath = m_currentNavArea.AStarSearch(closestCharacterNode, closestDestNode);
			if (characterNodePath.Contains(closestCharacterNode)){
				characterNodePath.Insert (0, closestCharacterNode);
			}

			// Iterate through the path and add the positions of all the nodes to the character path
			for (int i = 0; i < characterNodePath.Count; i++){
				m_characterPath.Add(characterNodePath[i].transform.position);
			}
			m_characterPath.Add (m_targetPosition);
		}
		m_currentPathIndex = 0;
	}

	// Returns the camera
	Camera FindCamera (){
		if (GetComponent<Camera>())
			return GetComponent<Camera>();
		else
			return Camera.main;
	}

	// Finds the current NavArea that the player object is in and sets the NavArea private variable of the 
	// character to this NavArea
	public void FindCurrentNavArea(){
		NavArea[] allNavAreas = FindObjectsOfType(typeof(NavArea)) as NavArea[];
		int allNavAreaLength = allNavAreas.Length;
		for (int i = 0; i < allNavAreaLength; i++){
			NavArea currentNavArea = allNavAreas[i];
			BoxCollider areaCollider = currentNavArea.GetComponent(typeof(BoxCollider)) as BoxCollider;
			if (areaCollider.bounds.Contains(transform.position)){
				m_currentNavArea = currentNavArea;
				break;
			}
		}
	}

    /// <summary>
    /// Checks the distance between the player and all of the doors and returns a door if the player is close enough
    /// </summary>
    /// <returns>Returns a door object if the player is close enough to a door, returns null other wise</returns>
    private Door _isAtRoomDoor()
    {
        MapManager mapManager = FindObjectOfType<MapManager>();
        RoomObject currentRoom = mapManager.GetRoom(m_playerLoc);
        Door[] roomDoors = currentRoom.RoomDoors;
        foreach(Door d in roomDoors)
        {
            // Only check doors that are enabled
            if (d.IsDoorEnabled)
            {
                Vector3 doorPos = d.transform.position;
                float distanceToDoor = Vector3.Distance(doorPos, transform.position);
                if (distanceToDoor < 1.5)
                {
                    return d;
                }
            }
        }
        return null;
    }

    */
	
}