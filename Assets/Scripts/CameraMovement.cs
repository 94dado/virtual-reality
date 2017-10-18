using UnityEngine;

public class CameraMovement : MonoBehaviour {

    public float keySpeed = 10;
    public float mouseSpeed = 1.25f;
    public GameObject eye;

    Quaternion originalRotation;
    Vector2 angle = new Vector2(0f, 0f);
    Vector2 minAngle = new Vector2(-360f, -90f);
    Vector2 maxAngle = new Vector2(360f, 90f);
    float limit = 360f;

    // Use this for initialization
    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        originalRotation = transform.localRotation;
    }

    void Update() {
        if (Input.GetKey(KeyCode.A)) {
            Strafe(-keySpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D)) {
            Strafe(keySpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.W)) {
            Ahead(keySpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S)) {
            Ahead(-keySpeed * Time.deltaTime);
        }

        float dx = Input.GetAxis("Mouse X");
        float dy = Input.GetAxis("Mouse Y");
        Look(new Vector2(dx, dy) * mouseSpeed);
    }

    // move player left and right
    void Strafe(float dist) {
        transform.position += eye.transform.right * dist;
    }

    // move player ahead and back
    void Ahead(float dist) {
        transform.position += eye.transform.forward * dist;
    }

    // move the camera to look the new position of the player
    void Look(Vector2 dist) {
        angle += dist;

        angle.x = ClampAngle(angle.x, minAngle.x, maxAngle.x);
        angle.y = ClampAngle(angle.y, minAngle.y, maxAngle.y);

        Quaternion quatX = Quaternion.AngleAxis(angle.x, Vector3.up);
        Quaternion quatY = Quaternion.AngleAxis(angle.y, -Vector3.right);

        transform.localRotation = originalRotation * quatX * quatY;
    }

    float ClampAngle(float angulation, float min, float max) {
        if (angulation < -limit) {
            angulation += limit;
        }
        else if (angulation > limit) {
            angulation -= limit;
        }
        return Mathf.Clamp(angulation, min, max);
    }
}