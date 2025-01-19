using System.Collections;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class Player : MonoBehaviour
{
    //public으로 선언시 에디터 내부에서 값을 수정 가능
    public float speed;

    public float dodgeSpeed = 0f;
    public const float JUMP_FORCE = 10;
    public const float DODGE_FORCE = 15;
    public const float DODGE_DECREASE_FORCE = 3f;

    private bool isDodgeAvailable = true; // 회피 가능 여부를 제어하는 변수
    private float dodgeCooldown = 1f; // 회피 쿨다운 시간 (초 단위)

    float hAxis;
    float vAxis;
    bool wDown;
    bool jDown;

    bool isJump;
    bool isDodge;

    Vector3 moveVec;

    //물리 효과를 위한 리지드 바디 선언
    Rigidbody rigid;

    //animator컨트롤러 정보를 가져오는 인스턴스 변수
    Animator anim;

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
    }

    void GetInput()
    {
        //키보드 입력값을 0,1로 가져오기
        //메서드내부의 문자열은 유니티에서 설정되있는 input의 문자열
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");
        jDown = Input.GetButtonDown("Jump");
    }

    void Move()
    {
        //움직일 벡터 계산
        //.normalized추가로 대각선 이동시에도 똑같은 1 값으로보정
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        //transform == 객체 내부에 들어있는 이동담당
        //Time.deltatTime == 프레임이 일정치 않아도 속도가 똑같게 만드는 값
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
        if (jDown && moveVec == Vector3.zero && !isJump)
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
        if (jDown && moveVec != Vector3.zero && !isDodge && isDodgeAvailable)
        {
            dodgeSpeed = DODGE_FORCE;
            Debug.Log("회피");
            anim.SetTrigger("doDodge");
            isDodge = true;

            StartCoroutine(DodgeOut());
            StartCoroutine(DodgeCooldown());
        }
    }

    IEnumerator DodgeOut()
    {
        while (NowSpeed() > 15f)
        {
            dodgeSpeed -= DODGE_DECREASE_FORCE;
            Debug.Log("속도를 줄이는중  speed : " + NowSpeed());
            isDodge = false;

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
}
