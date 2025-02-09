# Unity 3D 쿼터뷰 액션 게임 개발

이 프로젝트는 유니티 3D로 쿼터뷰 액션 게임을 개발하는 예제입니다. 본 게임은 [유니티 기초 강좌 예제 5번째](https://www.youtube.com/playlist?list=PLO-mt5Iu5TeYkrBzWKuTCl6IUm_bA6BKy) 강좌를 참고하여 개발 중입니다.

## 개발 환경
- Unity 3D 버전: Unity 6
- 사용된 에셋: [에셋 패키지 다운로드 링크](http://u3d.as/2mLK)

![image](https://github.com/user-attachments/assets/30b3b03b-2a12-4fdf-af69-f28592def1a4)

---

## 개발일지

### 2025년 1월 13일

#### 진행 내용:
- **플레이어 기본 이동 구현([3D 쿼터뷰 액션게임 - 플레이어 이동](https://www.youtube.com/watch?v=WkMM7Uu2AoA&list=PLO-mt5Iu5TeYkrBzWKuTCl6IUm_bA6BKy))**  
  - 플레이어 캐릭터가 WASD 키로 상하좌우로 이동할 수 있도록 기본 이동 시스템을 구현했습니다.
  - 이동 시 카메라가 플레이어 위에서 따라갈 수 있도록 구현을 완료했습니다.
 
  ![image](https://github.com/user-attachments/assets/c21e8547-31a8-45b4-bd12-bd6d606a5b6e)


### 2025년 1월 19일
- **점프와 회피 구현([3D 쿼터뷰 액션게임 - 플레이어 점프와 회피](https://www.youtube.com/watch?v=eZ8Dm809j4c&list=PLO-mt5Iu5TeYkrBzWKuTCl6IUm_bA6BKy&index=2))**  
  - 점프, 회피 애니메이션 설정시 any state 사용
  - 1회성 애니메이션은 컨디션에 trigger 사용
  - 지형의 충돌 문제 해결(비벼지는 문제)
    1. 지형의 속성 static으로 변경 ➞ collision detection-> continuous 를 잘 검출하기 위해
    2. 지형에 Rigidbody 추가 및, Gravity 제거, is Kinematic 설정 온 ➞「is Kinematic」직접 명령어로 이동시키지 않는 이상 이동하지 않는 설정
    3. 지형에 Physics Materials 추가
    4. (original)구르기 시 한번에 줄어드는 속도를 강의와 틀리게 일정한 감소값을 통해 점차 느려지게 코루틴을 사용하여 구현
   
  ![image](https://github.com/user-attachments/assets/c67339a3-3f75-47d6-b505-311c6d415482)

### 2025년 1월 26일
- **아이템 생성([3D 쿼터뷰 액션게임 - 아이템 만들기](https://www.youtube.com/watch?v=eZ8Dm809j4c&list=PLO-mt5Iu5TeYkrBzWKuTCl6IUm_bA6BKy&index=3))**  
  - item들이 공통으로 사용하는 item 스크립트 생성
  - 사용될 9가지 아이템들의 Prefabs 생성
  - Light, Particle 생성
 
  ![아이템](https://github.com/user-attachments/assets/5081362c-8bc5-47f9-9641-d35c80592dd3)

### 2025년 2월 02일
- **아이템 습득과 아이템 스왑([3D 쿼터뷰 액션게임 - 드랍 무기 입수와 교체](https://www.youtube.com/watch?v=eZ8Dm809j4c&list=PLO-mt5Iu5TeYkrBzWKuTCl6IUm_bA6BKy&index=4))**  
  - 각 아이템들이 배열을 사용해서 습득 가능하게 변경
  - 1, 2, 3 숫자키를 이용해서 각 무기 변경 구현
  - 구르기(회피) 시전시 방향전환 불가능하게 변경
  ![image](https://github.com/user-attachments/assets/0912385a-7c33-4153-9574-007d1d15e082)

### 2025년 2월 09일
- **아이템 먹기, 수류탄 플레이어 주위 공전 구현([3D 쿼터뷰 액션게임 - 아이템 먹기 & 공전물체 만들기](https://www.youtube.com/watch?v=eZ8Dm809j4c&list=PLO-mt5Iu5TeYkrBzWKuTCl6IUm_bA6BKy&index=5))**  
  - 각 아이템들에 설정된 VALUE만큼 습득시 값이 증가하게 구현
  - 플레이어 주위를 공전하는 GrenadeGroup생성 및 공전하게 구현
  - 현재 수류탄 수에 맞게 공전하는 수류탄 개수가 늘도록 구현

  ![image](https://github.com/user-attachments/assets/49fbd64e-d0e8-4138-ae19-7398babe1311)

