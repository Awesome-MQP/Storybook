using UnityEngine;
using System.Collections.Generic;

public class NavArea : MonoBehaviour {

	public NavNode start;
	public NavNode dest;
	public bool isTest;

	List<NavNode> areaNodes = new List<NavNode>();

	// Use this for initialization
	void Start () {
		Debug.Log("NavArea Starting up");
		NavNode[] allNodes = FindObjectsOfType(typeof(NavNode)) as NavNode[];
		// Initialize all of the nodes
		for (int i = 0; i < allNodes.Length; i++){
			allNodes[i].initializeNode();
		}
		Debug.Log("Area nodes length = " + areaNodes.Count);
		List<NavNode> path = aStarSearch(start, dest);
		for (int i = 0; i < path.Count; i++){
			Debug.Log ("Node ID = " + path[i].nodeId);
		}
		Debug.Log("Path length = " + path.Count.ToString());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// Finds a path between the start node and the destination node using A* pathfinding algorithm
	List<NavNode> aStarSearch(NavNode start, NavNode destination){
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
				if (currentNode.getCostToHere() < lowest.getCostToHere()){
					lowest = currentNode;
				}
			}

			// Remove the current lowest from open and add it to the closed since it is being visited
			open.Remove (lowest);
			closed.Add (lowest);

			// Iterate through the neighbors of the selected node
			for (int i = 0; i < lowest.getNeighborNodes().Count; i++){
				Debug.Log ("Going through neighbors of node " + lowest.nodeId.ToString ());
				NavNode currentNeighbor = lowest.getNeighborNodes()[i];
				// Calculate the cost as the distance between the current node and the current neighbor
				float cost = lowest.getCostToHere() + Vector3.Distance(lowest.transform.position, currentNeighbor.transform.position);
				if (open.Contains(currentNeighbor) && cost < currentNeighbor.getCostToHere()) {
					Debug.Log ("Removing neighbor from open");
					open.Remove(currentNeighbor);
				}
				if (closed.Contains(currentNeighbor) && cost < currentNeighbor.getCostToHere()) {
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
							if (cost < open[j].getCostToHere()){
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
					currentNeighbor.setCostToHere(cost);
					currentNeighbor.setParentNode(lowest);
				}
			}
		}

		// Once finished, determine what the path is by looking at the parent nodes and insert them into the path in the proper order
		NavNode node = destination;
		while (node.getParentNode() != start){
			path.Insert(0, node);
			node = node.getParentNode();
		}
		path.Insert(0, node);
		return path;
	}

	// Adds a NavNode the NavArea nodes list
	public void addNode(NavNode nodeToAdd){
		areaNodes.Add(nodeToAdd);
	}
}
