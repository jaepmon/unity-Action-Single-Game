using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type
    {
        MeleeAttack,
        RangeAttack
    };

    public Type attackType;

    public TrailRenderer meleeAttackEffect;
    public BoxCollider meleeAttackRange;
    public GameObject bullet;
    public GameObject bulletCase;
    public Transform bulletCasePos;
    public Transform bulletPos;
    public float attackSpeed;
    public int damage;

    public int maxAmmo;
    public int currentAmmo;
    public void UseWeapon()
    {
        if(attackType == Type.MeleeAttack)
        {
            StopCoroutine("MeleeAttackCoroutine");
            StartCoroutine("MeleeAttackCoroutine");
        }
        if(attackType == Type.RangeAttack && currentAmmo > 0)
        {
            currentAmmo--;
            StartCoroutine("RangeAttackCoroutine");
        }
    }

    IEnumerator MeleeAttackCoroutine()
    {
        yield return new WaitForSeconds(0.2f);
        
        meleeAttackRange.enabled = true;
        meleeAttackEffect.enabled = true;

        yield return new WaitForSeconds(0.3f);
        meleeAttackRange.enabled = false;

        yield return new WaitForSeconds(0.3f);
        meleeAttackEffect.enabled = false;
    }
    IEnumerator RangeAttackCoroutine()
    {
        GameObject createBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        Rigidbody rbBullet = createBullet.GetComponent<Rigidbody>();
        rbBullet.velocity = bulletPos.forward * 30;

        yield return null;

        GameObject createBulletCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody rbBulletCase = createBulletCase.GetComponent<Rigidbody>();
        Vector3 bulletCaseVec = (bulletCasePos.forward * -2) + (Vector3.up * 2);
        rbBulletCase.AddForce(bulletCaseVec, ForceMode.Impulse);
    }
}
