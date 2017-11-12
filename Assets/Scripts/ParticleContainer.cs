using UnityEngine;

public class ParticleContainer : MonoBehaviour {
	
	void Update () {
        // follow the parent during movement
        transform.position = transform.parent.position;
        transform.rotation = transform.parent.rotation;
	}
}
