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

    [HideInInspector] public bool isGrounded = true;
    private bool canMove = true;
    private float horizonatalMovement;
    private float verticalMovement;

    [Header("Combat")]
    private bool canGetSecondBaseAttack = false;
    private bool canGetThirdBaseAttack = false;

    [Header("Debug")]
    [SerializeField] private float shakeAmount = 1f;

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

        /*if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            CameraController.Instance.AddCamShake(shakeAmount);
        }*/
    }

    private void LateUpdate()
    {
        AnimationHandling();
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
                isGrounded = false;
                myRB.AddForce(Vector2.up * jumpVelocity * Time.deltaTime, ForceMode2D.Impulse);
            }

            //Holding space jumps higher (also fall faster than jumping)
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
    }

    private void AnimationHandling()
    {
        //Mirroring
        if (horizonatalMovement > 0)
        {
            transform.localScale = new Vector3(3, 3, 3);
        }
        else if (horizonatalMovement < 0)
        {
            transform.localScale = new Vector3(-3, 3, 3);
        }

        //Running
        if (horizonatalMovement != 0 && myRB.velocity.y == 0)
        {
            myAnim.SetBool("IsMoving", true);
        }
        else
        {
            myAnim.SetBool("IsMoving", false);
        }

        //Not mid-air
        if (myRB.velocity.y == 0)
        {
            myAnim.SetBool("IsJumping", false);
            myAnim.SetBool("IsFalling", false);
        }

        //In the air going up
        if (myRB.velocity.y > 0)
        {
            myAnim.SetBool("IsJumping", true);
        }

        //In the air going down
        if (myRB.velocity.y < 0)
        {
            myAnim.SetBool("IsJumping", false);
            myAnim.SetBool("IsFalling", true);
        }
    }

    private void AttackController()
    {
        if (isGrounded)
        {
            //Clicking
            if (Input.GetMouseButtonDown(0))
            {
                //1st attack
                if (myAnim.GetBool("IsComboing") == false)
                {
                    myAnim.SetTrigger("AttackBaseFirst");
                }

                //2nd attack 
                if (myAnim.GetBool("IsComboing") == true && canGetSecondBaseAttack)
                {
                    myAnim.SetTrigger("AttackBaseSecond");
                }

                //2nd attack 
                if (myAnim.GetBool("IsComboing") == true && canGetThirdBaseAttack)
                {
                    myAnim.SetTrigger("AttackBaseThird");
                }
            }
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

    public void EndCombo()
    {
        myAnim.SetBool("IsComboing", false);
        canGetSecondBaseAttack = false;
        canGetThirdBaseAttack = false;
    }

    public void CanGetSecondBaseAttack()
    {
        myAnim.SetBool("IsComboing", true);
        canGetSecondBaseAttack = true;
    }

    public void CanGetThirdBaseAttack()
    {
        myAnim.SetBool("IsComboing", true);
        canGetThirdBaseAttack = true;
    }

}
