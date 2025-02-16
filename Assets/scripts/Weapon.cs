using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type {Melee, Range};
    public Type type;
    public int damage;
    public float rate;
    public BoxCollider meleeArea;
    public TrailRenderer trailEffect;


    public void Use()
    {
        if(type == Type.Melee){
            //코루틴 멈추기(호출 중에라도)
            StopCoroutine("Swing");
            //코루틴 호출
            StartCoroutine("Swing");
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

}
