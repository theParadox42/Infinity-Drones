using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proner : MonoBehaviour
{

    [SerializeField] private PlayerController player;
    [SerializeField] private float droneSpeed = 3f;
    [SerializeField] private float targetDistance = 4f;

    private Vector2 playerDisplacement;
    private Vector2 velocity;

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
        
        velocity = playerDisplacement.normalized * droneSpeed * GetSpeedModifyer();
        rb.velocity = velocity;
    }

    float GetSpeedModifyer () {
        float n = Mathf.Pow(2, playerDisplacement.magnitude - targetDistance);
        return 2f * n / (n + 1f) - 1f;
    }
    
    // Do physics here
    void FixedUpdate() {
        
    }
}
