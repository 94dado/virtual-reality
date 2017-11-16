using UnityEngine;
using Leap;
using System.Collections;

/*LIST OF ALL GESTURES:
 * 1) Create fire-ball:            closed hand, palm facing up                                          E
 * 2) Shoot fire-ball:             open palm facing up                                                  R
 * 3) Create thunder:              closed hand, palm facing down                                        Q
 * 4) Shoot thunder:               open palm facing down                                                tab
 * 5) Missile:                     palm facing left, two fingers (one directed up, one forward)         F
 * 6) Flamethrower:                open palm facing forward                                             Space
 * 7) Aim enemy:                   one pointing fingers (directed forward)                              LeftMouse
 */

public enum Gesture {
    none,createFire,shootFire,createThunder,shootThunder,missile,flamethrower
}

public class PlayerController : MonoBehaviour {
    //public
    public GameObject particleContainer;
    public Transform handController;
    public Transform camera;
    public GameObject marker;
    public LayerMask enemyMask;
    public GameObject rocketCollider;
    public ParticleSystem[] palmParticleEffects;
    public ParticleSystem[] distantParticleEffects;
    public bool useLeapMotion = true;
    public float waitTime = 0.5f;

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
                particleAlive[particleNumber] = !particleAlive[particleNumber];
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
                CreateParticle(0);
            }
            else if (Input.GetKeyDown("r")) {
                Attack(0);
            }
            else if (Input.GetKeyDown("q")) {
                CreateParticle(1);
            }
            else if (Input.GetKeyDown("tab")) {
                Attack(1);
            }
            else if (Input.GetKeyDown("f")) {
                CreateParticle(2);
            }
            else if (Input.GetKeyDown("space")) {
                CreateParticle(3);
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

    //return true if the player is making a gun with his fingers (only two finger: one finger directed up and the other forward)
    bool FingersLikeGun(FingerList pointingFingers) {
        if (pointingFingers.Count != 2) return false;
        Vector3 v1 = GetVectorDirection(pointingFingers[0].Direction),
               v2 = GetVectorDirection(pointingFingers[1].Direction);
        return ( v1 == Vector.Up.ToUnity() && v2 == Vector.Forward.ToUnity()) || (v2 == Vector.Up.ToUnity() && v1 == Vector.Forward.ToUnity());
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
        if (pointingFingers.Count == 0 && palmDirection == Vector.Up.ToUnity()) {
            if(oldGesture != Gesture.createFire) {
                Debug.Log("Create fire-ball");
                oldGesture = Gesture.createFire;
                CreateParticle(0);
            }
        }
        //open palm facing up - shoot fireball
        //TODO
        else if (pointingFingers.Count == 5 && palmDirection == Vector.Up.ToUnity()) { 
            Debug.Log("Shoot fire-ball");
            Attack(0);
        }
        //closed hand, palm facing down - create thunder
        else if (pointingFingers.Count == 0 && palmDirection == Vector.Down.ToUnity()) {
            if (oldGesture != Gesture.createThunder) {
                Debug.Log("Create thunder");
                oldGesture = Gesture.createThunder;
                CreateParticle(1);
            }
        }
        //open palm facing down - shoot thunder
        //TODO
        else if (pointingFingers.Count == 5 && palmDirection == Vector.Down.ToUnity()) {
            Debug.Log("Shoot thunder");
            Attack(1);
        }
        //palm facing left, two fingers - missile
        //TODO
        else if(FingersLikeGun(pointingFingers) && palmDirection == Vector.Left.ToUnity()) {
            if(oldGesture != Gesture.missile) {
                Debug.Log("Shoot missile");
                oldGesture = Gesture.missile;
                CreatePalmParticle(2);
            }

        }
        //open palm facing forward - flamethrower
        else if(pointingFingers.Count == 5 && palmDirection == Vector.Forward.ToUnity()) {
            if(oldGesture != Gesture.flamethrower) {
                Debug.Log("Flamethrower");
                oldGesture = Gesture.flamethrower;
                CreateParticle(3);
            }

        }
        //one pointing finger forward - aim
        else if (pointingFingers.Count == 1 && palmDirection == Vector.Left.ToUnity()) {
            Debug.Log("Aiming");
            Aim(handController.position, handController.forward);
        }
        //none of the wanted gestures
        else {
            oldGesture = Gesture.none;          //reset the old gesture
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
            aimEnemy.GetComponent<AnimalController>().TakeDamage();
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
