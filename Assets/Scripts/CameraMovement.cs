using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    //controller settings
    public bool invertedAxis = true;
    //constraints
    public Transform player;
    public float maxUpRotation = 20f;
    public float maxDownRotation = 20f;
    public float verticalOffset;
    public float gravity = 100f;

    //movement velocities
    public float mouseSpeed = 1.25f;
    public float keySpeed = 10;

    float rotX; // rotation around the horizontal/x axis
    float rotY; // rotation around the up/y axis

    float fixedY;      // to never change Y value

    float distance;
    float rotationXoffset;

    CharacterController controller;

    // Use this for initialization
    void Start()
    {
        Vector3 rot = transform.localRotation.eulerAngles;
        rotX = rot.x;
        rotY = rot.y;
        rotationXoffset = rotX;
        distance = Vector3.Distance(transform.position, player.transform.position);
        //lock the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //get the player datas
        controller = player.GetComponent<CharacterController>();
        fixedY = player.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        //camera rotation
        float mouseY = -Input.GetAxis("Mouse Y");
        float mouseX = Input.GetAxis("Mouse X");
        rotY += mouseX * mouseSpeed * Time.deltaTime;
        rotX += mouseY * mouseSpeed * Time.deltaTime;
        rotX = Mathf.Clamp(rotX, rotationXoffset - maxUpRotation, rotationXoffset + maxDownRotation);
        Quaternion localRotation = Quaternion.Euler(rotX, rotY, 0.0f);
        transform.rotation = localRotation;

        //camera position
        Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
        Vector3 position = localRotation * negDistance + player.position;
        position = new Vector3(position.x, position.y + verticalOffset, position.z);
        transform.position = position;

        //player position
        Vector3 moveDir;
        if (!invertedAxis) moveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        else moveDir = new Vector3(Input.GetAxis("Vertical"), 0, -Input.GetAxis("Horizontal"));
        moveDir = transform.TransformDirection(moveDir);
        moveDir *= keySpeed;
        moveDir = new Vector3(moveDir.x, 0, moveDir.z);
        moveDir.y -= gravity * Time.deltaTime;
        controller.Move(moveDir * Time.deltaTime);
    }
}