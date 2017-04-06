using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--starts walking 

public class WalkingNPC : MonoBehaviour {

	public Animator animator;
	public float walkingSpeed = 1;
	public enum walkingDirections {Right = 0, Left = 1} 
	public walkingDirections walkingDirection= walkingDirections.Right & walkingDirections.Left;
	public Transform RaycastEnd;
	public bool hitEdge; 
	public RagdollWhenHit RagdollScript;

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
		RagdollScript = GetComponent<RagdollWhenHit>();
		animator.SetTrigger("MakeWalk");

		// Debug.Log(gameObject.name+"walking dir="+walkingDirection);

		if(walkingDirection == walkingDirections.Left){
			// Debug.Log("start left");
			Turn();
		}
	}
	
	void Update () {


		if(RagdollScript.alive){

			transform.Translate((Vector2.right * walkingSpeed) * Time.deltaTime);

			//--raycast
			Debug.DrawLine(transform.position, RaycastEnd.position, Color.green);
			hitEdge = Physics2D.Linecast(transform.position, RaycastEnd.position, 1 << LayerMask.NameToLayer("Helper"));

			if(hitEdge){
				Turn();
			}
		}


	}

	void Turn(){
		// Debug.Log(gameObject.name+" change direction");
		transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
		walkingSpeed = -walkingSpeed;
	}

}
