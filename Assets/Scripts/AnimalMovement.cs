using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AnimalMovement : MonoBehaviour {

    //custom settings
    public float idleTime;
    public float minimumDistance;

    public Vector3 destination;

    public Animator animator;
    private NavMeshAgent navigator;

    private float thinkingTime = 1.0f;

    private EnemySpawn spawner;
    // Update is called once per frame
    void Start () {
        //enable navmesh
        navigator = GetComponent<NavMeshAgent>();
        navigator = GetComponent<NavMeshAgent>();
        StartCoroutine(MovementRoutine());
        spawner = FindObjectOfType<EnemySpawn>();
	}

    protected IEnumerator MovementRoutine() {
        while (true) {
            //animal just spawned or stopped. Wait some times
            yield return new WaitForSeconds(idleTime);
            //choose destination
            bool isDistant = false;
            Vector2 coords = Vector2.zero;
            while (!isDistant) {
                coords = CoordFinder.SearchCoord(transform.parent, spawner.topCorner, spawner.bottomCorner, spawner.rightCorner, spawner.leftCorner);
                if(Vector2.Distance(coords,new Vector2(transform.position.x, transform.position.z)) >= minimumDistance) {
                    isDistant = true;
                }
                yield return new WaitForSeconds(thinkingTime);
            }
            destination = new Vector3(coords.x + transform.parent.position.x, transform.position.y, coords.y + transform.parent.position.z);
            //start moving to destination
            animator.SetBool("Move", true);
            navigator.enabled = true;
            navigator.SetDestination(destination);
            //Debug.Log("Destination: "+destination);
            yield return new WaitForSeconds(thinkingTime);
            //check when we arrive at the destination
            bool arrived = false;
            while (!arrived) {
                if (destination.x == transform.position.x && destination.z == transform.position.z) arrived = true;
                yield return new WaitForSeconds(thinkingTime);
            }
            //Debug.Log("Arrived");
            //destination reached. Stop animation
            animator.SetBool("Move", false);
            //stop moving because i'm arrived
            navigator.enabled = false;
        }
    }
}
