using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public bool isMelee;
    public bool isRock;
    public int damage;
    void OnCollisionEnter(Collision collision)
    {
        if(!isRock && gameObject.tag != "EnemyBullet" && collision.gameObject.tag == "Ground")
        {
            Destroy(gameObject, 3);
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (!isRock && !isMelee && other.gameObject.tag == "Wall")
        {
            Destroy(gameObject, 3);
        }
    }
}
