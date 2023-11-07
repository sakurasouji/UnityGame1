using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_input : MonoBehaviour
{
    public Animator animator;
    public float groundCheckDistance = 0.2f;
    public float wallRaycastDistance = 0.6f;
    public ContactFilter2D groundCheckFilter;

    private Collider2D collider2d;
    private List<RaycastHit2D> groundHits = new List<RaycastHit2D>();
    private List<RaycastHit2D> wallHits = new List<RaycastHit2D>();
    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        collider2d = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float moveX = Input.GetAxisRaw(PAP.axisXinput);
        animator.SetFloat(PAP.moveX, moveX);

        bool isMoving = !Mathf.Approximately(moveX, 0f);
        animator.SetBool(PAP.isMoving, isMoving);

        //Check Ground
        bool lastOnGround = animator.GetBool(PAP.isOnGround);
        bool newOnGround = CheckIfOnGround();
        animator.SetBool(PAP.isOnGround, newOnGround);
        //Sets landing state
        if (lastOnGround == false && newOnGround == true) 
        {
            animator.SetTrigger(PAP.landedOnGround);
            animator.SetBool(PAP.isOnGround, true);
        }
        else
        {
            animator.ResetTrigger(PAP.landedOnGround);
        }
        //Check & Trigger for On Wall
        bool onWall = CheckIfOnWall();
        animator.SetBool(PAP.isOnWall, onWall);


        // JUMP
        bool isJumpKeyPressed = Input.GetButtonDown(PAP.jumpKeyName);

        if (isJumpKeyPressed) 
        {
            animator.SetTrigger(PAP.JumpTriggerName);
        }
        else
        {
            animator.ResetTrigger(PAP.JumpTriggerName);
        }



   /*     if(moveX != 0)
        {
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }
        animator.SetBool("isJumping", !isGrounded());*/
    }

    bool CheckIfOnGround()
    {
        collider2d.Cast(Vector2.down, groundCheckFilter, groundHits, groundCheckDistance);

        if(groundHits.Count > 0)
        {
            return true;
        }
        else 
        { 
            return false; 
        }
    }
    
    bool CheckIfOnWall()
    {
        Vector2 localScale = transform.localScale;

        collider2d.Raycast(Mathf.Sign(localScale.x) * Vector2.right, groundCheckFilter, wallHits, wallRaycastDistance);

        if(wallHits.Count > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

 /*   public bool isGrounded()
    {
        if (Physics2D.BoxCast(transform.position, boxSize, 0, -transform.up, castDistance, groundLayer))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

*/

    void FixedUpdate()
    {
        float forceX = animator.GetFloat(PAP.forceX);

        if (forceX != 0) rb.AddForce(new Vector2 (forceX, 0) * Time.deltaTime);
                

        float impulseY = animator.GetFloat(PAP.impulseY);
        float impulseX = animator.GetFloat(PAP.impulseX);

        //if (impulseY != 0) rb.AddForce(new Vector2(0, impulseY), ForceMode2D.Impulse);

        if(impulseY != 0 || impulseX != 0){
          
            float xDirectionSign = Mathf.Sign(transform.localScale.x);
            Vector2 impulseVector = new Vector2(xDirectionSign * impulseX, impulseY);

            rb.AddForce(impulseVector, ForceMode2D.Impulse);
            animator.SetFloat(PAP.impulseX, 0);
            animator.SetFloat(PAP.impulseY, 0);
         
         }

        animator.SetFloat (PAP.velocityY, rb.velocity.y);

        bool isStopVelocity = animator.GetBool(PAP.stopVelocity);

        if (isStopVelocity)
        {
            rb.velocity = Vector2.zero;
            animator.SetBool(PAP.stopVelocity, false);
        }

    }

}
