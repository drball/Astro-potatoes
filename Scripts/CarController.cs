using UnityEngine;
using System.Collections;

public class CarController : MonoBehaviour {

	public WheelJoint2D FrontWheel;
	public WheelJoint2D BackWheel;

	JointMotor2D motorFront;
	JointMotor2D motorBack;

	public float speedForward;
	public float speedBackward;
	public float torqueForward;
	public float torqueBackward;
	public bool TractionFront = true;
	public bool TractionBack = true;
	public Vector2 respawnPos;
	public bool active = false;
	public Renderer[] childRends;
	public TouchControls TouchControls;

	private Vector2 initialPos;
	public Renderer rend;

	private Vector2 frontWheelInitialPos;
	private Vector2 backWheelInitialPos;

	// private Rigidbody2D rb;

	// Use this for initialization
	void Start () {
		// rb = GetComponent<Rigidbody2D>();
		initialPos = transform.localPosition;
		respawnPos = initialPos;

		childRends = GetComponentsInChildren<Renderer>( ) as Renderer[];

		frontWheelInitialPos = FrontWheel.transform.localPosition;
		backWheelInitialPos = BackWheel.transform.localPosition;
	}

	
	// Update is called once per frame
	void Update () {

		// Debug.Log("car vel = "+rb.velocity);

		if(active){

			if(TouchControls.RightPressed) {

				if(TractionFront){
					motorFront.motorSpeed = speedForward*-1;
					motorFront.maxMotorTorque = torqueForward;
					FrontWheel.motor = motorFront;
				}
				
				if(TractionBack){
					motorBack.motorSpeed = speedForward*-1;
					motorBack.maxMotorTorque = torqueForward;
					BackWheel.motor = motorBack;
				}

				

			} else if (TouchControls.LeftPressed){

				if(TractionFront){
					motorFront.motorSpeed = speedBackward*-1;;
					motorFront.maxMotorTorque = torqueBackward;
					FrontWheel.motor = motorFront;
				}

				if(TractionBack){
					motorBack.motorSpeed = speedBackward*-1;;
					motorBack.maxMotorTorque = torqueBackward;
					BackWheel.motor = motorBack;
				}

			} else {
				BackWheel.useMotor = false;
				FrontWheel.useMotor = false;
			}

		}

	}


	public void Respawn(){
		Debug.Log("Respawn player to "+respawnPos);

		//--reset the wheels first, as this affects the entire pos for some reason
		FrontWheel.transform.localPosition = frontWheelInitialPos;
		BackWheel.transform.localPosition = backWheelInitialPos;

		transform.position = respawnPos;
		transform.rotation = Quaternion.identity;

		
	}

	void UpdateRespawnPoint(Vector2 newPos){
		Debug.Log("updating respawnPos to "+newPos);
		respawnPos = newPos;
	}

	public void ActivateVehicle(){
		active = true;
		StartCoroutine("Blink");
		Debug.Log("activating "+gameObject.name);
	}

    void Hide(){
		//-disable renderer of all children
    	foreach( Renderer childRend in childRends ){
            childRend.enabled = false;
            // Debug.Log("make "+childRend+" hidden");
        }
        rend.enabled = false;
    }

    void Show(){
		//-enable renderer of all children
    	foreach( Renderer childRend in childRends ){
            childRend.enabled = true;
            // Debug.Log("make "+childRend+" show");
        }
        rend.enabled = true;
    }

    IEnumerator Blink(){
    	// Debug.Log("starting blink");
    	Hide();
    	yield return new WaitForSeconds(0.05f);
    	Show();
    	yield return new WaitForSeconds(0.05f);
    	Hide();
    	yield return new WaitForSeconds(0.05f);
    	Show();
    	yield return new WaitForSeconds(0.05f);
    	Hide();
    	yield return new WaitForSeconds(0.05f);
    	Show();
    }

    void DeactivateVehicle (){
    	active = false;
    }


}
