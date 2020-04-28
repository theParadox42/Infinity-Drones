using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WasteBlock : MonoBehaviour
{
    [SerializeField] ParticleSystem toxicBurn;

    // On Collision
    void OnTriggerEnter2D(Collider2D col) {
        if (col.tag == "Player" || col.tag == "Proner") {
            Vector3 position = new Vector3(col.transform.position.x, transform.position.y, col.transform.position.z + 0.3);
            Instantiate(toxicBurn, position, transform.rotation);
            if (col.tag == "Player") {
                col.GetComponent<PlayerController>().TakeDamage(Vector2.up);
            } else if (col.tag == "Proner") {
                col.GetComponent<Proner>().TakeDamage(Vector2.up);
            }
        }

    }
}
