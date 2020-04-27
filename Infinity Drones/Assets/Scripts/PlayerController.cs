using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : PhysicsBody
{

    [SerializeField] float maxSpeed = 20f;
    [SerializeField] float moveSpeed = 7f;
    [SerializeField] float jumpTakeOffSpeed = 6.2f;

    Vector2 tempKnockback = Vector2.zero;

    SpriteRenderer spriteRenderer;
    Animator animator;

    public bool flipped;

    // Start is called before the first frame update
    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        flipped = spriteRenderer.flipX;
        animator = GetComponent<Animator>();
        shellRadius = 0.04f;
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

        bool flipSprite = flipped ? move.x < -0.01f : move.x > 0.01f;
        FlipSprite(flipSprite);

        float xSpeed = Mathf.Abs(velocity.x);
        animator.SetBool ("grounded", grounded);
        animator.SetBool("moving", xSpeed > 0.05);
        animator.SetFloat("x-speed", xSpeed);
        animator.SetFloat("y-speed", velocity.y);

        targetVelocity = move * moveSpeed;

        if (tempKnockback.magnitude > 0.1) {
            // Applce the stuff
            targetVelocity.x += tempKnockback.x;
            velocity.y += tempKnockback.y / 3;
            // Because tempKnockback.y isn't reset every frame, it needs to be reset here
            tempKnockback.y = 0f;
            // Slowly decrease the x knockback
            tempKnockback.x *= 0.9f;
        }
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
