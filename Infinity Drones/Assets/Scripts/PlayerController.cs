using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : PhysicsBody
{

    [SerializeField] float maxSpeed = 20f;
    [SerializeField] float moveSpeed = 7f;
    [SerializeField] float jumpTakeOffSpeed = 6.2f;

    SpriteRenderer spriteRenderer;
    Animator animator;
    Camera cam;

    // Start is called before the first frame update
    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Start() {
        cam = Camera.main;
    }

    void Update() {
        if (Input.GetAxisRaw("Fire1") != 0) {
            Fire();
        }
    }
    
    protected override void ComputeVelocity() {
        Vector2 move = Vector2.zero;
        if (Mathf.Abs(velocity.magnitude) < maxSpeed) {
            move.x = Input.GetAxis("Horizontal");
        }

        if (Input.GetButtonDown("Jump") && grounded) {
            animator.SetTrigger("jump");
            velocity.y = jumpTakeOffSpeed;
        } else if(Input.GetButtonUp("Jump") && velocity.y > 0) {
            if (velocity.y > 0) {
                velocity.y = velocity.y * .5f;
            }
        }

        bool flipSprite = spriteRenderer.flipX ? move.x < -0.01f : move.x > 0.01f;
        FlipSprite(flipSprite);

        float xSpeed = Mathf.Abs(velocity.x);
        animator.SetBool ("grounded", grounded);
        animator.SetBool("moving", xSpeed > 0.05);
        animator.SetFloat("x-speed", xSpeed);
        animator.SetFloat("y-speed", velocity.y);

        targetVelocity = move * moveSpeed;
    }

    void Fire() {
        
        bool flipSprite = spriteRenderer.flipX ? fireVector.x < 0f : fireVector.x > 0f;
        FlipSprite(flipSprite);
        GameObject droneToFireAt = GetClosestObject("Proner", fireVector);
    }

    void FlipSprite(bool execute) {
        if (execute) {
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }
    }

    Vector2 GetFireVector () {
        Vector2 fireVector;
        var 
        if (gamepad != null) {
            fireVector = gamepad.rightStick.ReadValue();
        } else if (mouse != null) {
            fireVector = cam.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        } else {
            fireVector = new Vector2(spriteRenderer.flipX ? -1f : 1f, 0f);
        }
        fireVector = fireVector.normalized;
        return fireVector;
    }

    GameObject GetObjectToFireAt(string tag, Vector2 firePosition) {
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(tag);
        GameObject closestObject;
        float closestDistance = firePosition.magnitude;
        foreach (GameObject obj in objectsWithTag)
        {
            float angle = AngleBetweenVectors(transform.position, obj);
        }
        return closestObject;
    }

    float AngleBetweenVectors (Vector2 vec1, Vector2 vec2) {
        return Mathf.Atan2(vec2.y - vec1.y, vec2.x - vec1.x) * Mathf.Rad2Deg;
    }

}
