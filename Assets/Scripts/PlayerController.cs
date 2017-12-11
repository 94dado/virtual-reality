using UnityEngine;
using Leap;
using System.Collections;

/*LIST OF ALL GESTURES:
 * 1) Create fire-ball:            closed hand, palm facing up                                          E
 * 2) Shoot fire-ball:             open palm facing up                                                  R
 * 3) Create thunder:              closed hand, palm facing down                                        Q
 * 4) Shoot thunder:               open palm facing down                                                tab
 * 5) Missile:                     thumb directed up, index directed forward                            F
 * 6) Flamethrower:                open palm facing forward                                             Space
 * 7) Aim enemy:                   index pointing (directed forward)                   					LeftMouse
 */

/* -1 up, 1 down, left 1, right -1, 1 forward, -1 backward*/
public enum Gesture {
    none,createFire,createThunder,missile,flamethrower
}

public class PlayerController : MonoBehaviour {
    //public
    public GameObject particleContainer;
    public Transform handController;					//transform for the object that rapresents the rotation of the Leap Motion
    public GameObject marker;
    public LayerMask enemyMask;
    public GameObject rocketCollider;
    public ParticleSystem[] palmParticleEffects;
    public ParticleSystem[] distantParticleEffects;
    public bool useLeapMotion = true;
    public float waitTime = 0.5f;						//waiting time for leap motion frame handling
	public float tolleranceRecognition = 0.2f;			//tollerance to check if the hand/fingers are in the correct directions

	//aiming corrections
	//public float correctionX, correctionY, correctionZ;

    //particles numbers
    const int fire_n = 0;
    const int thunder_n = 1;
    const int missile_n = 2;
    const int flamethrower_n = 3;

    //private attributes
    Controller controller;              				//for leap
    bool aiming;             							//i'm aiming an enemy?
    ParticleSystem particle;
    Transform aimEnemy;									//aimed enemy
    GameObject mark;									//the marker to show the marked enemy
    Gesture oldGesture = Gesture.none;					//the old gesture, to improve Leap Motion controls
    Gesture gst = Gesture.none;
	Vector3 up,left,right,down,forward,backward;		//Leap Motion directions, used to recognize gestures	

    //boolean to know if a particle is generated
    bool[] particleAlive = { false, false, false, false};     //0 fire, 1 thunder, 2 missile, 3 flamethrower

    //code to create/destroy particle effects without the leap motion
    void CreateParticle(int particleNumber) {
        if (particleAlive[particleNumber]) {
            DestroyParticle();
            particleAlive[particleNumber] = !particleAlive[particleNumber];
        }
        else {
            if (particle == null || !particle.IsAlive()) {
                CreatePalmParticle(particleNumber);
                if(particleNumber!=missile_n) particleAlive[particleNumber] = !particleAlive[particleNumber];
            }
        }
        
    }

	//update axis
	void UpdateAxis(){
		up = handController.up;
		down = - handController.up;
		forward = handController.forward;
		backward = -handController.forward;
		right = handController.right;
		left = - handController.right;
	}

	//choose between leap motion or keyboard settings
    void Start() {
        //create the leap controller
        controller = new Controller();
        if (useLeapMotion) {
            StartCoroutine(Poller());
        }

    }

