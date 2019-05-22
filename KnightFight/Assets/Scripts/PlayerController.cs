using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 1f;
    [HideInInspector] private bool canMove = true;

    private Animator myAnim;

    void Start()
    {
        myAnim = GetComponent<Animator>();
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
        //Get intput
        int moveX = 0;
        int moveY = 0;

        if (Input.GetKey(KeyCode.A))
        {
            moveX--;
        }

        if (Input.GetKey(KeyCode.D))
        {
            moveX++;
        }


        if (canMove)
        {
            //Execute input
            transform.position += new Vector3(moveX, moveY, 0) * moveSpeed * Time.deltaTime;

            //Set animations
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


    }

    private void AttackController()
    {
        //Attack on click
        if (Input.GetMouseButtonDown(0))
        {
            canMove = false;
            myAnim.Play("player_attack_B");

        }
    }

    //Add on last keyframe of attack animations
    public void AttackAnimationOver()
    {
        canMove = true;
    }

}
