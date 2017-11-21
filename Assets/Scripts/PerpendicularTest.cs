using UnityEngine;

public class PerpendicularTest : MonoBehaviour {

    public float EPSILON = 0.2f;

    // Use this for initialization
    public Transform obj;
	void Start () {
        if (obj == null) obj = new GameObject().transform;
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 up = transform.up.normalized,
                right = transform.right.normalized,
                forward = transform.forward.normalized,
                down = -up,
                left = -right,
                backward = -forward;
        Debug.DrawRay(transform.position, up, Color.blue);
        Debug.DrawRay(transform.position, forward, Color.blue);
        Debug.DrawRay(transform.position, right, Color.blue);
        Debug.DrawRay(transform.position, down, Color.red);
        Debug.DrawRay(transform.position, left, Color.red);
        Debug.DrawRay(transform.position, backward, Color.red);

        Debug.Log("sameUp: " + SimilarDirections(up, obj.up) + ", sameForward: "+ SimilarDirections(forward, obj.forward) +", sameRight: "+SimilarDirections(right,obj.right));
    }

    bool SimilarDirections(Vector3 axe, Vector3 objDir) {
        bool sameX = Mathf.Abs(axe.x - objDir.x) < EPSILON;
        if (!sameX) return false;
        bool sameY = Mathf.Abs(axe.y - objDir.y) < EPSILON;
        if (!sameY) return false;
        bool sameZ = Mathf.Abs(axe.z - objDir.z) < EPSILON;
        return sameZ;
    }
}
