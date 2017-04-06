using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--makes this object and subobjects dynamic (non kinematic) if it's hit by player
//--requires a rigidbody

public class DynamicOnHit : MonoBehaviour {

	public GameObject body;
	private Rigidbody2D rb;
	private BoxCollider2D collider;
	public Renderer rend;
	public Vector2 initialPosition;
	public Rigidbody2D[] childRbs;
	public Renderer[] childRends;
	public bool doesReset = false;
	public bool alive = true;

	// Use this for initialization
	void Start () {
		rb = body.GetComponent<Rigidbody2D>();
		collider = GetComponent<BoxCollider2D>();
		childRbs = GetComponentsInChildren<Rigidbody2D>( ) as Rigidbody2D[];
		// childRends = GetComponentsInChildren<Renderer>( ) as Renderer[];
		// initialPosition = transform.localPosition;
		rend = GetComponent<Renderer>();

	}
	
	void OnTriggerEnter2D(Collider2D other) {

		// Debug.Log("other = "+other.name);

        if (other.tag == "DynamicLand" || other.tag == "PlayerVehicle"){

            Debug.Log(other.name+"collided with "+gameObject.name);

            if(alive){

            	alive = false;
        		
		        MakeDynamic();

				collider.enabled = false;

		        // Vector2 force = other.GetComponent<Rigidbody2D>().velocity * 10;
		        // Debug.Log("hit force = "+force);

		        // rb.AddForce(force, ForceMode2D.Impulse);

        		Invoke("StartFlashing",2.5f);
		       
            }
        }
        
    }

    void StartFlashing(){
    	Debug.Log("start flashing");
    	StartCoroutine("Blink");
    }

    IEnumerator Blink(){
    	rend.enabled = false;
    	yield return new WaitForSeconds(0.02f);
    	rend.enabled = true;
    	yield return new WaitForSeconds(0.02f);
    	rend.enabled = false;
    	yield return new WaitForSeconds(0.02f);
    	rend.enabled = true;
    	yield return new WaitForSeconds(0.02f);
    	rend.enabled = false;
    	Destroy(gameObject);
    }

    void MakeDynamic(){
		rb.isKinematic = false;

    	//--make subobjects dynamic 
    	foreach( Rigidbody2D childRb in childRbs ){
			// childRb.gameObject.transform.parent = parentObj.transform;
            childRb.isKinematic = false;
            Debug.Log("make "+childRb+" not kinematic");
           
        }
    }
}
