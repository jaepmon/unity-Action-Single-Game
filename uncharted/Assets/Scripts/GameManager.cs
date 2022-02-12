using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public RectTransform bossStatusContainer;
    public RectTransform bossHPbar;
    public GameObject menuContainer;
    public GameObject gameContainer;
    public GameObject gameOverContainer;
    public GameObject itemShop;
    public GameObject weaponShop;
    public GameObject startZone;
    public GameObject[] enemies;
    public Transform[] enemyRespawnZone;
    public Player player;
    public Image WeaponImg1;
    public Image WeaponImg2;
    public Image WeaponImg3;
    public Image WeaponImgR;
    public Boss boss;
    public Text maxScoreTxt;
    public Text scoreTxt;
    public Text stageTxt;
    public Text playTimeTxt;
    public Text playerHeartTxt;
    public Text playerAmmoTxt;
    public Text playerCoinTxt;
    public Text enemyNormalTxt;
    public Text enemyDashTxt;
    public Text enemyRangeTxt;
    public List<int> enemySpawnList;

    public float inGameTime;
    public bool isBattle;
    public int stage;
    public int enemyNormal;
    public int enemyDash;
    public int enemyRange;
    public int enemyBoss;

    public void Awake()
    {
        enemySpawnList = new List<int>();
        maxScoreTxt.text = string.Format("{0:n0}", PlayerPrefs.GetInt("MaxScore"));

        if(PlayerPrefs.HasKey("MaxScore"))
        {
            PlayerPrefs.SetInt("MaxScore", 0);
        }
    }
    public void Update()
    {
        if(isBattle)
        {
            inGameTime += Time.deltaTime;
        }
    }
    public void GameStart()
    {
        menuContainer.SetActive(false);
        gameContainer.SetActive(true);

        player.gameObject.SetActive(true);
    }
    public void GameOver()
    {
        gameContainer.SetActive(false);
        gameOverContainer.SetActive(true);
    }
    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
    public void StageStart()
    {
        isBattle = true;
        itemShop.SetActive(false);
        weaponShop.SetActive(false);
        startZone.SetActive(false);

        foreach(Transform enemyZone in enemyRespawnZone)
        {
            enemyZone.gameObject.SetActive(true);
        }

        StartCoroutine(BattleInCoroutine());
    }
    public void StageClear()
    {
        player.transform.position = Vector3.up * 1.8f;
        isBattle = false;
        itemShop.SetActive(true);
        weaponShop.SetActive(true);
        startZone.SetActive(true);

        foreach (Transform enemyZone in enemyRespawnZone)
        {
            enemyZone.gameObject.SetActive(false);
        }

        stage++;
    }
    IEnumerator BattleInCoroutine()
    {
        if(stage % 5 == 0)
        {
            enemyBoss++;
            GameObject instantEnemy = Instantiate(enemies[3], enemyRespawnZone[1].position, enemyRespawnZone[1].rotation);
            Enemy enemy = instantEnemy.GetComponent<Enemy>();
            enemy.target = player.transform;
            enemy.gm = this;
            boss = instantEnemy.GetComponent<Boss>();
        }
        else
        {
            for (int i = 0; i < stage; i++)
            {
                int random = Random.Range(0, 3);
                enemySpawnList.Add(random);

                switch (random)
                {
                    case 0:
                        {
                            enemyNormal++;
                        }
                        break;
                    case 1:
                        {
                            enemyDash++;
                        }
                        break;
                    case 2:
                        {
                            enemyRange++;
                        }
                        break;
                }
            }

            while (enemySpawnList.Count > 0)
            {
                int randomZone = Random.Range(0, 4);
                GameObject instantEnemy = Instantiate(enemies[enemySpawnList[0]], enemyRespawnZone[randomZone].position, enemyRespawnZone[randomZone].rotation);
                Enemy enemy = instantEnemy.GetComponent<Enemy>();
                enemy.target = player.transform;
                enemy.gm = this;
                enemySpawnList.RemoveAt(0);

                yield return new WaitForSeconds(3f);
            }
        }
        while(enemyNormal + enemyDash + enemyRange > 0)
        {
            yield return null;
        }
        yield return new WaitForSeconds(4f);
        boss = null;
        StageClear();
    }
    void LateUpdate()
    {
        scoreTxt.text = string.Format("{0:n0}", player.score);
        playerHeartTxt.text = player.heart + " / " + player.maxHeart;
        playerCoinTxt.text = string.Format("{0:n0}", player.coin);
        stageTxt.text = "STAGE " + stage;

        int hour = (int)(inGameTime / 3600);
        int min = (int)((inGameTime - hour * 3600) / 60);
        int sec = (int)(inGameTime % 60);
        playTimeTxt.text = string.Format("{0:00}", hour) + ":" + string.Format("{0:00}", min) + ":" + string.Format("{0:00}", sec);

        if (player.equipWeapon == null)
        {
            playerAmmoTxt.text = "- / " + player.ammo;
        }
        else if (player.equipWeapon.attackType == Weapon.Type.MeleeAttack)
        {
            playerAmmoTxt.text = "- / " + player.ammo;
        }
        else
        {
            playerAmmoTxt.text = player.equipWeapon.currentAmmo + " / " + player.ammo;
        }
        WeaponImg1.color = new Color(1, 1, 1, player.haveWeapons[0] ? 1 : 0);
        WeaponImg2.color = new Color(1, 1, 1, player.haveWeapons[1] ? 1 : 0);
        WeaponImg3.color = new Color(1, 1, 1, player.haveWeapons[2] ? 1 : 0);
        WeaponImgR.color = new Color(1, 1, 1, player.haveGrenades > 0 ? 1 : 0);

        enemyNormalTxt.text = enemyNormal.ToString();
        enemyDashTxt.text = enemyDash.ToString();
        enemyRangeTxt.text = enemyRange.ToString();

        if (boss != null)
        {
            bossStatusContainer.anchoredPosition = Vector3.down * 30;
            bossHPbar.localScale = new Vector3((float)boss.currentHealth / (float)boss.maxHealth, 1, 1);
        }
        else
        {
            bossStatusContainer.anchoredPosition = Vector3.up * 300;
        }
    }
}
