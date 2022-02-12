using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum Type
    {
        Normal,
        Dash,
        Range,
        Boss
    };
    public Type enemyType;

    public BoxCollider meleeRange;
    public GameManager gm;
    public GameObject missile;
    public GameObject[] coins;
    public Transform target;
    public bool isChase;
    public bool isAttack;
    public bool isDead;
    public int currentHealth;
    public int maxHealth;
    public int score;

    protected Rigidbody rb;
    protected BoxCollider bc;
    protected MeshRenderer[] mr;
    protected NavMeshAgent nav;
    protected Animator anim;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        bc = GetComponent<BoxCollider>();
        mr = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        if(enemyType != Type.Boss)
        {
            Invoke("ChaseStart", 2);
        }   
    }
    void Update()
    {
        if(enemyType != Type.Boss && nav.enabled)
        {
            nav.SetDestination(target.position);
            nav.isStopped = !isChase;
        }
    }
    void FixedUpdate()
    {
        Targeting();
        if(isChase)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
    void Targeting()
    {
        if(!isDead && enemyType != Type.Boss)
        {
            float targetRadious = 0;
            float targetRange = 0;

            switch (enemyType)
            {
                case Type.Normal:
                    {
                        targetRadious = 1.5f;
                        targetRange = 3f;
                    }
                    break;
                case Type.Dash:
                    {
                        targetRadious = 1f;
                        targetRange = 15f;
                    }
                    break;
                case Type.Range:
                    {
                        targetRadious = 0.5f;
                        targetRange = 25f;
                    }
                    break;
            }
            RaycastHit[] rayHit = Physics.SphereCastAll(transform.position,
                targetRadious, transform.forward, targetRange, LayerMask.GetMask("Player"));
            if (rayHit.Length > 0 && !isAttack)
            {
                StartCoroutine(AttackCoroutine());
            }
        } 
    }
    IEnumerator AttackCoroutine()
    {
        isChase = false;
        isAttack = true;
        anim.SetBool("isAttack", true);

        switch (enemyType)
        {
            case Type.Normal:
                {
                    yield return new WaitForSeconds(0.2f);

                    meleeRange.enabled = true;
                    yield return new WaitForSeconds(2f);

                    meleeRange.enabled = false;
                    yield return new WaitForSeconds(1f);
                }
            break;
            case Type.Dash:
                {
                    yield return new WaitForSeconds(0.1f);

                    rb.AddForce(transform.forward * 20, ForceMode.Impulse);
                    meleeRange.enabled = true;
                    yield return new WaitForSeconds(0.5f);

                    rb.velocity = Vector3.zero;
                    meleeRange.enabled = false;
                    yield return new WaitForSeconds(2f);
                }
            break;
            case Type.Range:
                {
                    yield return new WaitForSeconds(0.5f);

                    GameObject instantMissile = Instantiate(missile, transform.position, transform.rotation);
                    Rigidbody rbMissile = instantMissile.GetComponent<Rigidbody>();
                    rbMissile.velocity = transform.forward * 20;
                    yield return new WaitForSeconds(2f);
                }
            break;
        }
        isChase = true;
        isAttack = false;
        anim.SetBool("isAttack", false);
    }
    void ChaseStart()
    {
        isChase = true;
        anim.SetBool("isWalk", true);
    }
    void OnTriggerEnter(Collider other)
    {
        if(!isDead && other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            currentHealth -= weapon.damage;
            Vector3 knockBack = transform.position - other.transform.position;
            StartCoroutine(HitCoroutine(knockBack, false));
        }
        else if (!isDead && other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            currentHealth -= bullet.damage;
            Vector3 knockBack = transform.position - other.transform.position;
            Destroy(other.gameObject);
            StartCoroutine(HitCoroutine(knockBack, false));
        }
    }
    public void HitGrenade(Vector3 explosionPos)
    {
        currentHealth -= 50;
        Vector3 knockBack = transform.position - explosionPos;
        StartCoroutine(HitCoroutine(knockBack, true));
    }
    IEnumerator HitCoroutine(Vector3 knockBack, bool isGrenade)
    {
        foreach(MeshRenderer mesh in mr)
        {
            mesh.material.color = Color.red;
        }
        yield return new WaitForSeconds(0.1f);

        if(currentHealth > 0)
        {
            foreach (MeshRenderer mesh in mr)
            {
                mesh.material.color = Color.white;
            }
        }
        else
        {
            foreach (MeshRenderer mesh in mr)
            {
                mesh.material.color = Color.black;
            }

            gameObject.layer = 11;
            isDead = true;
            isChase = false;
            nav.enabled = false;
            anim.SetTrigger("isDie");
            Player player = target.GetComponent<Player>();
            player.score += score;
            int randomCoin = Random.Range(0, 3);
            Instantiate(coins[randomCoin], transform.position, Quaternion.identity);

            switch (enemyType)
            {
                case Type.Normal:
                {
                    gm.enemyNormal -= 1;
                }
                break;
                case Type.Dash:
                {
                    gm.enemyDash--;
                }
                break;
                case Type.Range:
                {
                    gm.enemyRange--;
                }
                break;
                case Type.Boss:
                {
                    gm.enemyBoss--;
                }
                break;
            }

            if (isGrenade)
            {
                knockBack = knockBack.normalized;
                knockBack += Vector3.up * 5;
                rb.freezeRotation = false;
                rb.AddForce(knockBack * 10, ForceMode.Impulse);
                rb.AddTorque(knockBack * 15, ForceMode.Impulse);
            }
            else
            {
                knockBack = knockBack.normalized;
                knockBack += Vector3.up;
                rb.AddForce(knockBack * 5, ForceMode.Impulse);
            }
            Destroy(gameObject, 3);         
        }
    }
}
