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
 * 7) Aim enemy:                   palm facing left,index pointing (directed forward)                   LeftMouse
 */

/* -1 up, 1 down, left 1, right -1, 1 forward, -1 backward*/
public enum Gesture {
    none,createFire,createThunder,missile,flamethrower
}

public class PlayerController : MonoBehaviour {
    //public
    public GameObject particleContainer;
    public Transform handController;
    public GameObject marker;
    public LayerMask enemyMask;
    public GameObject rocketCollider;
    public ParticleSystem[] palmParticleEffects;
    public ParticleSystem[] distantParticleEffects;
    public bool useLeapMotion = true;
    public float waitTime = 0.5f;

    //particles numers
    const int fire_n = 0;
    const int thunder_n = 1;
    const int missile_n = 2;
    const int flamethrower_n = 3;

    //private attributes
    Controller controller;              //for leap
    bool aiming;             //i'm aiming an enemy?
    ParticleSystem particle;
    Transform aimEnemy;
    GameObject mark;
    Gesture oldGesture = Gesture.none;

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

    void Start() {
        //create the leap controller
        controller = new Controller();
        if (useLeapMotion) {
            StartCoroutine(Poller());
        }
    }

    private void Update() {
        //when i'm not using leap motion, everything is on the keyboard
        if (!useLeapMotion){
            if (Input.GetKeyDown("e")) {
                CreateParticle(fire_n);
            }
            else if (Input.GetKeyDown("r")) {
                Attack(fire_n);
            }
            else if (Input.GetKeyDown("q")) {
                CreateParticle(thunder_n);
            }
            else if (Input.GetKeyDown("tab")) {
                Attack(thunder_n);
            }
            else if (Input.GetKeyDown("f")) {
                CreateParticle(missile_n);
            }
            else if (Input.GetKeyDown("space")) {
                CreateParticle(flamethrower_n);
            }
            else if (Input.GetMouseButtonDown(0)) {
                KeyboardAim();
            }
        }
    }

    IEnumerator Poller() {
        while (true) {
            yield return new WaitForSeconds(waitTime);
            //get frame and gestures
            Frame frame = controller.Frame();
            //search for a custom gesture
            if (frame.Hands.Count > 0)
                SearchCustomGesture(frame);
            else {
                DestroyParticle();
            }
        }
    }

	Vector3 convertLeapToUnity(Vector3 leapToUnity){
		if (leapToUnity == Vector.Backward.ToUnity ())
			return Vector3.up;
		if (leapToUnity == Vector.Forward.ToUnity ())
			return Vector3.down;
		return Vector3.zero;
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
        //check if thumb and index are in the correct directions
        Vector3 thumb_v = GetVectorDirection(thumb.Direction),
               	index_v = GetVectorDirection(index.Direction);
		Vector3 left = Vector.Left.ToUnity(),
				forward = Vector.Forward.ToUnity();
		//Debug.Log ("left:"+left+",thumb:"+thumb_v);
		//Debug.Log ("forward:"+forward+",index:"+index_v);
		return thumb_v == left && index_v == forward;
    }

    //get data from leap and check if a custom gesture is triggered
    void SearchCustomGesture(Frame frame) {
        //get the right hand
        Hand hand = frame.Hands.Rightmost;
        //get all the pointing fingers
        FingerList pointingFingers = hand.Fingers.Extended();
        //get the direction of the palm
        Vector3 palmDirection = GetVectorDirection(hand.PalmNormal);
        //closed hand, palm facing up - create fireball
		Debug.Log("unity up "+Vector.Up.ToUnity());
		Debug.Log ("palmDirection " + palmDirection);
		if (pointingFingers.Count == 0 && convertLeapToUnity(palmDirection) == Vector.Up.ToUnity()) {
            if(oldGesture != Gesture.createFire) {
                Debug.Log("Create fire-ball");
                oldGesture = Gesture.createFire;
                CreateParticle(fire_n);
            }
        }
        //open palm facing up
		else if (pointingFingers.Count == 5 && convertLeapToUnity(palmDirection) == Vector.Up.ToUnity()) { 
            Debug.Log("Shoot fire-ball");
            Attack(fire_n);
        }
        //closed hand, palm facing down
		else if (pointingFingers.Count == 0 && convertLeapToUnity(palmDirection) == Vector.Down.ToUnity()) {
            if (oldGesture != Gesture.createThunder) {
                Debug.Log("Create thunder");
                oldGesture = Gesture.createThunder;
                CreateParticle(thunder_n);
            }
        }
        //open palm facing down
		else if (pointingFingers.Count == 5 && convertLeapToUnity(palmDirection) == Vector.Down.ToUnity()) {
            Debug.Log("Shoot thunder");
            Attack(thunder_n);
        }
        //thumb directed up, index directed forward
        else if (FingersLikeGun(pointingFingers)) {
            if(oldGesture != Gesture.missile) {
                Debug.Log("Shoot missile");
                oldGesture = Gesture.missile;
                CreatePalmParticle(missile_n);
            }

        }
        //open palm facing forward
		else if(pointingFingers.Count == 5 && convertLeapToUnity(palmDirection) == Vector.Forward.ToUnity()) {
            if(oldGesture != Gesture.flamethrower) {
                Debug.Log("Flamethrower");
                oldGesture = Gesture.flamethrower;
                CreateParticle(flamethrower_n);
            }

        }
        //palm facing left,index pointing (directed forward)
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

    //return the rounded normal values of the palm of an hand (usefull to confront them with directions)
    Vector3 GetVectorDirection(Vector vec) {
        float x = Mathf.Round(vec.x);
        float y = Mathf.Round(vec.y);
        float z = Mathf.Round(vec.z);
        return new Vector3(x, y, z);
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

    Vector3 Vector3Abs(Vector3 vec) {
        return new Vector3(Mathf.Abs(vec.x), Mathf.Abs(vec.y), Mathf.Abs(vec.z));
    }
}
