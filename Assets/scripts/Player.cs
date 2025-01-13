using UnityEngine;

public class Player : MonoBehaviour
{
    //public으로 선언시 에디터 내부에서 값을 수정 가능
    public float speed;

    float hAxis;
    float vAxis;
    bool wDown;

    Vector3 moveVec;

    //animator컨트롤러 정보를 가져오는 인스턴스 변수
    Animator anim;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //MeshObject에 에니메이션 컨트롤러 정보가 있기에 InChildren붙이기
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //키보드 입력값을 0,1로 가져오기
        //메서드내부의 문자열은 유니티에서 설정되있는 input의 문자열
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");

        //움직일 벡터 계산
        //.normalized추가로 대각선 이동시에도 똑같은 1 값으로보정
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        //transform == 객체 내부에 들어있는 이동담당
        //Time.deltatTime == 프레임이 일정치 않아도 속도가 똑같게 만드는 값
        if(wDown){  //걸을 때 
            transform.position += moveVec * speed * 0.3f * Time.deltaTime;
        }else{
            transform.position += moveVec * speed * Time.deltaTime;
        }        

        //나아가야할 방향으로 바라보게 하는 함수
        transform.LookAt(transform.position + moveVec);
        

        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", wDown);
    }
}
