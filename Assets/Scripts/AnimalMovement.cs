using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnimalMovement : MonoBehaviour {

    public Animator animator;
    private NavMeshAgent navigator;
	
	// Update is called once per frame
	void Start () {
        animator = GetComponent<Animator>();
        navigator = GetComponent<NavMeshAgent>();
        
	}
}
