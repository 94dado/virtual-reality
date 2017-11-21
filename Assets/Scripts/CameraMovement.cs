using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    //controller settings
    public bool invertedAxis = true;
    //constraints
    public Transform player;
    public float gravity = 100f;

    public float keySpeed = 10;

    float distance;
    float rotationXoffset;

    CharacterController controller;

    // Use this for initialization
    void Start()
    {
        //lock the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //get the player datas
        controller = player.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        //player position
        Vector3 moveDir;
        if (invertedAxis) moveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        else moveDir = new Vector3(-Input.GetAxis("Vertical"), 0, Input.GetAxis("Horizontal"));
        moveDir = transform.TransformDirection(moveDir);
        moveDir *= keySpeed;
        moveDir = new Vector3(moveDir.x, 0, moveDir.z);
        moveDir.y -= gravity * Time.deltaTime;
        controller.Move(moveDir * Time.deltaTime);
    }
}