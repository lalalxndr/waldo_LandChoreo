using UnityEngine;
using System.Collections;

public class Node {
	
	public int state;
 
    public Vector3 worldPosition;
	public int gridX;
	public int gridY;

	public Node parent;
	
	public Node(int _state, Vector3 _worldPos, int _gridX, int _gridY) {
		state = _state;
		worldPosition = _worldPos;
		gridX = _gridX;
		gridY = _gridY;
	}
  
}
