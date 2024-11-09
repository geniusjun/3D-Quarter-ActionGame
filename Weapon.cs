using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Range};
    public Type type;
    public int damage;
    public float rate;
    public int maxAmmo;
    public int curAmmo;

    public BoxCollider meleeArea;
    public TrailRenderer trailEffect;
    public Transform bulletPos;
    public GameObject bullet;
    public Transform bulletCasePos;
    public GameObject bulletCase;

    public void Use()
    {
        if(type == Type.Melee)
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }
        else if (type == Type.Range && curAmmo > 0)
        {
            curAmmo--;
            StartCoroutine("Shot");
        }  
    }

    IEnumerator Swing()
    {
        yield return new WaitForSeconds(0.1f); // 0.1초 쉬고 코루틴 쓰면 Invoke()를 쓸필요가 없네!
        meleeArea.enabled = true;
        trailEffect.enabled = true; 
        
        yield return new WaitForSeconds(0.5f);
        meleeArea.enabled = false;
        
        yield return new WaitForSeconds(0.3f); // 0.3초 기다린 후 실행. 
        trailEffect.enabled = false;
    }

    //Use() 메인루틴 -> Swing() 서브루틴 -> Use() : 메인루틴
    //Use() 메인루틴 + Swing() : 코루틴 

    IEnumerator Shot()
    {
        // #1. 총알발사
        GameObject istantBullet = Instantiate(bullet,bulletPos.position, bulletPos.rotation);
        Rigidbody bulletRigid = istantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletPos.forward * 50;

        yield return null;
        // #2. 탄피배출
        GameObject istantCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody caseRigid = istantCase.GetComponent<Rigidbody>();
        Vector3 caseVec = bulletCasePos.forward * Random.Range(-3, -2) + Vector3.up * Random.Range(2, 3);
        caseRigid.AddForce(caseVec, ForceMode.Impulse);
        caseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse);
    }
}
