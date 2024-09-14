using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Animator))]
public class PenguinController : MonoBehaviour
{
    public float moveSpeed = 5f;       // 이동 속도
    public float maxJumpForce = 10f;   // 최대 점프 힘
    public float minJumpForce = 2f;    // 최소 점프 힘
    public float jumpHoldTime = 0f;    // 스페이스바를 누른 시간
    public AudioClip jumpSound;        // 점프 소리
    private AudioSource audioSource;   // 오디오 소스

    private Rigidbody rb;
    private Animator animator;
    private bool isJumping = false;    // 점프 중인지 여부
    private bool isGrounded = false;   // 땅에 있는지 여부
    private bool jumpRequested = false; // 점프 요청 여부
    private float jumpRequestTime = 0f; // 점프 요청 시간
    public CameraFollow cameraFollow;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        // 오디오 소스 설정
        audioSource = GetComponent<AudioSource>();

        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        // 방향키 입력을 받아 이동 속도 계산
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(moveX, 0, moveZ);

        // 캐릭터가 이동하는 방향으로 회전
        if (move.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        // 이동 처리
        rb.velocity = new Vector3(move.x * moveSpeed, rb.velocity.y, move.z * moveSpeed);

        // Animator의 Speed 파라미터에 이동 속도 전달
        animator.SetFloat("moveSpeed", move.magnitude);

        // 점프 요청 처리
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            jumpRequested = true;
            jumpRequestTime = Time.time; // 점프 요청 시간 기록
            cameraFollow.SetJumping(true);
        }

        if (jumpRequested && isGrounded)
        {
            // 점프 입력이 계속될 때 점프 힘을 증가
            if (Input.GetKey(KeyCode.Space))
            {
                jumpHoldTime = Time.time - jumpRequestTime;
            }
            else
            {
                // 스페이스바를 뗄 때 점프 실행
                float jumpForce = Mathf.Clamp(minJumpForce + (jumpHoldTime * maxJumpForce), minJumpForce, maxJumpForce);
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

                isJumping = true;
                isGrounded = false;
                animator.SetBool("isJumping", true);

                // 점프할 때 소리 재생 (점프 시에만 소리)
                if (audioSource != null && jumpSound != null)
                {
                    audioSource.PlayOneShot(jumpSound);
                }

                jumpRequested = false;
                jumpHoldTime = 0f;
                cameraFollow.SetJumping(false);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (isJumping) // 점프 중에 땅과 충돌한 경우
            {
                isJumping = false; // 점프가 끝났음을 나타냄
                animator.SetBool("isJumping", false);
            }
            isGrounded = true; // 땅에 닿았음을 나타냄
        }
    }
}