using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundChecker : MonoBehaviour
{
    private PlayerController myPlayer;

    private void Start()
    {
        myPlayer = GetComponentInParent<PlayerController>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Ground")
        {
            myPlayer.isGrounded = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Ground")
        {
            myPlayer.isGrounded = false;
        }
    }
}
