using UnityEngine;

// 필수 컴포넌트를 선언하는 부분
[RequireComponent(typeof(Rigidbody), typeof(Animator), typeof(AudioSource))]
public class PenguinController : MonoBehaviour
{
    public float moveSpeed = 5f;        // 이동 속도
    public float maxJumpForce = 10f;    // 최대 점프 힘
    public float minJumpForce = 2f;     // 최소 점프 힘
    public AudioClip jumpSound;         // 점프할 때 재생할 소리

    private Rigidbody rb;               // 리지드바디
    private Animator animator;          // 애니메이터
    private AudioSource audioSource;    // 오디오 소스

    // 주석 처리된 변수 (사용하지 않음)
    // private bool isGrounded = false;    // 땅에 닿았는지 여부
    // private bool jumpRequested = false; // 점프 요청 여부
    // private bool isJumping = false;     // 점프 중 여부

    // 주석 처리된 변수 (사용하지 않음)
    // private float jumpHoldTime = 0f;    // 점프 버튼 유지 시간
    // private float jumpRequestTime = 0f; // 점프 요청 시점

    // public CameraFollow cameraFollow;   // 카메라 움직임 처리 (아직 자세한 구현은 없음)

    // Start 함수: 게임 시작 시 호출되는 함수
    void Start()
    {
        // ====== 컴포넌트 초기화 ======
        rb = GetComponent<Rigidbody>();        // 리지드바디 컴포넌트를 가져와서 rb에 저장
        animator = GetComponent<Animator>();   // 애니메이터 컴포넌트를 가져와서 animator에 저장
        audioSource = GetComponent<AudioSource>(); // 오디오 소스를 가져와서 audioSource에 저장

        // ====== 리지드바디 설정 ======
        rb.isKinematic = false;   // 리지드바디가 물리적인 영향을 받도록 설정
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ; // X, Z축 회전 잠금

        // ====== 충돌 감지 모드 설정 ======
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous; // 빠른 물체가 충돌할 때도 감지를 놓치지 않도록 연속 충돌 감지 모드로 설정
    }

    // FixedUpdate: 물리 연산을 처리하는 업데이트 함수 (나중에 추가할게)
    void FixedUpdate()
    {
        // HandleMovement();  // 이동 처리 (나중에 설명할게)
        // HandleJump();      // 점프 처리 (나중에 설명할게)

        // rb.angularVelocity = Vector3.zero; // 불필요한 회전 제거 (나중에 추가)
    }

    // 캐릭터 이동 처리
    private void HandleMovement()
    {
        // 이동 입력 받기 (나중에 설명할게)
        // float moveX = Input.GetAxisRaw("Horizontal");
        // float moveZ = Input.GetAxisRaw("Vertical");

        // 이동 방향 계산 (나중에 추가할게)
        // Vector3 moveDirection = new Vector3(moveX, 0, moveZ);

        // 이동 방향에 따른 회전 (나중에 설명할 부분)
        // if (moveDirection.magnitude > 0.01f)
        // {
        //     Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
        //     transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        // }

        // 리지드바디를 이용한 이동 처리 (추후 설명)
        // rb.velocity = new Vector3(moveDirection.x * moveSpeed, rb.velocity.y, moveDirection.z * moveSpeed);

        // 애니메이션 업데이트 (추후 설명)
        // animator.SetFloat("moveSpeed", moveDirection.magnitude);
    }

    // 점프 처리 (추후 설명)
    private void HandleJump()
    {
        // 점프 입력 받기
        // if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        // {
        //     jumpRequested = true;
        //     jumpRequestTime = Time.time;
        //     cameraFollow.SetJumping(true);
        // }

        // 점프 시간 계산 (추후 설명)
        // if (jumpRequested && Input.GetKey(KeyCode.Space))
        // {
        //     jumpHoldTime = Time.time - jumpRequestTime;
        // }
        // else if (jumpRequested && !Input.GetKey(KeyCode.Space))
        // {
        //     float jumpForce = Mathf.Clamp(minJumpForce + (jumpHoldTime * maxJumpForce), minJumpForce, maxJumpForce);
        //     rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        //     jumpRequested = false;
        //     isGrounded = false;
        //     jumpHoldTime = 0f;
        //     isJumping = true;
        //     animator.SetBool("isJumping", true);
        //     audioSource.PlayOneShot(jumpSound);
        //     cameraFollow.SetJumping(false);
        // }
    }

    // 충돌 처리: 점프 상태 리셋 (추후 설명)
    // void OnCollisionEnter(Collision collision)
    // {
    //     if (collision.gameObject.CompareTag("Ground"))
    //     {
    //         if (isJumping)
    //         {
    //             isJumping = false;
    //             animator.SetBool("isJumping", false);
    //         }
    //         isGrounded = true;
    //     }
    // }
}