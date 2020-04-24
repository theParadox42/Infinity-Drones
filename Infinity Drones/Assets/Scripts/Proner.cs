using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proner : MonoBehaviour
{

    [SerializeField] PlayerController player = null;
    [SerializeField] float droneAcceleration = 2f;
    [SerializeField] float droneTargetSpeed = 8f;
    [SerializeField] float targetDistance = 4f;

    [SerializeField] GameObject bulletPrefab = null;
    [SerializeField] float bulletReloadTime = 0.3f;
    [SerializeField] float bulletSpeed = 15f;
    [SerializeField] float inaccuracy = 10f;
    float bulletTimer;

    private Vector2 tempKnockback;
    private Rigidbody2D rb;

    // Initialize stuff here
    void Start()
    {
        bulletTimer = bulletReloadTime * Random.Range(1f, 2f);
        rb = GetComponent<Rigidbody2D>();
    }

    // Determin direction to move here
    void Update()
    {
        Vector2 playerDisplacement = player.transform.position - transform.position;
        var modifyer = GetSpeedModifyer(playerDisplacement.magnitude - targetDistance);
        rb.velocity += playerDisplacement.normalized * droneAcceleration * modifyer;
        if (rb.velocity.magnitude > droneTargetSpeed) {
            rb.velocity *= 0.5f;
        } else {
            rb.velocity *= 0.98f;
        }

        if (tempKnockback.magnitude > 0.1f) {
            rb.velocity += tempKnockback;
            tempKnockback *= 0.95f;
        }

        bulletTimer -= Time.deltaTime;
        if (bulletTimer < 0 && playerDisplacement.magnitude < targetDistance * 1.5) {
            bulletTimer = bulletReloadTime;
            Fire(playerDisplacement);
        }
    }

    float GetSpeedModifyer (float distance) {
        float n = Mathf.Pow(2, distance);
        return 2f * n / (n + 1f) - 1f;
    }

    void Fire(Vector2 playerDisplacement) {
        Vector2 bulletVector = playerDisplacement.normalized;
        float bulletAngle = GetAngle(bulletVector) + Random.Range(-inaccuracy, inaccuracy);
        bulletAngle *= Mathf.Deg2Rad;
        bulletVector = new Vector2(Mathf.Cos(bulletAngle), Mathf.Sin(bulletAngle)) * bulletSpeed;
        
        // Create bullet
        GameObject newBullet = Instantiate(bulletPrefab, transform.position + new Vector3(0f, 0f, 0.1f), Quaternion.LookRotation(Vector3.forward, bulletVector));
        newBullet.GetComponent<Rigidbody2D>().velocity = bulletVector;
        Destroy(newBullet, 10f);
    }

    float GetAngle(Vector2 vector) {
        return Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;
    }

    public void AddKnockback (Vector2 knockback) {
        tempKnockback += knockback;
    }
}
