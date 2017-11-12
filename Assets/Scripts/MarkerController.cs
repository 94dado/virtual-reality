using UnityEngine;

public class MarkerController : MonoBehaviour {

    public Transform cam;

	// Update is called once per frame
	void Update () {
        // look the camera
        transform.LookAt(cam.transform);
        // follow the parent
        transform.position = transform.parent.position;
	}
}
