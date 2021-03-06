﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proner : MonoBehaviour
{

    #region Variables

    [SerializeField] int health = 4;
    [SerializeField] float damageCooldown = 0.2f;
    float damageCooldownTimer;

    [SerializeField] bool isChild = false;
    bool dead;
    bool active;

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

    [SerializeField] ParticleSystem explosion = null;
    PlayerController player = null;
    PronerCap pronerCap = null;

    Vector2 tempKnockback;
    Vector2 playerDisplacement;

    Animator animator;
    Rigidbody2D rb;
    Collider2D col;

    #endregion

    #region System Events

    void Start() {
        player = FindObjectOfType<PlayerController>();
        pronerCap = FindObjectOfType<PronerCap>();

        if (pronerCap) {
            pronerCap.AddProner();
        }

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        col = GetComponent<Collider2D>();

        bulletTimer = bulletReloadTime * Random.Range(1f, 2f);
        printerTimer = Random.Range(0.8f, 1.5f) * printerReloadTime;

        active = !isChild;
        dead = false;
        if (!active) {
            transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            col.enabled = false;
            rb.velocity = new Vector3(0, -0.5f, 0f);
        }
    }

    void Update() {
        if (player && active) {
            Move();
            ReloadBullets();
        } else if (!active) {
            UpdateActive();
            return;
        } else if (!player) {
            playerDisplacement = new Vector2(0f, 0f);
        }
        UpdateHealth();
        HandleKnockback();
        ReloadPrinter();

    }

    #endregion

    #region Movement

    void Move () {
        // Move towards player
        float tempTargetDistance = printingStage > 0 ? printingDistance : targetDistance;
        playerDisplacement = player.transform.position - transform.position;
        var modifyer = GetSpeedModifyer(playerDisplacement.magnitude - tempTargetDistance);
        rb.velocity += playerDisplacement.normalized * droneAcceleration * modifyer;
        if (rb.velocity.magnitude > droneTargetSpeed) {
            rb.velocity *= 0.5f;
        } else {
            rb.velocity *= 0.98f;
        }
    }

    float GetSpeedModifyer (float distance) {
        float n = Mathf.Pow(2, distance);
        return 2f * n / (n + 1f) - 1f;
    }

    #endregion

    #region Firing

    void ReloadBullets () {
        // Don't fire while printing
        if (printingStage == 0) {
            bulletTimer -= Time.deltaTime;
            // Fire at player
            if (bulletTimer < 0 && playerDisplacement.magnitude < targetDistance * 1.5) {
                bulletTimer = bulletReloadTime;
                Fire(playerDisplacement);
            }
        }
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

    #endregion

    #region Regular Updates

    void UpdateHealth () {
        // Die
        if (health <= 0 && !dead) {
            dead = true;
            Destroy(gameObject, 0.1f);
            pronerCap.RemoveProner();
            if (explosion) {
                Instantiate(explosion, transform.position, transform.rotation);
                explosion = null;
            }
        }

        if (damageCooldownTimer >= 0f) {
            damageCooldownTimer -= Time.deltaTime;
        }
    }

    void UpdateActive () {
        transform.localScale = transform.localScale * 1.2f;
        if (transform.localScale.x >= 1f) {
            transform.localScale = new Vector3(1f, 1f, 1f);
            active = true;
            col.enabled = true;
            transform.parent = null;
        }
    }

    void HandleKnockback () {
        // Knockback
        if (tempKnockback.magnitude > 0.1f) {
            rb.velocity += tempKnockback;
            tempKnockback *= 0.95f;
        }
    }

    #endregion
    
    #region Printing

    void ReloadPrinter () {
        // Make more drones
        if (!pronerCap.maxedOut && printingStage == 0) {
            printerTimer -= Time.deltaTime;
            if (printerTimer <= 0) {
                printerTimer = printerReloadTime;
                printingStage ++;
                printed = false;
                animator.SetBool("printing", true);
                Invoke ("PrintDrone", printerReloadTime / 2);
            }
        } else if (printingStage == 1 && playerDisplacement.magnitude > printingDistance) {
            PrintDrone();
        }
    }

    void PrintDrone() {
        if (printed) {
            return;
        }
        printed = true;
        animator.SetBool("printing", false);
        printingStage = 0;
        if (childProner) {
            var newProner = Instantiate(childProner, transform);
            newProner.transform.position = newProner.transform.position - new Vector3(0, col.bounds.extents.y, 0);
        }
    }
    
    #endregion

    #region Input Events

    public void AddKnockback (Vector2 knockback) {
        tempKnockback += knockback;
    }

    public bool TakeDamage (Vector2 knockback) {
        AddKnockback(knockback);
        return TakeDamage();
    }
    public bool TakeDamage () {
        if (damageCooldownTimer <= 0f) {
            animator.SetTrigger("flash");
            health --;
            damageCooldownTimer = damageCooldown;
            return true;
        }
        return false;
    }

    #endregion
}
