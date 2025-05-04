using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class Enemy : MonoBehaviour
{
    public enum Type
    {
        A,
        B,
        C,
        D,
    };

    public Type enemeyType;
    public int maxHealth;
    public int curHealth;
    public Transform target;
    public BoxCollider meleeArea;
    public GameObject bullet;
    public bool isChase;
    public bool isAttack;
    public bool isDead;

    protected Rigidbody rigid;
    protected BoxCollider boxCollider;
    protected MeshRenderer[] meshes;
    protected NavMeshAgent nav;
    protected Animator anim;

    //자식에선 실행 안됨
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        //메테리얼은 바로 못 가져옴
        meshes = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        if (enemeyType != Type.D)
        {
            Invoke("ChaseStart", 2);
        }
    }

    void ChaseStart()
    {
        isChase = true;
        anim.SetBool("isWalk", true);
    }

    void Update()
    {
        if (nav.enabled && enemeyType != Type.D)
        {
            nav.SetDestination(target.position);
            nav.isStopped = !isChase;
        }
    }

    void FreezeVelocity()
    {
        //추적시에만 충돌해도 물리 발생X
        if (isChase)
        {
            rigid.linearVelocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }

    void Targeting()
    {
        if (!isDead && enemeyType != Type.D)
        {
            float targetRadius = 0;
            float targetRange = 0;

            switch (enemeyType)
            {
                case Type.A:
                    targetRadius = 1.5f;
                    targetRange = 3f;
                    break;
                case Type.B:
                    targetRadius = 1f;
                    targetRange = 12f;
                    break;
                case Type.C:
                    targetRadius = 0.5f;
                    targetRange = 25f;
                    break;
            }

            RaycastHit[] rayHits = Physics.SphereCastAll(
                transform.position,
                targetRadius,
                transform.forward,
                targetRange,
                LayerMask.GetMask("Player")
            );

            if (rayHits.Length > 0 && !isAttack)
            {
                StartCoroutine(Attack());
            }
        }
    }

    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        anim.SetBool("isAttack", isAttack);

        switch (enemeyType)
        {
            case Type.A:
                //공격모션 대기 시간
                yield return new WaitForSeconds(0.2f);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(1f);
                meleeArea.enabled = false;

                yield return new WaitForSeconds(1f);
                break;
            case Type.B:
                //돌격
                yield return new WaitForSeconds(0.2f);
                rigid.AddForce(transform.forward * 20, ForceMode.Impulse);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(0.5f);
                rigid.linearVelocity = Vector3.zero;
                meleeArea.enabled = false;

                yield return new WaitForSeconds(2f);
                break;
            case Type.C:
                yield return new WaitForSeconds(0.5f);
                GameObject instantBullet = Instantiate(
                    bullet,
                    transform.position,
                    transform.rotation
                );
                Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
                rigidBullet.linearVelocity = transform.forward * 20;

                yield return new WaitForSeconds(2f);
                break;
        }

        isChase = true;
        isAttack = false;
        anim.SetBool("isAttack", isAttack);
    }

    void FixedUpdate()
    {
        Targeting();
        FreezeVelocity();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage;
            Vector3 reactVec = transform.position - other.transform.position;

            StartCoroutine(OnDamage(reactVec, false));
        }
        else if (other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage;

            Vector3 reactVec = transform.position - other.transform.position;
            Destroy(other.gameObject);

            StartCoroutine(OnDamage(reactVec, false));
        }
    }

    public void HitByGrenade(Vector3 explosionPos)
    {
        curHealth -= 100;

        Vector3 reactVec = transform.position - explosionPos;
        StartCoroutine(OnDamage(reactVec, true));
    }

    IEnumerator OnDamage(Vector3 reactVec, bool isGrenade)
    {
        if (!isDead)
        {
            foreach (MeshRenderer mesh in meshes)
            {
                mesh.material.color = Color.red;
            }
            yield return new WaitForSeconds(0.1f);

            if (curHealth > 0)
            {
                foreach (MeshRenderer mesh in meshes)
                {
                    mesh.material.color = Color.white;
                }
            }
            else
            {
                foreach (MeshRenderer mesh in meshes)
                {
                    mesh.material.color = Color.gray;
                }
                //12번 레이어라 12번 지정(Enemy Dead)
                gameObject.layer = 12;

                //추적 종료
                isDead = true;
                isChase = false;
                isAttack = true;
                nav.enabled = false;

                anim.SetTrigger("doDie");

                if (isGrenade)
                {
                    reactVec = reactVec.normalized;
                    reactVec += Vector3.up * 3;

                    //설정되어있는 프리즈 로테이션 해제
                    rigid.freezeRotation = false;
                    rigid.AddForce(reactVec * 5, ForceMode.Impulse);
                    rigid.AddTorque(reactVec * 15, ForceMode.Impulse);
                }
                else
                {
                    reactVec = reactVec.normalized;
                    reactVec += Vector3.up;
                    rigid.AddForce(reactVec * 5, ForceMode.Impulse);
                }

                //4초뒤 사라지기
                if (enemeyType != Type.D)
                {
                    Destroy(gameObject, 4);
                }
            }
        }
    }
}
