using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AnimalMovement : MonoBehaviour {

    //custom settings
    public float idleTime;
    public float minimumDistance;

    public Animator animator;
    private NavMeshAgent navigator;

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
                destination = CoordFinder.SearchCoord(transform, spawner.topCorner, spawner.bottomCorner, spawner.rightCorner, spawner.leftCorner);
                if(Vector3.Distance(destination,transform.position) > minimumDistance) {
                    isDistant = true;
                }
                yield return 0;
            }
            animator.SetBool("Move", true);
            navigator.SetDestination(destination);
            while (destination.x != transform.position.x && destination.z != transform.position.z) {
                yield return 0;
            }
            //destination reached. Stop animation
            animator.SetBool("Move", false);
        }
    }
}
