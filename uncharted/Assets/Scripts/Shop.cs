using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public RectTransform uiContainer;
    public GameObject[] itemObj;
    public Transform[] itemPos;
    public Animator anim;
    public Button[] itemButtons;
    public Text talkText;
    public string[] talkData;
    public string shopType;
    public int[] itemPrice;

    Player inPlayer;
    public void InPlayer(Player player)
    {
        inPlayer = player;
        uiContainer.anchoredPosition = Vector3.zero;
    }
    public void ExitPlayer()
    {
        anim.SetTrigger("isHello");
        uiContainer.anchoredPosition = Vector3.down * 1000;
    }
    public void BuyItem(int index)
    {
        int price = itemPrice[index];
        if (price > inPlayer.coin)
        {
            StopCoroutine("TxtCorotine");
            StartCoroutine("TxtCorotine");
            return;
        }
        inPlayer.coin -= price;
        Vector3 buyItemSpawnPos = Vector3.right * Random.Range(-3, 3)
                        + Vector3.forward * Random.Range(-25, -20)
                        + Vector3.up * 2;
        Instantiate(itemObj[index], itemPos[index].position + buyItemSpawnPos, itemPos[index].rotation);


        if (shopType == "Weapon")
        {
            itemButtons[index].interactable = false;
        }
    }
    IEnumerator TxtCorotine()
    {
        talkText.text = talkData[1];

        yield return new WaitForSeconds(2f);
        talkText.text = talkData[0];
    }
}
