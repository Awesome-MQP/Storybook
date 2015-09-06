using UnityEngine;
using System.Collections.Generic;

public class NavArea : MonoBehaviour {
	
	public bool isTest;

	[SerializeField]
	private List<NavNode> m_areaNodes = new List<NavNode>();

	// Use this for initialization
	void Start () {
		BoxCollider areaCollider = GetComponent(typeof(BoxCollider)) as BoxCollider;
		Debug.Log("NavArea Starting up");
		NavNode[] allNodes = FindObjectsOfType(typeof(NavNode)) as NavNode[];
		// Iterate through all the nodes
		for (int i = 0; i < allNodes.Length; i++){
			NavNode currentNode = allNodes[i];
			// If the current node is in the bounds of the NavArea, add it to the area nodes
			if (areaCollider.bounds.Contains(currentNode.transform.position)){
				Debug.Log ("Adding node");
				m_areaNodes.Add(currentNode);
			}
		}
		// Iterate through all the nodes in the NavArea and intialize them
		for (int i = 0; i < m_areaNodes.Count; i++){
			m_areaNodes[i].InitializeNode(m_areaNodes);
		}
		Debug.Log("Area nodes length = " + m_areaNodes.Count);
		/*
		List<NavNode> path = AStarSearch(start, dest);
		for (int i = 0; i < path.Count; i++){
			Debug.Log ("Node ID = " + path[i].nodeId);
		}
		Debug.Log("Path length = " + path.Count.ToString());
		*/
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// Finds a path between the start node and the destination node using A* pathfinding algorithm
	public List<NavNode> AStarSearch(NavNode start, NavNode destination){
		// Initialize the necessary lists and variables for A* pathfinding
		List<NavNode> all = new List<NavNode>();
		all.Add(start);
		List<NavNode> closed = new List<NavNode>();
		List<NavNode> open = new List<NavNode>(all);
		List<NavNode> path = new List<NavNode>();
		float totalCost = 0;
		float currentBest = 0;

		Debug.Log("Starting AStar");

		while (open[0] != destination){
			Debug.Log ("open[0] ID = " + open[0].nodeId);
			NavNode lowest = open[0];

			// Find the node with the lowest value in open
			for (int i = 0; i < open.Count; i++){
				NavNode currentNode = open[i];
				if (currentNode.GetCostToHere() < lowest.GetCostToHere()){
					lowest = currentNode;
				}
			}

			// Remove the current lowest from open and add it to the closed since it is being visited
			open.Remove (lowest);
			closed.Add (lowest);

			// Iterate through the neighbors of the selected node
			for (int i = 0; i < lowest.GetNeighborNodes().Count; i++){
				Debug.Log ("Going through neighbors of node " + lowest.nodeId.ToString ());
				NavNode currentNeighbor = lowest.GetNeighborNodes()[i];
				// Calculate the cost as the distance between the current node and the current neighbor
				float cost = lowest.GetCostToHere() + Vector3.Distance(lowest.transform.position, currentNeighbor.transform.position);
				if (open.Contains(currentNeighbor) && cost < currentNeighbor.GetCostToHere()) {
					Debug.Log ("Removing neighbor from open");
					open.Remove(currentNeighbor);
				}
				if (closed.Contains(currentNeighbor) && cost < currentNeighbor.GetCostToHere()) {
					Debug.Log ("Removing neighbor from closed");
					closed.Remove(currentNeighbor);
				}
				if (!open.Contains(currentNeighbor) && !closed.Contains(currentNeighbor)) {
					currentBest = cost;
					Debug.Log ("Open size = " + open.Count.ToString());
					// If the open list is empty, add the current neighbor to the open list
					if (open.Count == 0){
						Debug.Log("Adding neighbor " + currentNeighbor.nodeId.ToString () + " to open");
						open.Add(currentNeighbor);
					}
					// Otherwise, place the node in the open list at the appropriate position according to its cost
					else {
						bool addedToOpen = false;
						for (int j = 0; j < open.Count; j++){
							if (cost < open[j].GetCostToHere()){
								open.Insert(j, currentNeighbor);
								Debug.Log("Adding neighbor " + currentNeighbor.nodeId.ToString () + " to open");
								addedToOpen = true;
								break;
							}
						}
						if (!addedToOpen){
							open.Insert (open.Count, currentNeighbor);
						}
					}
					
					// Set the cost and parent of the current neighbor
					currentNeighbor.SetCostToHere(cost);
					currentNeighbor.SetParentNode(lowest);
				}
			}
		}

		// Once finished, determine what the path is by looking at the parent nodes and insert them into the path in the proper order
		NavNode node = destination;
		while (node.GetParentNode() != start){
			path.Insert(0, node);
			node = node.GetParentNode();
		}
		path.Insert(0, node);
		return path;
	}

	// Adds a NavNode the NavArea nodes list
	public void AddNode(NavNode nodeToAdd){
		m_areaNodes.Add(nodeToAdd);
	}

	public List<NavNode> getAreaNodes(){
		return m_areaNodes;
	}
}
