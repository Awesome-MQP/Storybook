using UnityEngine;
using System.Collections.Generic;

public class NavNode : MonoBehaviour {
	
	public int nodeId;
	List<NavNode> neighborNodes = new List<NavNode>();
	float costToHere = 0;
	NavNode parentNode;
	bool isStart = true;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

	}
	
	// Uses raycasting to find all of the neigboring nodes and adds them to its neighbor list
	void findNeighbors(){
		NavNode[] allNodes = FindObjectsOfType(typeof(NavNode)) as NavNode[];
		// Iterate through all of the NavNodes
		for(int i = 0; i < allNodes.Length; i++){
			Vector3 currentPosition = transform.position;
			NavNode otherNode = allNodes[i];
			Vector3 otherNodePosition = otherNode.transform.position;
			// If the other node isn't this node, check to see if there is a clear path to it
			if (otherNode != this){
				Vector3 direction = otherNodePosition - currentPosition;
				float distance = Vector3.Distance(currentPosition, otherNodePosition);
				Ray toOtherNodeRay = new Ray(currentPosition, direction);
				// If nothing is hit by the raycast, add the other node to the neighbor list
				if (!Physics.Raycast(toOtherNodeRay, distance)){
					neighborNodes.Add(otherNode);
				}
			}
		}
	}

	// Initialize the node by finding its neighbors and executing other start up code
	public void initializeNode(){
		// At startup, find the neighbors of the node
		findNeighbors();
		Debug.Log ("Node " + nodeId.ToString () + " has " + neighborNodes.Count.ToString () + " neighbors");
		for (int i = 0; i < neighborNodes.Count; i++){
			Debug.Log ("Node " + nodeId.ToString() + " neighbor = " + neighborNodes[i].nodeId);
		}
		NavArea navArea = FindObjectOfType(typeof(NavArea)) as NavArea;
		navArea.addNode(this);
	}

	// Getter and setter for costToHere
	public float getCostToHere(){
		return costToHere;
	}

	public void setCostToHere(float newCost){
		costToHere = newCost;
	}

	// Getter and setter for parentNode
	public NavNode getParentNode(){
		return parentNode;
	}

	public void setParentNode(NavNode newParent){
		parentNode = newParent;
	}

	// Getter for neighborNodes list
	public List<NavNode> getNeighborNodes(){
		return neighborNodes;
	}
}
