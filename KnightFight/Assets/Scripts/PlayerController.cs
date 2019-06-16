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
    [SerializeField]private bool canMove = true;
    private float horizonatalMovement;
    private float verticalMovement;

    [Header("Combat")]
    bool canBaseAttack = true; //Locks ability to attack during animation
    int amountBaseCombo = 0; //Determines animation to play

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
                BaseAttackStarter();
            }
        }

    }

    private void BaseAttackStarter()
    {
        if (canBaseAttack)
        {
            amountBaseCombo++;
        }

        if (amountBaseCombo == 1)
        {
            myAnim.SetInteger("basicComboCounter", 1);
        }
    }

    public void BaseAttackComboCheck()
    {
        canBaseAttack = false;

        //1st attack
        if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("player_attack_B") && amountBaseCombo == 1)
        {
            //If the 1st animation is still playing and only 1 click has happend, return to idle
            myAnim.SetInteger("basicComboCounter", 4);
            canBaseAttack = true;
            amountBaseCombo = 0;
        }
        else if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("player_attack_B") && amountBaseCombo >= 2)
        {
            //If the 1st animation is still playing and at least 2 clicks has happend, continue combo
            myAnim.SetInteger("basicComboCounter", 2);
            canBaseAttack = true;
        }
        //2nd attack
        else if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("player_attack_A") && amountBaseCombo == 2)
        {
            //If the 2nd animation is still playing and only 2 clicks has happend, return to idle
            myAnim.SetInteger("basicComboCounter", 4);
            canBaseAttack = true;
            amountBaseCombo = 0;
        }
        else if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("player_attack_A") && amountBaseCombo >= 3)
        {
            //If the 2nd animation is still playing and at least 3 clicks has happend, continue combo
            myAnim.SetInteger("basicComboCounter", 3);
            canBaseAttack = true;
        }
        //3rd attack
        else if(myAnim.GetCurrentAnimatorStateInfo(0).IsName("player_attack_C"))
        {
            //The 3rd attack is the last in the combo, retun to idle
            myAnim.SetInteger("basicComboCounter", 4);
            canBaseAttack = true;
            amountBaseCombo = 0;
        }
    }

    //Add on last keyframe of attack animations
    public void DisableMovement()
    {
        Debug.Log("PlayerController::DisableMovement -- Event called");

        myRB.velocity = Vector3.zero;
        canMove = false;
    }

    public void EnableMovement()
    {
        canMove = true;
    }
    
}
