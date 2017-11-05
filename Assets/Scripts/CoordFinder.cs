using UnityEngine;

public static class CoordFinder {

    // find a coord in a range
    public static Vector2 SearchCoord(Transform transform, Transform topCorner, Transform bottomCorner, Transform rightCorner, Transform leftCorner) {
        bool find = false;
        float x = 0f;
        float z = 0f;
        while (!find) {
            x = Random.Range(bottomCorner.position.x, topCorner.position.x);
            z = Random.Range(rightCorner.position.z, leftCorner.position.z);
            find = CheckCoord(x, z, transform);
        }
        return new Vector2(x, z);
    }

    // check if coords are not to near to another animal
    static bool CheckCoord(float x, float z, Transform transform) {
        float offset = 6f;
        bool isFree = true;
        foreach (Transform animal in transform) {
            // if is very near another object set to false
            if (x <= (animal.position.x + offset) && x >= (animal.position.x - offset) && z <= (animal.position.z + offset) && z >= (animal.position.z - offset)) {
                isFree = false;
                break;
            }
        }
        return isFree;
    }
}
