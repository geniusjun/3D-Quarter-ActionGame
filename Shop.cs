using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Net.NetworkInformation;

public class Shop : MonoBehaviour
{
    public RectTransform uiGroup;
    public Animator anim;

    public GameObject[] itemObj;
    public int[] itemPrice;
    public Transform[] itemPos; 
    public TextMeshProUGUI talkText;
    public string[] talkData;
    Player enterPlayer;
    public void Enter(Player player)
    {
        enterPlayer = player;
        uiGroup.anchoredPosition = Vector3.zero; //이렇게 하면 uiGroup이 Canvas의 정중앙에 위치하게 되어, 게임 화면에서 정확히 가운데에 배치됌
    }

    public void Exit()
    {
        anim.SetTrigger("doHello");
        uiGroup.anchoredPosition = Vector3.down * 1000;
    }

    public void Buy(int index)
    {
        int price = itemPrice[index];
        if(price > enterPlayer.coin)
        {
            StopCoroutine(Talk());
            StartCoroutine(Talk());
            return;
        }
        
        enterPlayer.coin -= price;
        Vector3 ranVec = Vector3.right * Random.Range(-3,3) + Vector3.forward * Random.Range(-3,3);
        Instantiate(itemObj[index], itemPos[index].position + ranVec, itemPos[index].rotation);   
    }

    IEnumerator Talk()
    {
        talkText.text = talkData[1];
        yield return new WaitForSeconds(2f);
        talkText.text = talkData[0];
    }
}
