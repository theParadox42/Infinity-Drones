using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : PhysicsBody
{

    public float maxSpeed = 7f;
    public float jumpTakeOffSpeed = 5f;

    private SpriteRenderer spriteRenderer;
    private Animator animator;

    // Start is called before the first frame update
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }
    
    protected override void ComputeVelocity() {
        Vector2 move = Vector2.zero;
        move.x = Input.GetAxis("Horizontal");

        if (Input.GetButtonDown("Jump") && grounded) {
            animator.SetTrigger("jump");
            velocity.y = jumpTakeOffSpeed;
        } else if(Input.GetButtonUp("Jump")) {
            if (velocity.y > 0) {
                velocity.y = velocity.y * .5f;
            }
        }

        bool flipSprite = spriteRenderer.flipX ? move.x < -0.01f : move.x > 0.01f;
        if (flipSprite) {
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }

        float xSpeed = Mathf.Abs(velocity.x);
        animator.SetBool ("grounded", grounded);
        animator.SetBool("moving", xSpeed > 0.05);
        animator.SetFloat("x-speed", xSpeed);
        animator.SetFloat("y-speed", velocity.y);

        targetVelocity = move * maxSpeed;

    }

}
