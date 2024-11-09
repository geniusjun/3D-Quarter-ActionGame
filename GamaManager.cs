using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GamaManager : MonoBehaviour
{
    public GameObject menuCam;
    public GameObject gameCam;
    public Player player;
    public Boss boss;
    public GameObject itemShop;
    public GameObject weaponShop;
    public GameObject startZone;
    public int stage;
    public float playTime;
    public bool isBattle;
    public int enemyCntA;
    public int enemyCntB;
    public int enemyCntC;
    public int enemyCntD;

    public Transform[] enemyZones;
    public GameObject[] enemies;
    public List<int> enemyList;

    public GameObject menuPanel;
    public GameObject gamePanel;
    public GameObject overPanel;
    public TextMeshProUGUI maxScoreTxt;
    public TextMeshProUGUI scoreTxt;
    public TextMeshProUGUI stageTxt;
    public TextMeshProUGUI playTimeTxt;
    public TextMeshProUGUI playerHealthTxt;
    public TextMeshProUGUI playerAmmoTxt;
    public TextMeshProUGUI playerCoinTxt;
    public Image weapon1Img;
    public Image weapon2Img;
    public Image weapon3Img;
    public Image weaponRImg;
    public TextMeshProUGUI enemyATxt;
    public TextMeshProUGUI enemyBTxt;
    public TextMeshProUGUI enemyCTxt;
    public RectTransform bossHealthGroup;
    public RectTransform bossHealthBar;
    public TextMeshProUGUI curScoreText;
    public TextMeshProUGUI bestText;

    public AudioSource GameOverSound;
    public AudioSource StageEndSound;




    void Awake()
    {
        enemyList = new List<int>();    
        maxScoreTxt.text = string.Format("{0:n0}", PlayerPrefs.GetInt("MaxScore"));

        if (!PlayerPrefs.HasKey("MaxScore")){
            PlayerPrefs.SetInt("MaxScore", 0);
        }
    }

    public void GameStart()
    {
        menuCam.SetActive(false);
        gameCam.SetActive(true);

        menuPanel.SetActive(false);
        gamePanel.SetActive(true);

        player.gameObject.SetActive(true);
    }
    
    public void GameOver()
    {
        overPanel.SetActive(true);
        gamePanel.SetActive(false);
        curScoreText.text = scoreTxt.text;


        int maxScore = PlayerPrefs.GetInt("MaxScore");
        if(player.score > maxScore)
        {
            bestText.gameObject.SetActive(true);
            PlayerPrefs.SetInt("MaxScore", player.score);
        } 
        GameOverSound.Play();
    }

    public void ReStart()
    {
        SceneManager.LoadScene(0);
    }

    public void StageStart()
    {
        itemShop.SetActive(false);
        weaponShop.SetActive(false);
        startZone.SetActive(false);

        foreach (Transform zone in enemyZones)
        {
            zone.gameObject.SetActive(true);
        }

        isBattle = true;
        StartCoroutine(InBattle());
    }
    public void StageEnd()
    {
        // 미사일 제거
        GameObject[] missiles = GameObject.FindGameObjectsWithTag("EnemyBullet");
        foreach (GameObject missile in missiles)
        {
            Destroy(missile);
        }

        // 보스 시체 제거
        GameObject[] bossCorpses = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject bossCorpse in bossCorpses)
        {
            Destroy(bossCorpse);
        }

        player.transform.position = Vector3.up * 0.8f; // 기본 위치 (0,0.8,0) 으로 설정해 놓음

        itemShop.SetActive(true);
        weaponShop.SetActive(true);
        startZone.SetActive(true);

        foreach (Transform zone in enemyZones)
        {
            zone.gameObject.SetActive(false);
        }
        StageEndSound.Play();
        isBattle =false;
        stage++;
    }

    IEnumerator InBattle()
    {
        if (stage % 5 == 0)
        {
            enemyCntD++;
            GameObject instantEnemy = Instantiate(enemies[3], enemyZones[0].position, enemyZones[0].rotation);
            Enemy enemy = instantEnemy.GetComponent<Enemy>();
            enemy.target = player.transform;
            enemy.manager = this;
            boss = instantEnemy.GetComponent<Boss>();

        }
        else
        {
            for (int index = 0; index < stage; index++)
            {
                int ran = Random.Range(0, 3);
                enemyList.Add(ran);

                switch (ran)
                {
                    case 0:
                        enemyCntA++;
                        break;
                    case 1:
                        enemyCntB++;
                        break;
                    case 2:
                        enemyCntC++;
                        break;
                }
            }

            while (enemyList.Count > 0)
            {
                int ranZone = Random.Range(0, 4);
                GameObject instantEnemy = Instantiate(enemies[enemyList[0]], enemyZones[ranZone].position, enemyZones[ranZone].rotation);
                Enemy enemy = instantEnemy.GetComponent<Enemy>();
                enemy.target = player.transform;
                enemy.manager = this;
                enemyList.RemoveAt(0);
                yield return new WaitForSeconds(4f);
            }
        }

        while (enemyCntA + enemyCntB + enemyCntC + enemyCntD > 0)
        {
            yield return null; // while문같은 반복문에선 이게 Update문 느낌임. 하나의 프레임.
        }

        yield return new WaitForSeconds(4f);
        boss = null;
        StageEnd();
    }

    void Update()
    {
        if(isBattle)
        {
            playTime += Time.deltaTime;
        }    
    }

    void LateUpdate()
    {
        // 상단 UI
        scoreTxt.text = string.Format("{0:n0}", player.score);
        stageTxt.text = "STAGE " + stage;

        int hour = (int)(playTime / 3600);
        int min = (int)((playTime - hour * 3600) / 60);
        int second = (int) (playTime % 60);
        playTimeTxt.text = string.Format("{0:00}", hour) + ":" + string.Format("{0:00}", min) + ":" + string.Format("{0:00}", second); 

        // 플레이어 UI
        playerHealthTxt.text = player.health + " / " + player.maxHealth;
        playerCoinTxt.text = string.Format("{0:n0}", player.coin);
        if(player.equipWeapon == null)
        {
            playerAmmoTxt.text = "- / " + player.ammo;
        }
        else if (player.equipWeapon.type == Weapon.Type.Melee)
        {
            playerAmmoTxt.text = "- / " + player.ammo;
        }
        else
        {
            playerAmmoTxt.text = player.equipWeapon.curAmmo + " / " + player.ammo;
        }

        // 무기 UI
        weapon1Img.color = new Color(1, 1, 1, player.hasWeapons[0] ? 1 : 0);
        weapon2Img.color = new Color(1, 1, 1, player.hasWeapons[1] ? 1 : 0);
        weapon3Img.color = new Color(1, 1, 1, player.hasWeapons[2] ? 1 : 0);
        weaponRImg.color = new Color(1, 1, 1, player.hasGrenades>0 ? 1 : 0);

        // 몬스터 숫자 UI
        enemyATxt.text = enemyCntA.ToString();
        enemyBTxt.text = enemyCntB.ToString();
        enemyCTxt.text = enemyCntC.ToString();

        // 보스 몬스터 체력 UI
        if(boss != null)
        {
            bossHealthGroup.anchoredPosition = Vector3.down * 30;
            bossHealthBar.localScale = new Vector3((float)boss.curHealth / boss.maxHealth, 1, 1);
        }
        else
        {
            bossHealthGroup.anchoredPosition = Vector3.up * 200;
        }

           
    }
}
 