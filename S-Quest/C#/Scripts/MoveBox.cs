using UnityEngine;
using System.Collections;

public class MoveBox : MonoBehaviour {

	public Transform Pos1;
	public Transform Pos2;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = Vector3.Slerp (Pos1.position, Pos2.position, Mathf.PingPong(Time.time*0.1f, 1.0f));
	}
}
