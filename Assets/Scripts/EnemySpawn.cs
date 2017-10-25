using UnityEngine;

public class EnemySpawn : MonoBehaviour {

    public Transform topCorner;
    public Transform bottomCorner;
    public Transform rightCorner;
    public Transform leftCorner;
    public GameObject[] animals;
    [Range(1, 10)] public int maxAnimals;
    public float spawnTime;

    float currentTime;

	void Update () {
        // if we can add another animal and spawntime is passed
        if (transform.childCount < maxAnimals && currentTime <= Time.time) {
            Spawn();
        }
	}

    // spawn an animal in the 
    void Spawn() {
        Vector2 position = CoordFinder.SearchCoord(transform, topCorner, bottomCorner, rightCorner, leftCorner);
        // spawn animal
        Instantiate(animals[Random.Range(0, 3)], new Vector3(position.x, 0.5f, position.y), Quaternion.identity, transform);
        currentTime = spawnTime + Time.time;
    }


}
