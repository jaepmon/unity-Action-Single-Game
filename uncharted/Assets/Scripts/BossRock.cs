using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRock : Bullet
{
    Rigidbody rb;

    float rockAcceleration = 2;
    float rockScale = 0.1f;

    bool isShot;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(ChargeTimerCoroutine());
        StartCoroutine(ChargePower());
    }

    IEnumerator ChargeTimerCoroutine()
    {
        yield return new WaitForSeconds(2.5f);
        isShot = true;
    }

    IEnumerator ChargePower()
    {
        while(!isShot)
        {
            rockAcceleration += 0.03f;
            rockScale += 0.005f;
            transform.localScale = Vector3.one * rockScale;
            rb.AddTorque(transform.right * rockAcceleration, ForceMode.Acceleration);
            yield return null;
        }
    }
}
