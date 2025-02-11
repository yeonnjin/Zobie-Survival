using UnityEngine;

// 플레이어 캐릭터를 사용자 입력에 따라 움직이는 스크립트
public class PlayerMovement : MonoBehaviour {
    public float moveSpeed = 5f;            // 앞뒤 움직임의 속도
    public float rotateSpeed = 180f;        // 좌우 회전 속도

    private PlayerInput playerInput;        // 플레이어 입력을 알려주는 컴포넌트
    private Rigidbody playerRigidbody;      // 플레이어 캐릭터의 리지드바디
    private Animator playerAnimator;        // 플레이어 캐릭터의 애니메이터

    // 사용할 컴포넌트들의 참조를 가져오기
    private void Start() {       
        
        playerInput = GetComponent<PlayerInput>();
        playerRigidbody = GetComponent<Rigidbody>();    
        playerAnimator = GetComponent<Animator>();
    }

    // FixedUpdate는 물리 갱신 주기에 맞춰 실행됨
    // 물리 갱신 주기마다 움직임, 회전, 애니메이션 처리 실행
    // FixedUpdate() 내부에서 Time.deltaTime의 값에 접근할 경우 자동으로 Time.fixedDeltaTime의 값을 출력
    private void FixedUpdate() {
        
        Rotate();
        Move();

        playerAnimator.SetFloat("Move", playerInput.move);
    }

    // 입력값에 따라 캐릭터를 앞뒤로 움직임
    private void Move() {
        
        Vector3 moveDistance = playerInput.move * transform.forward * moveSpeed * Time.deltaTime;

        playerRigidbody.MovePosition(playerRigidbody.position + moveDistance);

        // transform.position으로 위치를 바로 수정할 경우 물리 처리를 무시할 수 있음
        // 물리적 상호작용과 부드러운 이동을 원한다면 MovePosition()
        // 특정 상황에서 물리 엔진의 영향을 받지 않고 위치를 즉시 변경해야 할 필요가 있을 때는 playerRigidbody.position을 직접 수정
    }

    // 입력값에 따라 캐릭터를 좌우로 회전
    private void Rotate() {

        float turn = playerInput.rotate * rotateSpeed * Time.deltaTime;

        // playerRigidbody.MoveRotation(playerRigidbody.rotation * Quaternion.Euler(0f, turn, 0f));
        playerRigidbody.rotation *= Quaternion.Euler(0f, turn, 0f); 
    }
}