using System.Collections;
using System.Collections.Generic;
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
        Vector2 position = SearchCoord();
        // spawn animal
        Instantiate(animals[Random.Range(0, 3)], new Vector3(position.x, 0.5f, position.y), Quaternion.identity, transform);
        currentTime = spawnTime + Time.time;
    }

    // find a coord in a range
    Vector2 SearchCoord() {
        bool find = false;
        float x = 0f;
        float z = 0f;
        while (!find) {
            x = Random.Range(topCorner.position.x, bottomCorner.position.x);
            z = Random.Range(rightCorner.position.z, leftCorner.position.z);
            find = CheckCoord(x, z);
        }
        return new Vector2(x, z);
    }

    // check if coords are not to near to another animal
    bool CheckCoord(float x, float z) {
        bool isFree = true;
        foreach (Transform animal in transform) {
            // if is very near another object set to false
            if (x <= (animal.position.x + 5f) && x >= (animal.position.x - 5f) && z <= (animal.position.z + 5f) && z >= (animal.position.z - 5f)) {
                isFree = false;
                break;
            }
        }
        return isFree;
    }
}
