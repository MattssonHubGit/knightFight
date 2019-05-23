using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 400f;
    [Space]
    [SerializeField] private float jumpVelocity = 800f;
    [SerializeField] private float fallMultiplier = 250f;
    [SerializeField] private float lowJumpMultiplier = 200f;

    /*[HideInInspector] */public bool isGrounded = true;
    private bool canMove = true;
    private float horizonatalMovement;
    private float verticalMovement;


    [Header("Components")]
    private Rigidbody2D myRB;
    private Animator myAnim;

    void Start()
    {
        myAnim = this.GetComponent<Animator>();
        myRB = this.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        //FaceingController();
        MoveController();
        AttackController();
    }

    private void FaceingController()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (mousePos.x > transform.position.x)
        {
            transform.localScale = new Vector3(3, 3, 3);
        }
        else
        {
            transform.localScale = new Vector3(-3, 3, 3);
        }
    }

    private void MoveController()
    {
        int moveX = 0;
        if (canMove)
        {
            //Get horizontal movement
            if (Input.GetKey(KeyCode.A))
            {
                moveX--;
            }
            if (Input.GetKey(KeyCode.D))
            {
                moveX++;
            }
            horizonatalMovement = moveX * moveSpeed * Time.deltaTime;

            //Get vertical movement
            //If grounded and pressing space, jump
            if (isGrounded && Input.GetKeyDown(KeyCode.Space))
            {
                //isGrounded = false;
                myRB.AddForce(Vector2.up * jumpVelocity * Time.deltaTime, ForceMode2D.Impulse);
                myAnim.SetBool("IsJumping", true);
            }

            //Holding space jumps higher
            if (myRB.velocity.y < 0)
            {
                myRB.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1f) * Time.deltaTime;
            }
            else if (myRB.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
            {
                myRB.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1f) * Time.deltaTime;
            }

            //Put vertical and horizontal together
            Vector2 newVel = Vector2.right * horizonatalMovement;
            newVel.y = myRB.velocity.y;
            myRB.velocity = newVel;
        }


        //Set animations and mirror
        //Walking
        if (isGrounded)
        {
            myAnim.SetBool("IsFalling", false);
            myAnim.SetBool("IsJumping", false);
            if (moveX > 0)
            {
                transform.localScale = new Vector3(3, 3, 3);
                myAnim.SetBool("IsMoving", true);
            }
            else if (moveX < 0)
            {
                transform.localScale = new Vector3(-3, 3, 3);
                myAnim.SetBool("IsMoving", true);
            }
            else
            {
                myAnim.SetBool("IsMoving", false);
            }
        }
        else //Airborn
        {
            if (myRB.velocity.x > 0)
            {
                transform.localScale = new Vector3(3, 3, 3);
            }
            else if (myRB.velocity.x < 0)
            {
                transform.localScale = new Vector3(-3, 3, 3);
            }

            if (myRB.velocity.y > 0)
            {
                myAnim.SetBool("IsFalling", true);
                myAnim.SetBool("IsJumping", false);
            }
            else
            {
                myAnim.SetBool("IsFalling", false);
                myAnim.SetBool("IsJumping", true);
            }
        }

    }

    private void AttackController()
    {
        //Attack on click
        if (Input.GetMouseButtonDown(0) && isGrounded)
        {
            canMove = false;
            myAnim.Play("player_attack_B");

        }
    }

    //Add on last keyframe of attack animations
    public void DisableMovement()
    {
        myRB.velocity = Vector3.zero;
        canMove = false;
    }

    public void EnableMovement()
    {
        canMove = true;
    }

}
