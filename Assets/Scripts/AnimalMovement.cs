using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AnimalMovement : MonoBehaviour {

    //custom settings
    public float idleTime;

    public Animator animator;
    private NavMeshAgent navigator;

	
	// Update is called once per frame
	void Start () {
        animator = GetComponent<Animator>();
        navigator = GetComponent<NavMeshAgent>();
        StartCoroutine(MovementRoutine());
	}

    private IEnumerator MovementRoutine() {
        while (true) {
        //animal just spawned or stopped. Wait some times
        yield return new WaitForSeconds(idleTime);
        //choose destination
        Vector3 destination = new Vector3(0, 0, 1);    //TODO call method to change them
                                                        //set destination and start animation
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
