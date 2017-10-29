using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AnimalMovement : MonoBehaviour {

    //custom settings
    public float idleTime;
    public float minimumDistance;

    public Animator animator;
    private NavMeshAgent navigator;

    private float thinkingTime = 1.0f;

    private EnemySpawn spawner;
    // Update is called once per frame
    void Start () {
        //enable navmesh
        navigator = GetComponent<NavMeshAgent>();
        navigator.enabled = true;
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
            Vector3 destination = Vector3.zero;
            while (!isDistant) {
                destination = CoordFinder.SearchCoord(transform.parent, spawner.topCorner, spawner.bottomCorner, spawner.rightCorner, spawner.leftCorner);
                if(Vector3.Distance(destination,transform.position) >= minimumDistance) {
                    isDistant = true;
                }
                yield return new WaitForSeconds(thinkingTime);
            }
            //start moving to destination
            animator.SetBool("Move", true);
            navigator.SetDestination(destination);
            yield return new WaitForSeconds(thinkingTime);
            //check when we arrive at the destination
            bool arrived = false;
            while (!arrived) {
                if (destination.x == transform.position.x && destination.z == transform.position.z) arrived = true;
                yield return new WaitForSeconds(thinkingTime);
            }
            //destination reached. Stop animation
            animator.SetBool("Move", false);
        }
    }
}
