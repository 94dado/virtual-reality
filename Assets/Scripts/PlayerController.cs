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
    bool useLeapMotion = true;

    //private attributes
    Controller controller;              //for leap
    bool aiming;             //i'm aiming an enemy?

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
            //TODO write code to test every action that the player can do
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
        //TODO use the data that we have to recognize the gestures, even the "no gestures event"

        //closed hand, palm facing up - create fireball
        if (pointingFingers.Count == 0 && palmDirection == Vector.Up) {
            Debug.Log("Create fire-ball");
            //TODO create fireball
        }
        //open palm facing up - shoot fireball
        else if (pointingFingers.Count == 5 && palmDirection == Vector.Up) {
            //TODO shoot fireball
            Debug.Log("Shoot fire-ball");
        }
        //closed hand, palm facing down - create thunder
        else if (pointingFingers.Count == 0 && palmDirection == Vector.Down) {
            //TODO create thunder
            Debug.Log("Create thunder");
        }
        //open palm facing down - shoot thunder
        else if (pointingFingers.Count == 5 && palmDirection == Vector.Down) {
            //TODO launch thunder
            Debug.Log("Shoot thunder");
        }
        //palm facing left, two fingers - missile
        else if(FingersLikeGun(pointingFingers) && palmDirection == Vector.Left) {
            //TODO shoot missile
            Debug.Log("Shoot missile");
        }
        //open palm facing forward - flamethrower
        else if(pointingFingers.Count == 5 && palmDirection == Vector.Forward) {
            //TODO start flamethrowing
            Debug.Log("Flamethrower");
        }
        //one pointing finger forward - aim
        else if (pointingFingers.Count == 1 && palmDirection == Vector.Left) {
            //TODO aim
            Debug.Log("Aiming");
        }

    }

    //return the rounded normal values of the palm of an hand (usefull to confront them with directions)
    Vector GetVectorDirection(Vector vec) {
        float x = Mathf.Round(vec.x);
        float y = Mathf.Round(vec.y);
        float z = Mathf.Round(vec.z);
        return new Vector(x, y, z);
    }
}
