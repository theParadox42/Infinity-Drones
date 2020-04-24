using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
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
            col.GetComponent<PlayerController>().AddKnockback(rb.velocity / 10);
        } else if (col.tag == "Proner") {
            col.GetComponent<Proner>().AddKnockback(rb.velocity / 10);
        }
        Destroy(gameObject);
    }

}
