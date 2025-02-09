using System.Collections;
using Unity.Multiplayer.Center.Common.Analytics;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class Player : MonoBehaviour
{
    //public으로 선언시 에디터 내부에서 값을 수정 가능
    public float speed;
    
    public GameObject[] weapons;
    public bool[] hasWeapons;
    public GameObject[] grenades;
    public int hasGrenade;

    public int ammo;
    public int coin;
    public int health;

    public int MaxAmmo;
    public int MaxCoin;
    public int MaxHealth;
    public int MaxHasGrenade;

    public float dodgeSpeed = 0f;

    public float DODGE_DECREASE_FORCE = 10f;
    public float JUMP_FORCE = 10;
    public float DODGE_FORCE = 20;
    private bool isDodgeAvailable = true; // 회피 가능 여부를 제어하는 변수
    private float dodgeCooldown = 0.9f; // 회피 쿨다운 시간 (초 단위)

    float hAxis;
    float vAxis;
    bool wDown;
    bool jDown;

    //e키가 눌렸을 때
    bool iDown;

    bool sDown1;
    bool sDown2;
    bool sDown3;

    bool isJump;
    bool isDodge;
    bool isSwap;

    Vector3 moveVec;

    //물리 효과를 위한 리지드 바디 선언
    Rigidbody rigid;

    //animator컨트롤러 정보를 가져오는 인스턴스 변수
    Animator anim;

    //주변 아이템을 저장하기 위한 변수
    GameObject nearObject;

    GameObject equipWeapon;

    int equipWeaponIndex = -1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        //MeshObject에 에니메이션 컨트롤러 정보가 있기에 InChildren붙이기
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Dodge();
        Swap();
        Interaction();
    }

    void GetInput()
    {
        //키보드 입력값을 0,1로 가져오기
        //메서드내부의 문자열은 유니티에서 설정되있는 input의 문자열
        if (isDodgeAvailable)
        {
            hAxis = Input.GetAxisRaw("Horizontal");
            vAxis = Input.GetAxisRaw("Vertical");
        }
        wDown = Input.GetButton("Walk");
        jDown = Input.GetButtonDown("Jump");
        iDown = Input.GetKeyDown(KeyCode.E);

        sDown1 = Input.GetKeyDown(KeyCode.Alpha1);
        sDown2 = Input.GetKeyDown(KeyCode.Alpha2);
        sDown3 = Input.GetKeyDown(KeyCode.Alpha3);
    }

    void Move()
    {
        //움직일 벡터 계산
        //.normalized추가로 대각선 이동시에도 똑같은 1 값으로보정
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        //transform == 객체 내부에 들어있는 이동담당
        //Time.deltatTime == 프레임이 일정치 않아도 속도가 똑같게 만드는 값

        if (isSwap)
        {
            moveVec = Vector3.zero;
        }

        if (wDown)
        { //걸을 때
            transform.position += moveVec * NowSpeed() * 0.3f * Time.deltaTime;
        }
        else
        {
            transform.position += moveVec * NowSpeed() * Time.deltaTime;
        }

        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", wDown);
    }

    void Turn()
    {
        //나아가야할 방향으로 바라보게 하는 함수
        transform.LookAt(transform.position + moveVec);
    }

    void Jump()
    {
        if (jDown && moveVec == Vector3.zero && !isJump && !isSwap)
        {
            Debug.Log("점프");
            //물리적 힘을 가하기
            //ForceMode.Impulse == 즉발적인 힘
            rigid.AddForce(Vector3.up * JUMP_FORCE, ForceMode.Impulse);

            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");

            isJump = true;
        }
    }

    void Dodge()
    {
        if (jDown && moveVec != Vector3.zero && !isDodge && isDodgeAvailable && !isSwap)
        {
            dodgeSpeed = DODGE_FORCE;
            Debug.Log("회피");
            anim.SetTrigger("doDodge");
            isDodge = true;
             isDodgeAvailable = false;

            StartCoroutine(DodgeOut());
            StartCoroutine(DodgeCooldown());
        }
    }

    IEnumerator DodgeOut()
    {
        while (NowSpeed() > 15f)
        {
            dodgeSpeed -= DODGE_DECREASE_FORCE;
            yield return new WaitForSeconds(0.2f);
        }

        Debug.Log("속도를 다 줄임");
        dodgeSpeed = 0;
        isDodge = false;
    }

    IEnumerator DodgeCooldown()
    {
        yield return new WaitForSeconds(dodgeCooldown); // 쿨다운 시간 동안 대기
        isDodgeAvailable = true; // 회피가 다시 가능해짐
        Debug.Log("회피가능");
    }

    void Swap()
    {
        if(sDown1 && (!hasWeapons[0] || equipWeaponIndex == 0))
            return;

        if(sDown2 && (!hasWeapons[1] || equipWeaponIndex == 1))
            return;

        if(sDown3 && (!hasWeapons[2] || equipWeaponIndex == 2))
            return;

        int weaponIndex = -1;
        if(sDown1) weaponIndex = 0;
        if(sDown2) weaponIndex = 1;
        if(sDown3) weaponIndex = 2;

        if((sDown1 || sDown2 || sDown3) && !isJump && !isDodge)
        {
            if(equipWeapon !=null) 
            {
                equipWeapon.SetActive(false);
            }
            
            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex];
            weapons[weaponIndex].SetActive(true);

            anim.SetTrigger("doSwap");

            isSwap = true;

            Invoke("SwapOut", 0.4f);
        }
    }

    void SwapOut()
    {
        isSwap = false;
    }

    void Interaction()
    {
        if (iDown && nearObject != null && !isJump)
        {
            if (nearObject.tag == "Weapon")
            {
                Item item = nearObject.GetComponent<Item>();
                //무기 구분
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;

                //입수한 무기 삭제
                Destroy(nearObject);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        //현재 "Floor"태그에 붙어있는지 체크
        if (collision.gameObject.tag == "Floor")
        {
            anim.SetBool("isJump", false);

            isJump = false;
        }
    }

    float NowSpeed()
    {
        return this.speed + this.dodgeSpeed;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Item"){
            Item item = other.GetComponent<Item>();
            switch(item.type){
                case Item.Type.Ammo:
                    ammo += item.value;
                    if(ammo > MaxAmmo){
                        ammo = MaxAmmo;
                    }
                    break;
                case Item.Type.Coin:
                    coin += item.value;
                    if(coin > MaxCoin){
                        coin = MaxCoin;
                    }
                    break;
                case Item.Type.Heart:
                    health += item.value;
                    if(health > MaxHealth){
                        health = MaxHealth;
                    }
                    break;
                case Item.Type.Grenade:
                    grenades[hasGrenade].SetActive(true);
                    hasGrenade += item.value;
                    if(hasGrenade > MaxHasGrenade){
                        hasGrenade = MaxHasGrenade;
                    }
                    break;    
            }
            Destroy(other.gameObject);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon")
        {
            nearObject = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
        {
            nearObject = null;
        }
    }
}