	//keyboard settings:
    private void Update() {
        /*
        Debug.Log("bttn0: " + Input.GetKeyDown(KeyCode.Joystick1Button0));
        Debug.Log("bttn1: " + Input.GetKeyDown(KeyCode.Joystick1Button1));
        Debug.Log("bttn2: " + Input.GetKeyDown(KeyCode.Joystick1Button2));
        Debug.Log("bttn3: " + Input.GetKeyDown(KeyCode.Joystick1Button3));
        Debug.Log("bttn4: " + Input.GetKeyDown(KeyCode.Joystick1Button4));
        Debug.Log("bttn5: " + Input.GetKeyDown(KeyCode.Joystick1Button5));
        Debug.Log("bttn6: " + Input.GetKeyDown(KeyCode.Joystick1Button6));
        Debug.Log("bttn7: " + Input.GetKeyDown(KeyCode.Joystick1Button7));
        Debug.Log("bttn8: " + Input.GetKeyDown(KeyCode.Joystick1Button8));
        Debug.Log("bttn9: " + Input.GetKeyDown(KeyCode.Joystick1Button9));
        Debug.Log("bttn10: " + Input.GetKeyDown(KeyCode.Joystick1Button10));
        Debug.Log("bttn11: " + Input.GetKeyDown(KeyCode.Joystick1Button11));
        Debug.Log("bttn12: " + Input.GetKeyDown(KeyCode.Joystick1Button12));
        Debug.Log("bttn13: " + Input.GetKeyDown(KeyCode.Joystick1Button13));
        */
        
        //when i'm not using leap motion, everything is on the keyboard
        if (!useLeapMotion){
            if (gst != Gesture.createFire && Input.GetKeyDown(KeyCode.Joystick1Button0)) {
                gst = Gesture.createFire;
                CreateParticle(fire_n);
            }
            else if (Input.GetKeyDown(KeyCode.Joystick1Button0)) {
                if (gst == Gesture.createFire) {
                    Attack(fire_n);
                    gst = Gesture.none;
                }
                
            }
            else if (gst != Gesture.createThunder && Input.GetKeyDown(KeyCode.Joystick1Button1)) {
                gst = Gesture.createThunder;
                CreateParticle(thunder_n);
            }
            else if (Input.GetKeyDown(KeyCode.Joystick1Button1)) {
                if (gst == Gesture.createThunder) {
                    Attack(thunder_n);
                    gst = Gesture.none;
                }
            }
            else if (Input.GetKeyDown(KeyCode.Joystick1Button2)) {
                gst = Gesture.none;
                CreateParticle(missile_n);
            }
            else if (Input.GetKeyDown(KeyCode.Joystick1Button3)) {
                gst = Gesture.none;
                CreateParticle(flamethrower_n);
            }
            else if (Input.GetKeyDown(KeyCode.Joystick1Button10)) {
                gst = Gesture.none;
                KeyboardAim();
            }
        }
    }

	//Leap Motion settings:
    IEnumerator Poller() {
        while (true) {
            yield return new WaitForSeconds(waitTime);
            //get frame and gestures
            Frame frame = controller.Frame();
            //search for a custom gesture
			if (frame.Hands.Count > 0) {
				//update axis of the leap rotation
				UpdateAxis();
				//try to recognize gestures
				SearchCustomGesture (frame);
			}
            else {
				//no hands: remove any active effect
                DestroyParticle();
            }
        }
    }

	//check if a vector is really similar to another
	bool VectorEquals(Vector3 a, Vector3 b){
		bool toRet = true;
		float diffx = Mathf.Abs(b.x - a.x),
		diffy = Mathf.Abs(b.y - a.y),
		diffz = Mathf.Abs(b.z - a.z);
		if (diffx > tolleranceRecognition) {
			toRet = false;
		} else if (diffy > tolleranceRecognition) {
			toRet = false;
		} else if (diffz > tolleranceRecognition) {
			toRet = false;
		}
		//Debug.Log ("same: " + toRet);
		return toRet;
	}

    //return true if the player is making a gun with his fingers (only two finger: one finger directed up and the other forward)
    bool FingersLikeGun(FingerList pointingFingers) {
        //check if there are only 2 fingers
        if (pointingFingers.Count != 2) return false;
        //check if the 2 fingers are thumb and index
        Finger thumb, index;
        thumb = index = null;
        if (pointingFingers[0].Type == Finger.FingerType.TYPE_THUMB)
            thumb = pointingFingers[0];
        else if (pointingFingers[1].Type == Finger.FingerType.TYPE_THUMB)
            thumb = pointingFingers[1];
        if (pointingFingers[0].Type == Finger.FingerType.TYPE_INDEX)
            index = pointingFingers[0];
        else if (pointingFingers[1].Type == Finger.FingerType.TYPE_INDEX)
            index = pointingFingers[1];
        if (thumb == null || index == null) return false;
		return true;
    }

