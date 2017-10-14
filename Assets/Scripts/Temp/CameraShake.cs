using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public bool debugMode = false;//Test-run/Call ShakeCamera() on start

    public float shakeAmount;//The amount to shake this frame.
    public float shakeTimeLeft;//The duration this frame.

    //Readonly values...
    float shakePercentage;//A percentage (0-1) representing the amount of shake to be applied when setting rotation.
    float startAmount;//The initial shake amount (to determine percentage), set when ShakeCamera is called.
    float startDuration;//The initial shake duration, set when ShakeCamera is called.

    bool isPlaying = false; //Is the coroutine running right now?

    public bool smooth;//Smooth rotation?
    public float smoothAmount = 5f;//Amount to smooth

    void Start()
    {
        if (debugMode) ShakeCamera();
    }

    void ShakeCamera()
    {
        startAmount = shakeAmount;//Set default (start) values
        startDuration = shakeTimeLeft;//Set default (start) values

        if (!isPlaying) StartCoroutine(Shake());//Only call the coroutine if it isn't currently running. Otherwise, just set the variables.
    }

    public void ShakeCamera(float amount, float duration)
    {
        shakeAmount = amount;//Add to the current amount.
        startAmount = shakeAmount;//Reset the start amount, to determine percentage.
        shakeTimeLeft = duration;//Add to the current time.
        startDuration = shakeTimeLeft;//Reset the start time.

        if (!isPlaying) StartCoroutine(Shake());//Only call the coroutine if it isn't currently running. Otherwise, just set the variables.
    }

    IEnumerator Shake()
    {
        isPlaying = true;
        Vector3 originalRot = transform.eulerAngles;

        while (shakeTimeLeft > 0f)
        {
            Vector3 rotationAmount = Random.insideUnitSphere * shakeAmount;//A Vector3 to add to the Local Rotation
            //rotationAmount.z = 0;//Don't change the Z; it looks funny.

            shakePercentage = shakeTimeLeft / startDuration;//Used to set the amount of shake (% * startAmount).

            shakeAmount = startAmount * shakePercentage;//Set the amount of shake (% * startAmount).
            shakeTimeLeft -= Time.deltaTime;//Lerp the time, so it is less and tapers off towards the end.

            //Vector3 newRot = smooth ? Vector3.Lerp(transform.eulerAngles, rotationAmount, Time.deltaTime * smoothAmount) : Vector3.Cross(transform.eulerAngles, rotationAmount);

            //transform.localRotation = Quaternion.Euler(newRot);
            transform.eulerAngles = originalRot + rotationAmount;
        
            //if (smooth)
            //    transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(rotationAmount), Time.deltaTime * smoothAmount);
            //else
            //    transform.localRotation = transform.localRotation * Quaternion.Euler(rotationAmount);//Set the local rotation the be the rotation amount.

            yield return null;
        }
        transform.localRotation = Quaternion.Euler(0.0f, transform.localRotation.eulerAngles.y, 0.0f);
        isPlaying = false;
    }
}
