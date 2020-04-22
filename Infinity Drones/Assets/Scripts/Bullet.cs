using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D col) {
        // Make an explosion
        Destroy(gameObject);
    }
}
