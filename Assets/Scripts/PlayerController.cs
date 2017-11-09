using UnityEngine;
using Leap;

/*LIST OF ALL GESTURES:
 * Create fire-ball: open palm facing up
 * Shoot fire-ball: closed hand, palm facing down
 * Thunder: two pointing fingers (directed forward)
 * Missile: palm facing left, two fingers (one directed up, one forward)
 * Flamethrower: open palm facing forward
 * Aim enemy: one pointing fingers (directed forward)
 */

public class PlayerController : MonoBehaviour {

    //leap motion settings
    public float aimMinVelocity = 50.0f;            // mm/s
    public float aimHistorySeconds = .1f;           // s
    public float aimMinDistance = 5.0f;             // mm

    public float flameRadius = 5.0f;                // mm

    //private attributes
    Controller controller;              //for leap
    bool aiming;             //i'm aiming an enemy?

	void Start () {
        //create the leap controller
        controller = new Controller();
    }
	

	void Update () {
        //get frame and gestures
        Frame frame = controller.Frame();
        //search for a custom gesture
        if(frame.Hands.Count > 0)
            SearchCustomGesture(frame);
	}

    //get data from leap and check if a custom gesture is triggered
    void SearchCustomGesture(Frame frame) {
        //get the right hand
        Hand hand = frame.Hands.Rightmost;
        //get all the pointing fingers
        FingerList pointingFingers = hand.Fingers.Extended();
        //get the direction of the palm
        Vector palmDirection = GetVectorDirection(hand.PalmNormal);
        //TODO use the data that we have to recognize the gestures, even the "no gestures event"
        Debug.Log("Pointing Fingers: " + pointingFingers.Count);
        Debug.Log("Palm direction: " + palmDirection);
    }

    //return the rounded normal values of the palm of an hand (usefull to confront them with directions)
    Vector GetVectorDirection(Vector vec) {
        float x = Mathf.Round(vec.x);
        float y = Mathf.Round(vec.y);
        float z = Mathf.Round(vec.z);
        return new Vector(x, y, z);
    }



    // attack
    void Attack() {
        
    }
}
