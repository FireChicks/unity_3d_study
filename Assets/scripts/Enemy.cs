using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
   public int maxHealth;
   public int curHealth;

   Rigidbody rigid;
   BoxCollider boxCollider;
   Material mat;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        //메테리얼은 바로 못 가져옴
        mat = GetComponent<MeshRenderer>().material;
        mat.color = Color.white;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Melee"){
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage;
            Vector3 reactVec = transform.position - other.transform.position;
            
            StartCoroutine(OnDamage(reactVec));
        } 
        else if (other.tag == "Bullet"){
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage;
            Vector3 reactVec = transform.position - other.transform.position;
            Destroy(other.gameObject);

            StartCoroutine(OnDamage(reactVec));
        }
    }

    IEnumerator OnDamage(Vector3 reactVec)
    {
        mat.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        if(curHealth > 0) {
            mat.color = Color.white;
        } else {
            mat.color = Color.gray;
            //12번 레이어라 12번 지정(Enemy Dead)
            gameObject.layer = 12;

            reactVec = reactVec.normalized;
            reactVec += Vector3.up;
            rigid.AddForce(reactVec * 5, ForceMode.Impulse);

            //4초뒤 사라지기
            Destroy(gameObject, 4);
        }
    }
}
