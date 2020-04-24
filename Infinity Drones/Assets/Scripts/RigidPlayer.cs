using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidPlayer : MonoBehaviour
{   

    [SerializeField] float maxSpeed = 8f;
    [SerializeField] float moveSpeed = 0.1f;
    [SerializeField] float jumpTakeOffSpeed = 6.2f;
    bool grounded = false;

    [SerializeField] LayerMask groundMask;

    Vector2 tempKnockback = Vector2.zero;

    SpriteRenderer spriteRenderer;
    Rigidbody2D rb;
    Animator animator;
    BoxCollider2D boxCollider2d;

    public bool flipped;
    
    // Start is called before the first frame update
    void Awake() {
        groundMask = LayerMask.NameToLayer("Block");
        spriteRenderer = GetComponent<SpriteRenderer>();
        flipped = spriteRenderer.flipX;
        rb = GetComponent<Rigidbody2D>();
        boxCollider2d = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
    }

    void Update () {


        CheckGrounded();

        Vector2 move = Vector2.zero;
        move.x = Input.GetAxis("Horizontal");

        if (Input.GetButtonDown("Jump") && grounded) {
            animator.SetTrigger("jump");
            rb.AddForce(jumpTakeOffSpeed * transform.up, ForceMode2D.Impulse);
            Debug.Log("jump");
        } else if(Input.GetButtonUp("Jump") && rb.velocity.y > 0) {
            if (rb.velocity.y > 0) {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * .5f);
            }
        }

        bool flipSprite = spriteRenderer.flipX ? move.x < -0.01f : move.x > 0.01f;
        FlipSprite(flipSprite);

        float xSpeed = Mathf.Abs(rb.velocity.x);
        animator.SetBool ("grounded", grounded);
        animator.SetBool("moving", xSpeed > 0.05);
        animator.SetFloat("x-speed", xSpeed);
        animator.SetFloat("y-speed", rb.velocity.y);

        Vector2 acceleration = move * moveSpeed;

        if (tempKnockback.magnitude > 0.1) {
            acceleration += tempKnockback;
            tempKnockback *= 0.95f;
        }

        rb.velocity += acceleration;
        if (rb.velocity.magnitude > maxSpeed) {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }

    }

    void CheckGrounded () {
        float heightShell = 0.5f;
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider2d.bounds.center, boxCollider2d.bounds.size, 0f, Vector2.down, heightShell, groundMask);
        Debug.Log(heightShell);
        grounded = raycastHit.collider != null;
    }

    public void FlipSprite(bool execute) {
        if (execute) {
            spriteRenderer.flipX = !spriteRenderer.flipX;
            flipped = spriteRenderer.flipX;
        }
    }

    public void AddKnockback(Vector2 knockback) {
        tempKnockback += knockback;
    }
}
