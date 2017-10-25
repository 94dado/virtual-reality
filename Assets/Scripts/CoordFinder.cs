using UnityEngine;

public class CoordFinder {

    // find a coord in a range
    public static Vector2 SearchCoord(Transform transform, Transform topCorner, Transform bottomCorner, Transform rightCorner, Transform leftCorner) {
        bool find = false;
        float x = 0f;
        float z = 0f;
        while (!find) {
            x = Random.Range(topCorner.position.x, bottomCorner.position.x);
            z = Random.Range(rightCorner.position.z, leftCorner.position.z);
            find = CheckCoord(x, z, transform);
        }
        return new Vector2(x, z);
    }

    // check if coords are not to near to another animal
    static bool CheckCoord(float x, float z, Transform transform) {
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
