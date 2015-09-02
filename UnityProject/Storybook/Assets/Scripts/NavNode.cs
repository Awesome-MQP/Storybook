using UnityEngine;
using System.Collections.Generic;

public class NavNode : MonoBehaviour {

	public bool isStarted = false;
	public int nodeId;
	List<NavNode> neighborNodes = new List<NavNode>();
	float costToHere = 0;
	NavNode parentNode;

	// Use this for initialization
	void Start () {
		findNeighbors();
		Debug.Log ("Node " + nodeId.ToString () + " has " + neighborNodes.Count.ToString () + " neighbors");
		isStarted = true;
		NavArea navArea = FindObjectOfType(typeof(NavArea)) as NavArea;
		navArea.addNode(this);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void findNeighbors(){
		NavNode[] allNodes = FindObjectsOfType(typeof(NavNode)) as NavNode[];
		for(int i = 0; i < allNodes.Length; i++){
			Vector3 currentPosition = transform.position;
			NavNode otherNode = allNodes[i];
			if (otherNode != this){
				RaycastHit hit;
				Ray toOtherNodeRay = new Ray(transform.position, otherNode.transform.position);
				Vector3	otherNodePosition = otherNode.transform.position;
				if (!Physics.Raycast(toOtherNodeRay, out hit)){
					neighborNodes.Add(otherNode);
				}
			}
		}
	}

	public float getCostToHere(){
		return costToHere;
	}

	public void setCostToHere(float newCost){
		costToHere = newCost;
	}

	public NavNode getParentNode(){
		return parentNode;
	}

	public void setParentNode(NavNode newParent){
		parentNode = newParent;
	}

	public List<NavNode> getNeighborNodes(){
		return neighborNodes;
	}
}
