using UnityEngine;

public class Orbit : MonoBehaviour
{

    public Transform target;
    public float orbitSpeed;
    Vector3 offSet;

    void Start()
    {
        //타겟과의 거리 차이를 계속 집어넣기
        offSet = transform.position - target.position;
    }

    void Update()
    {
        //움직임의 오차 수정
        transform.position = target.position + offSet;

        //RotateAround(타겟 주위로 회전하는 함수)기준점, 기준축, 속도
        transform.RotateAround(target.position, 
                                Vector3.up,
                                orbitSpeed * Time.deltaTime);
        
        offSet = transform.position - target.position;
    }
}
