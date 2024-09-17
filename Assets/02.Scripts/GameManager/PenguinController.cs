using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Animator), typeof(AudioSource))]
public class PenguinController : MonoBehaviour
{
    public float moveSpeed = 5f;        // 이동 속도
    public float maxJumpForce = 10f;    // 최대 점프 힘
    public float minJumpForce = 2f;     // 최소 점프 힘
    public AudioClip jumpSound;         // 점프할 때 재생할 소리

    private Rigidbody rb;
    private Animator animator;
    private AudioSource audioSource;    // 점프 사운드를 재생할 오디오 소스

    private bool isGrounded = false;    // 플레이어가 땅에 있는지 여부
    private bool jumpRequested = false; // 점프를 요청했는지 여부
    private bool isJumping = false;     // 점프 중인지 여부

    private float jumpHoldTime = 0f;    // 스페이스바를 누르고 있는 시간
    private float jumpRequestTime = 0f; // 점프 요청 시점

    public CameraFollow cameraFollow;   // 카메라 움직임을 처리하는 스크립트

    void Start()
    {
        // 필수 컴포넌트 가져오기
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        // 리지드바디 설정: X, Z축 회전을 잠그고 중력 영향 받기
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        HandleMovement();   
        HandleJump();   

        // 불필요한 회전 제거
        rb.angularVelocity = Vector3.zero;
    }

    // 캐릭터 이동 처리
    private void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(moveX, 0, moveZ);

        // 움직임이 충분히 클 때만 회전
        if (moveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        // 리지드바디를 통해 이동 처리
        rb.velocity = new Vector3(moveDirection.x * moveSpeed, rb.velocity.y, moveDirection.z * moveSpeed);

        // 애니메이터의 이동 속도 업데이트
        animator.SetFloat("moveSpeed", moveDirection.magnitude);
    }

    // 점프 처리
    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            jumpRequested = true;
            jumpRequestTime = Time.time;
            cameraFollow.SetJumping(true);
        }

        if (jumpRequested && Input.GetKey(KeyCode.Space))
        {
            jumpHoldTime = Time.time - jumpRequestTime;
        }
        else if (jumpRequested && !Input.GetKey(KeyCode.Space))
        {
            float jumpForce = Mathf.Clamp(minJumpForce + (jumpHoldTime * maxJumpForce), minJumpForce, maxJumpForce);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            
            jumpRequested = false;
            isGrounded = false;
            jumpHoldTime = 0f;
            isJumping = true;
            animator.SetBool("isJumping", true);
            audioSource.PlayOneShot(jumpSound);
            cameraFollow.SetJumping(false);
        }
    }

    // 충돌 처리: 땅과 충돌 시 점프 상태 해제
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (isJumping)
            {
                isJumping = false; // 점프가 끝났음을 나타냄
                animator.SetBool("isJumping", false);
            }
            isGrounded = true; // 땅에 닿았음을 나타냄
        }
    }
}