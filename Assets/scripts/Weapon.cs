using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type {Melee, Range};
    public Type type;
    public int damage;
    public float rate;
    public int MaxAmmo;
    public int curAmmo;
    public BoxCollider meleeArea;
    public TrailRenderer trailEffect;

    public Transform bulletPos;
    public GameObject bullet;
    public Transform bulletCasePos;
    public GameObject bulletCase;


    public void Use()
    {
        if(type == Type.Melee){
            //코루틴 멈추기(호출 중에라도)
            StopCoroutine("Swing");
            //코루틴 호출
            StartCoroutine("Swing");
        }
        else if(type == Type.Range && curAmmo > 0) {
            curAmmo--;
            StartCoroutine("Shot");
        }
    }

    //코루틴 -> 서브루틴과 같은 일반적인 함수호출과는 틀리게 함수가 호출되도 메인 함수와 동시에 실행됨
    IEnumerator Swing()
    {
        //1
        yield return new WaitForSeconds(0.1f); //0.1초 대기

        meleeArea.enabled = true;
        trailEffect.enabled = true;
        yield return new WaitForSeconds(0.3f); //0.3초 대기

        meleeArea.enabled = false;
        yield return new WaitForSeconds(0.3f); //0.3초 대기
        
        trailEffect.enabled = false;
        //코루틴 탈출
        yield break;
    }

    IEnumerator Shot()
    {
        //#1. 총알 발사
        GameObject instantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();
        bulletRigid.linearVelocity = bulletPos.forward * 50;


        yield return null;
        //#2. 탄피 배출
        GameObject instantCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody caseRigid = instantCase.GetComponent<Rigidbody>();
        //z축의 뒤로 가게 하기 위해 -3~-2로 설정
        Vector3 caseVec = bulletCasePos.forward * Random.Range(-3, -2) + Vector3.up * Random.Range(2, 3);
        caseRigid.AddForce(caseVec, ForceMode.Impulse);
        //탄피 회전
        caseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse);                      

    }
}
