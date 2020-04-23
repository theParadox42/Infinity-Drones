using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proner : MonoBehaviour
{

    [SerializeField] PlayerController player = null;
    [SerializeField] float droneAcceleration = 2f;
    [SerializeField] float droneTargetSpeed = 8f;
    [SerializeField] float targetDistance = 4f;

    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float bulletReloadTime = 0.3f;
    [SerializeField] float bulletSpeed = 15f;
    [SerializeField] float inconsistency = 10f;
    float bulletTimer;


    private Vector2 playerDisplacement;
    private Rigidbody2D rb;

    // Initialize stuff here
    void Start()
    {
        bulletTimer = bulletReloadTime;
        rb = GetComponent<Rigidbody2D>();
    }

    // Determin direction to move here
    void Update()
    {
        playerDisplacement = player.transform.position - transform.position;
        var modifyer = GetSpeedModifyer(playerDisplacement.magnitude - targetDistance);
        rb.velocity += playerDisplacement.normalized * droneAcceleration * modifyer;
        if (rb.velocity.magnitude > droneTargetSpeed) {
            rb.velocity *= 0.5f;
        } else {
            rb.velocity *= 0.98f;
        }

        bulletTimer -= Time.deltaTime;
        if (bulletTimer < 0 && playerDisplacement.magnitude < targetDistance * 2) {
            bulletTimer = bulletReloadTime;
            var newBullet = Instatiate(bulletPrefab, transform.position + Vector3(0f, 0f, 0.1f), transform.rotation));
            newBullet.GetComponent<Rigidbody2D>().velocity = playerDisplacement.normalized * bulletSpeed;
        }
    }

    float GetSpeedModifyer (float distance) {
        float n = Mathf.Pow(2, distance);
        return 2f * n / (n + 1f) - 1f;
    }
}
