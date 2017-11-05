using UnityEngine;
using Leap;

public class PlayerController : MonoBehaviour {

    //leap motion settings
    public float aimMinVelocity = 50.0f;            // mm/s
    public float aimHistorySeconds = .1f;           // s
    public float aimMinDistance = 5.0f;             // mm

    public float flameRadius = 5.0f;                // mm

    //private attributes
    Controller controller;              //for leap
    bool aiming = false;             //i'm aiming an enemy?

	void Start () {
        //create the leap controller
        controller = new Controller();
        //enable gestures
        controller.EnableGesture(Gesture.GestureType.TYPECIRCLE);       //gesture for flamethrower
        controller.EnableGesture(Gesture.GestureType.TYPESCREENTAP);    //gesture for aiming

        //circle gesture parameter
        controller.Config.SetFloat("Gesture.Circle.MinRadius", flameRadius);
        
        //screentap gesture parameters
        controller.Config.SetFloat("Gesture.ScreenTap.MinForwardVelocity", aimMinVelocity);
        controller.Config.SetFloat("Gesture.ScreenTap.HistorySeconds", aimHistorySeconds);
        controller.Config.SetFloat("Gesture.ScreenTap.MinDistance", aimMinDistance);
    }
	

	void Update () {
        //get frame and gestures
        Frame frame = controller.Frame();
        GestureList gestures = frame.Gestures();
        //elaborate every gesture
        foreach(Gesture gesture in gestures) {
            //check if is a valid gesture
            if (gesture != Gesture.Invalid) {
                ElaborateGesture(gesture);
            }
        }
        //check if i'm aiming
        if (aiming) {
            //todo write the code to handle the different attacks
        }
	}

    //elaborate gesture to do the correct action
    void ElaborateGesture(Gesture gesture) {
        if(gesture.Type == Gesture.GestureType.TYPECIRCLE) {
            Debug.Log("FLAMETHROWER!!!");
            //todo write the code for the flamethrower
        }else if(gesture.Type == Gesture.GestureType.TYPESCREENTAP) {
            //todo write the code to aim an animal
            Debug.Log("AIM!");
        }
    }

    // attack
    void Attack() {
        
    }
}
