using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterMovement : MonoBehaviour {

	public bool active = true;
	public float speed = 1f;
	private Rigidbody2D rb;
	private Vector3 initialScale;
	public TouchControls TouchControls;
	private Vector3 reverseScale;
	public Animator animator;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D>();
		initialScale = transform.localScale;
		Debug.Log("initial scale = "+initialScale.y);
		reverseScale = new Vector3(-initialScale.x, initialScale.y, initialScale.z);
	}
	
	// Update is called once per frame
	void Update () {

		// Debug.Log("car vel = "+rb.velocity);

		if(active){

			if(TouchControls.RightPressed) {

				rb.velocity = new Vector2(speed, 0);
				transform.localScale = initialScale;
				// animator.SetTrigger("MakeWalk");

			} else if (TouchControls.LeftPressed){
				rb.velocity = new Vector2(-speed, 0);
				transform.localScale = reverseScale;
				// animator.SetTrigger("MakeWalk");
			} else {
				// animator.SetTrigger("MakeIdle");
			}

			animator.SetFloat("Speed", Mathf.Abs(rb.velocity.magnitude));


		
		}
	

	}
}
