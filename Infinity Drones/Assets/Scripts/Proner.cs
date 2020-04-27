﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proner : MonoBehaviour
{

    [SerializeField] ParticleSystem explosion = null;
    [SerializeField] bool isChild = false;
    [SerializeField] int health = 4;

    [SerializeField] float droneAcceleration = 2f;
    [SerializeField] float droneTargetSpeed = 8f;
    [SerializeField] float targetDistance = 4f;

    [SerializeField] GameObject bulletPrefab = null;
    [SerializeField] float bulletReloadTime = 0.3f;
    [SerializeField] float bulletSpeed = 15f;
    [SerializeField] float inaccuracy = 10f;
    float bulletTimer;

    [SerializeField] Proner childProner = null;
    [SerializeField] float printerReloadTime = 10f;
    [SerializeField] float printingDistance = 15f;
    float printerTimer;
    int printingStage = 0;
    bool printed;

    PlayerController player = null;
    Vector2 tempKnockback;
    Animator animator;
    Rigidbody2D rb;

    // Initialize stuff here
    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        bulletTimer = bulletReloadTime * Random.Range(1f, 2f);
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        printerTimer = Random.Range(0.8f, 1.5f) * printerReloadTime;
    }

    // Determine direction to move here
    void Update()
    {
        // Move towards player
        float tempTargetDistance = printingStage > 0 ? printingDistance : targetDistance;
        Vector2 playerDisplacement = player.transform.position - transform.position;
        var modifyer = GetSpeedModifyer(playerDisplacement.magnitude - tempTargetDistance);
        rb.velocity += playerDisplacement.normalized * droneAcceleration * modifyer;
        if (rb.velocity.magnitude > droneTargetSpeed) {
            rb.velocity *= 0.5f;
        } else {
            rb.velocity *= 0.98f;
        }

        // Knockback
        if (tempKnockback.magnitude > 0.1f) {
            rb.velocity += tempKnockback;
            tempKnockback *= 0.95f;
        }

        // Don't fire while printing
        if (printingStage == 0) {
            bulletTimer -= Time.deltaTime;
            printerTimer -= Time.deltaTime;
            // Fire at player
            if (bulletTimer < 0 && playerDisplacement.magnitude < targetDistance * 1.5) {
                bulletTimer = bulletReloadTime;
                Fire(playerDisplacement);
            }
        }

        // Make more drones
        if (printerTimer < 0 && printingStage == 0) {
            printerTimer = printerReloadTime;
            printingStage ++;
            printed = false;
            animator.SetBool("printing", true);
            Invoke ("PrintDrone", printerReloadTime / 2);
        } else if (printingStage == 1 && playerDisplacement.magnitude > printingDistance) {
            PrintDrone();
        }

        // Die
        if (health <= 0) {
            Destroy(gameObject, 0.1f);
            if (explosion) {
                Instantiate(explosion, transform.position, transform.rotation);
                explosion = null;
            }
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

        AddKnockback(-bulletVector / 50);
    }

    float GetAngle(Vector2 vector) {
        return Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;
    }

    void PrintDrone() {
        if (printed) {
            return;
        }
        printed = true;
        animator.SetBool("printing", false);
        printingStage = 0;
        if (childProner) {
            Instantiate(childProner, transform.position + new Vector3(0, -3f, 0), transform.rotation);
        }
    }

    public void AddKnockback (Vector2 knockback) {
        tempKnockback += knockback;
    }

    public void TakeDamageFromBullet (GameObject bullet, Vector2 knockback) {
        AddKnockback(knockback);
        animator.SetTrigger("flash");
        health --;
    }
}
