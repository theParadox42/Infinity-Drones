using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] DetatchParticles detatchParticles = null;
    [SerializeField] ParticleSystem bulletExplosion = null;
    Rigidbody2D rb;

    void Start () {
        rb = GetComponent<Rigidbody2D>();
        Invoke("DestroySelf", Random.Range(0.5f, 0.7f));
    }

    void OnCollisionEnter2D(Collision2D col) {
        Collision(col.collider);
    }
    void OnTriggerEnter2D(Collider2D col) {
        Collision(col);
    }

    void Collision(Collider2D col) {
        if (col.tag == "Player") {
            col.GetComponent<PlayerController>().TakeDamage(rb.velocity);
        } else if (col.tag == "Proner") {
            col.GetComponent<Proner>().TakeDamage(rb.velocity / 10);
        }
        DestroySelf();
    }

    void DestroySelf() {
        if (detatchParticles) {
            detatchParticles.Detatch();
        }
        if (bulletExplosion) {
            Instantiate(bulletExplosion, transform.position + new Vector3(0, 0, 10f), transform.rotation);
        }
        Destroy(gameObject);
    }

}
