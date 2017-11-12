using UnityEngine;
using Leap;

/*LIST OF ALL GESTURES:
 * 1) Create fire-ball:            closed hand, palm facing up                                          E
 * 2) Shoot fire-ball:             open palm facing up                                                  R
 * 3) Create thunder:              closed hand, palm facing down                                        Q
 * 4) Shoot thunder:               open palm facing down                                                tab
 * 5) Missile:                     palm facing left, two fingers (one directed up, one forward)         F
 * 6) Flamethrower:                open palm facing forward                                             Space
 * 7) Aim enemy:                   one pointing fingers (directed forward)                              LeftMouse
 */

public class PlayerController : MonoBehaviour {
    //public
    public GameObject particleContainer;
    public Transform handController;
    public GameObject marker;
    public LayerMask enemyMask;
    public float markOffset;
    public ParticleSystem[] palmParticleEffects;
    public ParticleSystem[] distantParticleEffects;
    public bool useLeapMotion = true;

    //private attributes
    Controller controller;              //for leap
    bool aiming;             //i'm aiming an enemy?
    ParticleSystem particle;
    Transform aimEnemy;
    GameObject mark;

    //boolean to know if a particle is generated
    bool[] particleAlive = { false, false, false, false};     //0 fire, 1 thunder, 2 missile, 3 flamethrower

    //code to create/destroy particle effects without the leap motion
    void KeyboardCreateParticle(int particleNumber) {
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

    //code to attack without the leap motion
    void KeyboardAttack(int attack) {
        if(aiming && particleAlive[attack]) {
            Destroy(Instantiate(distantParticleEffects[attack], aimEnemy.position, Quaternion.identity).transform.parent.gameObject, 4f);
        }
    }

    void Start() {
        //create the leap controller
        controller = new Controller();
    }


    void Update() {
        if (useLeapMotion) {
            //get frame and gestures
            Frame frame = controller.Frame();
            //search for a custom gesture
            if (frame.Hands.Count > 0)
                SearchCustomGesture(frame);
        }
        //when i'm not using leap motion, everything is on the keyboard
        else {
            if (Input.GetKeyDown("e")){
                KeyboardCreateParticle(0);
            }else if (Input.GetKeyDown("r")) {
                KeyboardAttack(0);
            }else if (Input.GetKeyDown("q")) {
                KeyboardCreateParticle(1);
            }
            else if (Input.GetKeyDown("tab")) {
                KeyboardAttack(1);
            }
            else if (Input.GetKeyDown("f")) {
                KeyboardCreateParticle(2);
            }
            else if (Input.GetKeyDown("space")) {
                KeyboardCreateParticle(3);
            }
        }
        
    }

    //return true if the player is making a gun with his fingers (only two finger: one finger directed up and the other forward)
    bool FingersLikeGun(FingerList pointingFingers) {
        if (pointingFingers.Count != 2) return false;
        Vector v1 = GetVectorDirection(pointingFingers[0].Direction),
               v2 = GetVectorDirection(pointingFingers[1].Direction);
        return ( v1 == Vector.Up && v2 == Vector.Forward) || (v2 == Vector.Up && v1 == Vector.Forward);
    }

    //get data from leap and check if a custom gesture is triggered
    void SearchCustomGesture(Frame frame) {
        //get the right hand
        Hand hand = frame.Hands.Rightmost;
        //get all the pointing fingers
        FingerList pointingFingers = hand.Fingers.Extended();
        //get the direction of the palm
        Vector palmDirection = GetVectorDirection(hand.PalmNormal);

        //closed hand, palm facing up - create fireball
        if (pointingFingers.Count == 0 && palmDirection == Vector.Up) {
            Debug.Log("Create fire-ball");
            CreatePalmParticle(0);
        }
        //open palm facing up - shoot fireball
        else if (pointingFingers.Count == 5 && palmDirection == Vector.Up) {
            //TODO shoot fireball
            Debug.Log("Shoot fire-ball");
            Attack(0);
        }
        //closed hand, palm facing down - create thunder
        else if (pointingFingers.Count == 0 && palmDirection == Vector.Down) {
            //TODO create thunder
            Debug.Log("Create thunder");
            CreatePalmParticle(1);
        }
        //open palm facing down - shoot thunder
        else if (pointingFingers.Count == 5 && palmDirection == Vector.Down) {
            //TODO launch thunder
            Debug.Log("Shoot thunder");
            Attack(1);
        }
        //palm facing left, two fingers - missile
        else if(FingersLikeGun(pointingFingers) && palmDirection == Vector.Left) {
            //TODO shoot missile
            Debug.Log("Shoot missile");
            CreatePalmParticle(2);
        }
        //open palm facing forward - flamethrower
        else if(pointingFingers.Count == 5 && palmDirection == Vector.Forward) {
            //TODO start flamethrowing
            Debug.Log("Flamethrower");
            CreatePalmParticle(3);
        }
        //one pointing finger forward - aim
        else if (pointingFingers.Count == 1 && palmDirection == Vector.Left) {
            //TODO aim
            Debug.Log("Aiming");
            Aim();
        }

    }

    //return the rounded normal values of the palm of an hand (usefull to confront them with directions)
    Vector GetVectorDirection(Vector vec) {
        float x = Mathf.Round(vec.x);
        float y = Mathf.Round(vec.y);
        float z = Mathf.Round(vec.z);
        return new Vector(x, y, z);
    }

    // create the particle effect and attachs it to the hand
    void CreatePalmParticle(int particleNumber) {
        // TODO: change the transform in the right hand
        GameObject container;
        // if flamethrower and rocket
        if (particleNumber >= 2) {
            container = Instantiate(particleContainer, Vector3.forward, Quaternion.identity, handController);
        }
        else {
            container = Instantiate(particleContainer, Vector3.zero, Quaternion.identity, handController);
        }
        particle = Instantiate(palmParticleEffects[particleNumber], Vector3.zero, Quaternion.identity, container.transform);
    }

    //destroy the particle
    void DestroyParticle() {
        if (particle.IsAlive()) {
            Destroy(particle.transform.parent.gameObject);
        }
    }

    // generate the explosion
    void Attack(int particleNumber) {
        // if some enemy is aimed
        if (aiming && particleAlive[particleNumber]) {
            Destroy(Instantiate(distantParticleEffects[particleNumber], aimEnemy.position, Quaternion.identity).transform.parent.gameObject, 4f);
        }
    }

    // aim a target
    void Aim() {
        RaycastHit hit;
        // TODO: use the finger not a transform
        if (Physics.Raycast(handController.position, Vector3.forward, out hit, Mathf.Infinity, enemyMask)) {
            aimEnemy = hit.transform;
            // if exist
            if (mark != null) {
                Destroy(mark);
            }
            aiming = true;
            mark = Instantiate(marker, new Vector3(aimEnemy.position.x, aimEnemy.position.y + markOffset, aimEnemy.position.z), Quaternion.identity, aimEnemy);
        }
    }
}