    //get data from leap and check if a custom gesture is triggered
    void SearchCustomGesture(Frame frame) {
        //get the right hand
        Hand hand = frame.Hands.Rightmost;
        //get all the pointing fingers
        FingerList pointingFingers = hand.Fingers.Extended();
        //get the direction of the palm
		Vector3 palmDirection = hand.PalmNormal.ToUnity();
		//rotate the normal of -90 degrees on x axis because the Leap Motion is head mounted (aka rotated of 90 degrees on x axis)
		palmDirection = Quaternion.Euler (-90, 0, 90) * palmDirection;
		//closed hand, palm facing up
		if (pointingFingers.Count == 0 && VectorEquals(palmDirection, up)) {
            if(oldGesture != Gesture.createFire) {
                Debug.Log("Create fire-ball");
                oldGesture = Gesture.createFire;
                CreateParticle(fire_n);
            }
        }
        //open palm facing up
		else if (pointingFingers.Count == 5 && VectorEquals(palmDirection, up)) { 
            Debug.Log("Shoot fire-ball");
            Attack(fire_n);
        }
        //closed hand, palm facing down
		else if (pointingFingers.Count == 0 && VectorEquals(palmDirection, down)) {
            if (oldGesture != Gesture.createThunder) {
                Debug.Log("Create thunder");
                oldGesture = Gesture.createThunder;
                CreateParticle(thunder_n);
            }
        }
        //open palm facing down
		else if (pointingFingers.Count == 5 && VectorEquals(palmDirection, down)) {
            Debug.Log("Shoot thunder");
            Attack(thunder_n);
        }
        //thumb directed up, index directed forward
        else if (FingersLikeGun(pointingFingers)) {
            if(oldGesture != Gesture.missile) {
                Debug.Log("Shoot missile");
                oldGesture = Gesture.missile;
                CreateParticle(missile_n);
            }

        }
        //open palm facing forward
		else if(pointingFingers.Count == 5 && VectorEquals(palmDirection, forward)) {
            if(oldGesture != Gesture.flamethrower) {
                Debug.Log("Flamethrower");
                oldGesture = Gesture.flamethrower;
                CreateParticle(flamethrower_n);
            }

        }
        //index pointing (directed forward)
        else if (pointingFingers.Count == 1 && pointingFingers[0].Type == Finger.FingerType.TYPE_INDEX) {
            Debug.Log("Aiming");
			Aim(handController.position, handController.forward);
        }
        //none of the wanted gestures
        else {
            //reset the old gesture
            oldGesture = Gesture.none;
        }

    }

    // create the particle effect and attachs it to the hand
    void CreatePalmParticle(int particleNumber) {
        // TODO: change the transform in the right hand
        GameObject container;
        // if  rocket
        if (particleNumber == 2) {
            container = Instantiate(particleContainer, handController.position, handController.rotation, handController);
            //automaticcally destroy the rocket
            ParticleSystem rocket = Instantiate(palmParticleEffects[particleNumber], container.transform.position, container.transform.rotation, container.transform);
            Destroy(rocket, 2f);
            GameObject newRocketCollider = Instantiate(rocketCollider, handController.position, handController.rotation, handController);
            float speed = rocket.main.startSpeed.constant;
            newRocketCollider.GetComponent<Rigidbody>().velocity = newRocketCollider.transform.forward * speed;
            Destroy(newRocketCollider, 2f);
        }
        // if flamethrower
        else if (particleNumber == 3) {
            container = Instantiate(particleContainer, handController.position, handController.rotation, handController);
            particle = Instantiate(palmParticleEffects[particleNumber], container.transform.position, container.transform.rotation, container.transform);
        }
        else {
            container = Instantiate(particleContainer, handController.position, handController.rotation, handController);
            particle = Instantiate(palmParticleEffects[particleNumber], container.transform.position, container.transform.rotation, container.transform);
        }
    }

    //destroy the particle
    void DestroyParticle() {
        if (particle != null && particle.IsAlive()) {
            Destroy(particle.transform.parent.gameObject);
        }
    }

    // generate the explosion
    void Attack(int particleNumber) {
        // if some enemy is aimed
        if (aiming && particleAlive[particleNumber]) {
            // create particle
            Destroy(Instantiate(distantParticleEffects[particleNumber], aimEnemy.position, aimEnemy.rotation).gameObject, 4f);
            // hit enemy
            bool dead = aimEnemy.GetComponent<AnimalController>().TakeDamage();
            //remove marker if the enemy was killed by the shot
            if (dead) {
                aiming = false;
            }
            //remove particle effect
            CreateParticle(particleNumber);
        }
    }

    // aim a target
    void KeyboardAim() {
        Aim(handController.position, handController.forward);
    }

    void Aim(Vector3 fingerPosition, Vector3 direction) {
        RaycastHit hit;
        Debug.DrawRay(fingerPosition, direction * 200f, Color.red, 5f);
        // TODO: use the finger not a transform
        if (Physics.Raycast(fingerPosition, direction, out hit, Mathf.Infinity, enemyMask)) {
            aimEnemy = hit.transform;
            // if exist
            if (mark != null) {
                Destroy(mark);
            }
            aiming = true;
            mark = Instantiate(marker, aimEnemy.position, aimEnemy.rotation, aimEnemy);
        }
    }
}
