using UnityEngine;
using System.Collections.Generic;

public class NavArea : MonoBehaviour {

	public NavNode start;
	public NavNode dest;

	List<NavNode> areaNodes = new List<NavNode>();

	// Use this for initialization
	void Start () {
		Debug.Log("NavArea Starting up");
		bool areAllNodesStarted = false;
		NavNode[] allNodes = FindObjectsOfType(typeof(NavNode)) as NavNode[];
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

	List<NavNode> aStarSearch(NavNode start, NavNode destination){
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

			for (int i = 0; i < open.Count; i++){
				NavNode currentNode = open[i];
				if (currentNode.getCostToHere() < lowest.getCostToHere()){
					lowest = currentNode;
				}
			}

			open.Remove (lowest);
			closed.Add (lowest);

			for (int i = 0; i < lowest.getNeighborNodes().Count; i++){
				Debug.Log ("Going through neighbors of node " + lowest.nodeId.ToString ());
				NavNode currentNeighbor = lowest.getNeighborNodes()[i];
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
					if (open.Count == 0){
						Debug.Log("Adding neighbor to open");
						open.Add(currentNeighbor);
					}
					else {
						for (int j = 0; j < open.Count; j++){
							if (cost < open[j].getCostToHere()){
								open.Insert(j, currentNeighbor);
								break;
							}
						}
					}
					currentNeighbor.setCostToHere(cost);
					currentNeighbor.setParentNode(lowest);
				}
			}
		}

		NavNode node = destination;
		while (node.getParentNode() != start){
			path.Insert(0, node);
			node = node.getParentNode();
		}
		path.Insert(0, node);
		return path;
	}

	public void addNode(NavNode nodeToAdd){
		areaNodes.Add(nodeToAdd);
	}
}
