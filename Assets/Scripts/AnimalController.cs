using UnityEngine;

public class AnimalController : MonoBehaviour {

    // health of the animal
    [Range(1, 3)] public int health;

    // kill the animal
    void Die() {
        Destroy(transform.gameObject);
    }

    // take a hit
    public bool TakeDamage() {
        health -= 1;
        if (health <= 0) {
            Die();
            return true;
        }
        return false;
    }

    void OnTriggerEnter(Collider other) {
        // if there is a collision with a particle
        TakeDamage();
    }

    void OnTriggerStay(Collider other) {
        // if there is a collision with a particle (not work for rocket)
        if (other.GetComponent<ParticleSystem>() != null) {
            TakeDamage();
        }
    }
}
