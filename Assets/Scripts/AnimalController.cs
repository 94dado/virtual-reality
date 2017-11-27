using UnityEngine;

public class AnimalController : MonoBehaviour {

    // health of the animal
    [Range(1, 3)] public int health;
    // explosion speed 
    public float explosionSpeed;
    public ParticleSystem explosion;

    Rigidbody myRigidbody;
    AnimalMovement move;
    AudioSource audioSource;
    bool isFlame;
    bool isRocket;

    void Start() {
        myRigidbody = GetComponent<Rigidbody>();
        move = GetComponent<AnimalMovement>();
        audioSource = GetComponent<AudioSource>();
    }

    // kill the animal
    void Die() {
        // stop movement coroutine 
        if (move != null) {
            move.StopMovement();
        }
        // if is not flamethrower push the animal up
        if (isRocket) {
            // create explosion
            Destroy(Instantiate(explosion, transform.position, transform.rotation).gameObject, 4f);
        }
        if (!isFlame) {
            myRigidbody.velocity = Vector3.up * explosionSpeed;
            myRigidbody.rotation = Random.rotation;
        }
        // play the audio of the death of enemy
        audioSource.Play();
        Destroy(transform.gameObject, 5f);
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

    // collision with a particle
    void OnTriggerEnter(Collider other) {
        // if is a rocket
        if (other.tag == "Rocket") {
            isRocket = true;
        }
        // if is flame
        else {
            isFlame = true;
        }
        TakeDamage();
    }

    void OnTriggerStay(Collider other) {
        // if there is a collision with a particle (not work for rocket)
        if (other.GetComponent<ParticleSystem>() != null) {
            isFlame = true;
            TakeDamage();
        }
    }

    void OnTriggerExit(Collider other) {
        isRocket = false;
        isFlame = false;
    }
}
