using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WasteBlock : MonoBehaviour
{
    [SerializeField] ParticleSystem toxicBurn = null;
    BoxCollider2D boxCollider;

    void Start () {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    // On Collision
    void OnTriggerStay2D(Collider2D col) {
        bool releaseParticles = false;
        if (col.tag == "Player") {
            if (col.GetComponent<PlayerController>().TakeDamage(Vector2.up)) {
                releaseParticles = true;
            }
        } else if (col.tag == "Proner") {
            if (col.GetComponent<Proner>().TakeDamage(Vector2.up * 2f)) {
                releaseParticles = true;
            }
        }
        if (releaseParticles) {
            Vector3 position = new Vector3(col.transform.position.x, transform.position.y, col.transform.position.z + 0.3f);
            Instantiate(toxicBurn, position, transform.rotation);
        }
    }
}
