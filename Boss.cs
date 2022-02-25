using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : Enemy
{
    public GameObject bossMissile;
    public Transform shotPosA;
    public Transform shotPosB;
    public bool isLook;

    Vector3 lookDir;
    Vector3 tauntDir;

    protected override void Awake()
    {
        base.Awake();
        nav.isStopped = true;
        StartCoroutine(RandomActCoroutine());
    }

    void Update()
    {
        if(isDead)
        {
            StopAllCoroutines();
            return;
        }
        if(isLook)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            lookDir = new Vector3(h, 0, v) * 5f;
            transform.LookAt(target.position + lookDir);
        }
        else
        {
            nav.SetDestination(tauntDir);
        }
    }
    IEnumerator RandomActCoroutine()
    {
        yield return new WaitForSeconds(0.2f);

        int randomAct = Random.Range(0, 5);
        switch (randomAct)
        {
            case 0:
            case 1:
                {
                    StartCoroutine(MissileShotCoroutine());
                }   
            break;
            case 2:
            case 3:
                {
                    StartCoroutine(RockShotCoroutine());
                }
            break;
            case 4:
                {
                    StartCoroutine(TauntCoroutine());
                }
            break;
        }
    }

    IEnumerator MissileShotCoroutine()
    {
        anim.SetTrigger("isShot");

        yield return new WaitForSeconds(0.3f);
        GameObject instantBossMissileA = Instantiate(bossMissile, shotPosA.position, shotPosA.rotation);
        BossBullet bossMissileaA = instantBossMissileA.GetComponent<BossBullet>();
        bossMissileaA.target = target;

        yield return new WaitForSeconds(0.3f);
        GameObject instantBossMissileB = Instantiate(bossMissile, shotPosB.position, shotPosB.rotation);
        BossBullet bossMissileaB = instantBossMissileB.GetComponent<BossBullet>();
        bossMissileaB.target = target;

        yield return new WaitForSeconds(2.5f);
        StartCoroutine(RandomActCoroutine());
    }
    IEnumerator RockShotCoroutine()
    {
        isLook = false;
        anim.SetTrigger("isBigShot");
        Instantiate(missile, transform.position, transform.rotation);

        yield return new WaitForSeconds(3f);
        isLook = true;
        StartCoroutine(RandomActCoroutine());
    }
    IEnumerator TauntCoroutine()
    {
        tauntDir = target.position + lookDir;
        isLook = false;
        nav.isStopped = false;
        bc.enabled = false;
        anim.SetTrigger("isTaunt");

        yield return new WaitForSeconds(1.5f);
        meleeRange.enabled = true;

        yield return new WaitForSeconds(0.5f);
        meleeRange.enabled = false;

        yield return new WaitForSeconds(1f);
        isLook = true;
        nav.isStopped = true;
        bc.enabled = true;
        StartCoroutine(RandomActCoroutine());
    }
}
