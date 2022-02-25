using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public GameObject meshObject;
    public GameObject effectObject;
    public Rigidbody rb;

    void Start()
    {
        StartCoroutine(ExplosionCoroutine());
    }

    IEnumerator ExplosionCoroutine()
    {
        yield return new WaitForSeconds(3f);
        
        meshObject.SetActive(false);
        effectObject.SetActive(true);
        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, 20, Vector3.up, 0f,LayerMask.GetMask("Enemy"));

        foreach(RaycastHit hitObj in rayHits)
        {
            hitObj.transform.GetComponent<Enemy>().HitGrenade(transform.position);
        }
        Destroy(gameObject, 5);
    }
}
