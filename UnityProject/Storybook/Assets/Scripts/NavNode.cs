using UnityEngine;
using System.Collections.Generic;

public class NavNode : MonoBehaviour {
	[SerializeField]
	private List<NavNode> m_neighborNodes = new List<NavNode>();

	[SerializeField]
	private float m_costToHere = -1;

	[SerializeField]
	private NavNode m_parentNode;

	// Uses raycasting to find all of the neigboring nodes and adds them to its neighbor list
	private void _findNeighbors(List<NavNode> areaNodes){

		// Iterate through all of the NavNodes
		int areaCount = areaNodes.Count;
		for(int i = 0; i < areaCount; i++){
			Vector3 currentPosition = transform.position;
			NavNode otherNode = areaNodes[i];
			Vector3 otherNodePosition = otherNode.transform.position;

			// If the other node isn't this node, check to see if there is a clear path to it
			if (otherNode != this){
				Vector3 direction = otherNodePosition - currentPosition;
				float distance = Vector3.Distance(currentPosition, otherNodePosition);
				Ray toOtherNodeRay = new Ray(currentPosition, direction);
				// If nothing is hit by the raycast, add the other node to the neighbor list
				if (!Physics.Raycast(toOtherNodeRay, distance)){
					m_neighborNodes.Add(otherNode);
				}
			}
		}
	}

	// Initialize the node by finding its neighbors and executing other start up code
	public void InitializeNode(List<NavNode> areaNodes){
		// At startup, find the neighbors of the node
		_findNeighbors(areaNodes);
	}

	// Getter and setter for costToHere
	public float GetCostToHere(){
		return m_costToHere;
	}

	public void SetCostToHere(float newCost){
		m_costToHere = newCost;
	}

	// Getter and setter for parentNode
	public NavNode GetParentNode(){
		return m_parentNode;
	}

	public void SetParentNode(NavNode newParent){
		m_parentNode = newParent;
	}

	// Getter for neighborNodes list
	public List<NavNode> GetNeighborNodes(){
		return m_neighborNodes;
	}
}
