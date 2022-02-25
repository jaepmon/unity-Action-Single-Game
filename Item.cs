using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum ItemType
    {
        Grenade,
        Weapon,
        Ammo,
        Heart,
        Coin
    };
    public ItemType itemType;

    SphereCollider sc;
    Rigidbody rb;

    public int offset;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        sc = GetComponent<SphereCollider>();
    }
    void Update()
    {
        transform.Rotate(Vector3.up * 20 * Time.deltaTime);
    }
    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            rb.isKinematic = true;
            sc.enabled = false;
        }
    }
}
