using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proner : MonoBehaviour
{

    [SerializeField] PlayerController player = null;
    [SerializeField] float droneAcceleration = 2f;
    [SerializeField] float droneTargetSpeed = 8f;
    [SerializeField] float targetDistance = 4f;
    [SerializeField] float reloadTime = 0.3f;


    private Vector2 playerDisplacement;
    private Rigidbody2D rb;

    // Initialize stuff here
    void Start()
    {
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
    }

    float GetSpeedModifyer (float distance) {
        float n = Mathf.Pow(2, distance);
        return 2f * n / (n + 1f) - 1f;
    }
    
    // Do physics here
    void FixedUpdate() {
        
    }
}
