using UnityEngine;

public class AnimalController : MonoBehaviour {

    // health of the animal
    [Range(1, 3)] public int health;

    // kill the animal
    void Die() {
        Destroy(transform.gameObject);
    }

    // take a hit
    void TakeDamage() {
        health -= 1;
        if (health <= 0) {
            Die();
        }
    }
}
