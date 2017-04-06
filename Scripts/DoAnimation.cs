using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoAnimation : MonoBehaviour {

	public Animator animator;
	public float delay = 1f;
	public string animTrigger; 

	// Use this for initialization
	void Start () {
		Invoke("StartAnimation", delay);
	}
	
	void StartAnimation(){
		animator.SetTrigger(animTrigger);
	}
}
