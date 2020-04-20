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
        Vector2 fireVector = GetFireVector ();

        // Graphics update
        bool flipSprite = spriteRenderer.flipX ? fireVector.x < 0f : fireVector.x > 0f;
        FlipSprite(flipSprite);
        Animator.SetTrigger("shoot");

        // TODO: Make this so it can't go strait up
        float fireRotation = Mathf.Constrain(Vector2.Angle(Vector2.zero, fireVector);
        Debug.Log(fireRotation);
        // Lock onto proner
        GameObject droneToFireAt = GetObjectToFireAt ("Proner", fireRotation, fireVector.x > 0);
        if (droneToFireAt) {
            fireVector = (droneToFireAt.transform.position - transform.position).normalized;
        }
        // Create bullet here
        Debug.Log(fireVector);
    }

    void FlipSprite(bool execute) {
        if (execute) {
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }
    }

    Vector2 GetFireVector () {
        float minMag = 0.1f;
        Vector2 fireVector = new Vector2(Input.GetAxis("Joy X"), Input.GetAxis("Joy Y"));
        if (fireVector.magnitude > minMag) {
            Debug.Log("Using Joystick");
        } else if (Input.mousePosition.magnitude >= minMag) {
            fireVector = cam.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        } else {
            fireVector = new Vector2(spriteRenderer.flipX ? -1f : 1f, 0f);
        }
        return fireVector.normalized;
    }

    GameObject GetObjectToFireAt(string tag, float fireAngle, bool goesRight) {
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(tag);
        GameObject bestObject = null;
        float closestDistance = 10f;
        foreach (GameObject obj in objectsWithTag) {
            Vector2 displacement = obj.transform.position - transform.position;
            float distance = displacement.magnitude;
            // Checks if it is the right direction
            if ((displacement.x > 0 && goesRight) || (displacement.x < 0 && !goesRight)) {
                // How far out something can be
                float margin = 30f;
                float relativeAngle = Vector2.Angle(transform.position, obj.transform.position) - fireAngle;
                // Check if it is out of the fire range
                if (relativeAngle > -margin && relativeAngle < margin) {
                    if (distance < closestDistance) {
                        closestDistance = distance;
                        bestObject = obj;
                    }
                }
            }
        }
        return closestObject;
    }

}
