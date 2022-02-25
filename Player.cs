using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject[] weapons;
    public GameObject tGrenade;
    public GameManager gm;
    private Rigidbody rb;
    public Weapon equipWeapon;
    public float moveSpeed = 10f;
    public bool[] haveWeapons;
    public int haveGrenades;
    public int heart;
    public int score;
    public int ammo;
    public int coin;

    public int maxAmmo;
    public int maxHeart;
    public int maxCoin;
    public int maxGrenades;

    MeshRenderer[] mr;
    GameObject nearObject;
    Animator anim;
    
    Vector3 inputDir;

    float attackDelay;

    bool walkDown;
    bool jumpDown;
    bool dodgeDown;
    bool interactionDown;
    bool swapDown1;
    bool swapDown2;
    bool swapDown3;
    bool attackDown;
    bool reloadDown;
    bool grenadeDown;

    bool isDodge;
    bool isSwap;
    bool isAttack = true;
    bool isReload;
    bool isDamage;
    bool isShop;
    bool isDead;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        mr = GetComponentsInChildren<MeshRenderer>();

        PlayerPrefs.SetInt("MaxScore", 112500);
    }
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        walkDown = Input.GetButton("Walk");
        jumpDown = Input.GetButtonDown("Jump");
        dodgeDown = Input.GetButtonDown("Dodge");
        interactionDown = Input.GetButtonDown("Interaction");
        swapDown1 = Input.GetButtonDown("Swap1");
        swapDown2 = Input.GetButtonDown("Swap2");
        swapDown3 = Input.GetButtonDown("Swap3");
        attackDown = Input.GetButton("Fire1");
        grenadeDown = Input.GetButtonDown("Fire2");
        reloadDown = Input.GetButtonDown("Reload");

        if (!isDodge)
        {
            inputDir = new Vector3(h, 0, v).normalized;
        }
        if (isSwap || isReload || !isAttack || isDead)
        {
            inputDir = new Vector3(0, 0, 0);
        }
        transform.position += inputDir * moveSpeed * (walkDown ? 0.3f : 1f) * Time.deltaTime;
        transform.LookAt(transform.position + inputDir);
        if(attackDown && !isDodge && !isDead)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
           
            if(Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 fireDir = rayHit.point - transform.position;
                fireDir.y = 0; 
                transform.LookAt(transform.position + fireDir);
            }
        }
        anim.SetBool("isRun", inputDir != Vector3.zero);
        anim.SetBool("isWalk", walkDown);

        Jump();
        Dodge();
        Interaction();
        WeaponSwap();
        Attack();
        Reload();
        Grenade();
    }
    void FixedUpdate()
    {
        rb.angularVelocity = Vector3.zero;
    }
    void Jump()
    {
        if (jumpDown && !isDodge && !isSwap && !isDead)
        {
            if (rb.velocity.y != 0)
            {
                return;
            }
            rb.AddForce(Vector3.up * 15, ForceMode.Impulse);
            anim.SetBool("isLand", true);
            anim.SetTrigger("isJump");
        }
    }
    void Grenade()
    {
        if(haveGrenades == 0)
        {
            return;
        }

        if(grenadeDown && !isReload && !isSwap && !isDead)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;

            if (Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 fireDir = rayHit.point - transform.position;
                fireDir.y = 15;
                GameObject instantGrenade = Instantiate(tGrenade, transform.position, transform.rotation);

                Rigidbody rbGrenade = instantGrenade.GetComponent<Rigidbody>();
                rbGrenade.AddForce(fireDir, ForceMode.Impulse);

                haveGrenades--;
            }
        }
    }
    void Dodge()
    {
        if (dodgeDown && !jumpDown && !isDodge && !isSwap && !isDead)
        {
            moveSpeed *= 2;
            anim.SetTrigger("isDodge");
            isDodge = true;
            Invoke("DodgeOut", 0.5f);
        }
    }
    void DodgeOut()
    {
        moveSpeed *= 0.5f;
        isDodge = false;
    }

    void WeaponSwap()
    {
        int weaponIndex = -1;

        if (swapDown1) weaponIndex = 0;
        if (swapDown2) weaponIndex = 1;
        if (swapDown3) weaponIndex = 2;
        
        if ((swapDown1 || swapDown2 || swapDown3) && 
            haveWeapons[weaponIndex] && 
            equipWeapon != weapons[weaponIndex] && 
            !jumpDown && !isDodge)
        {
            if(equipWeapon != null)
            {
                equipWeapon.gameObject.SetActive(false);
            }
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true);
            anim.SetTrigger("isSwap");
            isSwap = true;
            Invoke("SwapOut", 0.3f);
        }
    }
    void SwapOut()
    { 
        isSwap = false;
    }
    void Interaction()
    {
        if (interactionDown && nearObject != null && !jumpDown && !isDodge && !isDead)
        {
            if(nearObject.tag == "Weapon")
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.offset;
                haveWeapons[weaponIndex] = true;
                Destroy(nearObject);
            }
            else if(nearObject.tag == "Shop")
            {
                Shop shop = nearObject.GetComponent<Shop>();
                shop.InPlayer(this);
                isShop = true;
            }
        }
    }
    void Attack()
    {
        if (equipWeapon == null)
        {
            return;
        }
            attackDelay += Time.deltaTime;
            isAttack = equipWeapon.attackSpeed < attackDelay;
        
        if(attackDown && isAttack && !isDodge && !isSwap && !isShop && !isDead)
        {
            equipWeapon.UseWeapon();
            anim.SetTrigger(equipWeapon.attackType == Weapon.Type.MeleeAttack ? "isMeleeAttack" : "isRangeAttack");
            attackDelay = 0;
        }
    }
    void Reload()
    {
        if (equipWeapon == null ||
            equipWeapon.attackType == Weapon.Type.MeleeAttack ||
            ammo == 0)
        {
            return;
        }

        if(reloadDown && !jumpDown && !isDodge && !isSwap && isAttack && !isShop && !isDead)
        {
            anim.SetTrigger("isReload");
            isReload = true;

            Invoke("ReloadOut", 2f);
        }
    }
    void ReloadOut()
    {
        int reloadAmmo = equipWeapon.maxAmmo - equipWeapon.currentAmmo;
        
        if(ammo < reloadAmmo)
        {
            reloadAmmo = ammo;
        }
        equipWeapon.currentAmmo += reloadAmmo;
        ammo -= reloadAmmo;
        isReload = false;
    }
    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            anim.SetBool("isLand", false);
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Item")
        {
            Item item = other.GetComponent<Item>();

            switch (item.itemType)
            {
                case Item.ItemType.Grenade:
                {
                    haveGrenades += item.offset;
                        if(haveGrenades >= maxGrenades)
                        {
                            haveGrenades = maxGrenades;
                        }
                }
                break;
                case Item.ItemType.Ammo:
                {
                    ammo += item.offset;
                        if(ammo >= maxAmmo)
                        {
                            ammo = maxAmmo;
                        }
                }
                break;
                case Item.ItemType.Heart:
                {
                    heart += item.offset;
                        if (heart >= maxHeart)
                        { 
                           heart = maxHeart;
                        }
                }
                break;
                case Item.ItemType.Coin:
                {
                    coin += item.offset;
                    if (coin >= maxCoin)
                    {
                        coin = maxCoin;
                    }
                }
                break; 
            }
            Destroy(other.gameObject);
        }
        else if(other.tag == "EnemyBullet")
        {
            if(!isDamage)
            {
                Bullet enemyBullet = other.GetComponent<Bullet>();
                heart -= enemyBullet.damage;

                bool isBossAttack = other.name == "TauntRange";
                StartCoroutine(DamageCoroutine(isBossAttack));
            }
            if (other.GetComponent<Rigidbody>() != null)
            {
                Destroy(other.gameObject);
            }
        }
    }
    IEnumerator DamageCoroutine(bool isBossAttack)
    {
        isDamage = true;
        foreach(MeshRenderer meshs in mr)
        {
            meshs.material.color = Color.gray;
        }
        if(isBossAttack)
        {
            rb.AddForce(transform.forward * -25, ForceMode.Impulse);
        }
        if (heart <= 0 && !isDead)
        {
            Die();
        }
        yield return new WaitForSeconds(1f);

        isDamage = false;
        foreach (MeshRenderer meshs in mr)
        {
            meshs.material.color = Color.white;
        }
        if (isBossAttack)
        {
            rb.velocity = Vector3.zero;
        }
        
    }
    void Die()
    {
        anim.SetTrigger("isDie");
        isDead = true;
        gm.GameOver();
    }
    void OnTriggerStay(Collider other)
    {
        if(other.tag == "Weapon" || other.tag == "Shop")
        {
            nearObject = other.gameObject;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if(other.tag == "Weapon")
        {
            nearObject = null;
        }
        else if(other.tag == "Shop")
        {
            Shop shop = nearObject.GetComponent<Shop>();
            shop.ExitPlayer();
            isShop = false;
            nearObject = null;
        }
    }
}
