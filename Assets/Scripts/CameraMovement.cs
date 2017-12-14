using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    //controller settings
    public bool invertedAxis = true;
    //constraints
    public float gravity = 100f;

    public float keySpeed = 10f;

	public CharacterController player;
    float distance;
    float rotationXoffset;

    

    // Update is called once per frame
    void Update()
    {
        //player rotation
        player.transform.localEulerAngles = 
            new Vector3(player.transform.localEulerAngles.x, transform.localEulerAngles.y, player.transform.localEulerAngles.z);
        //player position
        Vector3 moveDir;
        if (!invertedAxis) moveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        else moveDir = new Vector3(Input.GetAxis("Vertical"), 0, -Input.GetAxis("Horizontal"));
        moveDir = transform.TransformDirection(moveDir);
        moveDir *= keySpeed;
        moveDir = new Vector3(moveDir.x, 0, moveDir.z);
        moveDir.y -= gravity * Time.deltaTime;
        player.Move(moveDir * Time.deltaTime);
    }
}