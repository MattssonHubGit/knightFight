using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Singleton")]
    public static CameraController Instance;

    [Header("Shake")]
    [SerializeField] private float shakeDecay = 0.5f;
    private float shakeBuffer = 0.01f;
    private float shakeAmount;
    private bool shouldShake = false;
    private Vector3 storedPos;

    [Header("Motion")]
    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;

        //Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Update()
    {
        ShakeManager();
    }

    //Add a amount of screenshake
    public void AddCamShake(float _amount)
    {
        shakeAmount = _amount * shakeBuffer;
        shouldShake = true;
    }

    private void ShakeManager()
    {
        //If shaking, pick a random position at a certain distance, this distance shrinks until not shaking anymore
        if (shouldShake == true)
        {
            transform.position = new Vector3(storedPos.x + (Random.Range(-shakeAmount, shakeAmount)), storedPos.y + (Random.Range(-shakeAmount, shakeAmount)), -10f);

            shakeAmount -= shakeDecay * Time.deltaTime;
            if (shakeAmount <= 0)
            {
                shakeAmount = 0;
                shouldShake = false;
                transform.position = storedPos;
            }
        }
        else
        {
            transform.position = startPos;
        }
    }
}
