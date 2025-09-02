using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake inst;
    private const float shakeAmount = 0.1f;
    public float decreaseFactor = 1;
    private float shakeDuration = 0;
    private Vector3 defaultPos;

    void Awake()
    {
        if (inst == null) inst = this;
        else Destroy(gameObject);

        defaultPos = transform.localPosition;      
    }

    void Update()
    {
        //When shake duration is set to a value greater than 0,
        //the camera's local position will be set to its default position plus a little shift in a random direction
        if (shakeDuration > 0)
        {
            transform.localPosition  = defaultPos + (Vector3)Random.insideUnitCircle * shakeAmount;
            //Shake durstion gets decreased until it reaches zero
            shakeDuration -= Time.deltaTime * decreaseFactor;


        }
        else
        {
            //Once the shake duration is zero, we return the camera to its original position.
            transform.localPosition = defaultPos;
        }
    }

    // Below is the public method that can be called from any script to initiate a camera shake.
    // This works by simply passing the duration of the camera shake desired (in seconds).
    public void Shake(float duration)
    {
        shakeDuration = duration;
    }

}
