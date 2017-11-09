using UnityEngine;
using Leap;

public class CustomLeapNotice : MonoBehaviour {

    public int frameDelay = 10;

    Controller leapController;
    int frameCounter;
    Canvas canvas;
	// Use this for initialization
	void Start () {
        leapController = new Controller();
        frameCounter =  0;
        canvas = GetComponent<Canvas>();
        canvas.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(IsConnected() && canvas.enabled) {
            //leap connected and canvas is visible. Hide canvas
            canvas.enabled = false;
            frameCounter = 0;
        }
        else if(!IsConnected()){
            frameCounter++;
            if(frameCounter > frameDelay) {
                canvas.enabled = true;
            }
        }
	}

    bool IsConnected()
    {
        return leapController.IsConnected;
    }
}
