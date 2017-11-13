using UnityEngine;

public class MarkerController : MonoBehaviour {

    public Transform cam;
    public float markOffset;

    private void Start() {
        cam = FindObjectOfType<CameraMovement>().transform;
    }

    // Update is called once per frame
    void Update () {
        // look the camera
        transform.LookAt(cam.transform);
        // follow the parent
        transform.position = new Vector3(transform.parent.position.x, transform.parent.position.y + markOffset, transform.parent.position.z);
	}
}
