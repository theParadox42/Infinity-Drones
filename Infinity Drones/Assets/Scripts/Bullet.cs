using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] DetatchParticles detatchParticles = null;
    Rigidbody2D rb;

    void Start () {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnCollisionEnter2D(Collision2D col) {
        Collision(col.collider);
    }
    void OnTriggerEnter2D(Collider2D col) {
        Collision(col);
    }

    void Collision(Collider2D col) {
        if (col.tag == "Player") {
            col.GetComponent<PlayerController>().AddKnockback(rb.velocity);
        } else if (col.tag == "Proner") {
            col.GetComponent<Proner>().AddKnockback(rb.velocity / 10);
        }
        if (detatchParticles) {
            detatchParticles.Detatch();
        }
        Destroy(gameObject);
    }

}
