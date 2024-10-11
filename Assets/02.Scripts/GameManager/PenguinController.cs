using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Animator), typeof(AudioSource))]
public class PenguinController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float maxJumpForce = 10f;
    public float minJumpForce = 2f;
    public AudioClip jumpSound;

    private Rigidbody rb;
    private Animator animator;
    private AudioSource audioSource;

    private bool isGrounded = true;      // 처음엔 땅에 있는 상태로 설정
    private bool jumpRequested = false;  // 점프 요청 상태
    private bool isJumping = false;
    private bool moveKeyPressed = false;

    private float jumpHoldTime = 0f;
    private float jumpRequestTime = 0f;

    public CameraFollow cameraFollow;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    void Update()  // 입력 처리는 Update에서!
    {
        HandleMovement();  // 이동 처리
        HandleJumpInput();  // 점프 입력 처리
    }

    void FixedUpdate()
    {
        ApplyJump();  // 점프는 물리 연산이므로 FixedUpdate에서 처리
    }

    private void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(moveX, 0, moveZ);
        moveKeyPressed = moveDirection.magnitude > 0.1f;  // 이동키가 눌렸는지 확인

        if (moveKeyPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        rb.velocity = new Vector3(moveDirection.x * moveSpeed, rb.velocity.y, moveDirection.z * moveSpeed);
        animator.SetFloat("moveSpeed", moveDirection.magnitude);
    }

    // 점프 입력 처리
    private void HandleJumpInput()
    {
        // 점프키를 눌렀을 때 점프 요청을 설정
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            jumpRequested = true;
            jumpRequestTime = Time.time;
            cameraFollow.SetJumping(true);
        }

        // 점프키를 계속 누르고 있을 때 점프 준비
        if (jumpRequested && Input.GetKey(KeyCode.Space))
        {
            jumpHoldTime = Time.time - jumpRequestTime; // 점프키를 누른 시간 계산
        }
    }

    // 점프는 FixedUpdate에서 처리 (물리 연산)
    private void ApplyJump()
    {
        if (jumpRequested && !Input.GetKey(KeyCode.Space))
        {
            float jumpForce = Mathf.Clamp(minJumpForce + (jumpHoldTime * maxJumpForce), minJumpForce, maxJumpForce);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // 점프 힘 적용

            // 점프 완료 후 상태 초기화
            jumpRequested = false;
            isGrounded = false;  // 땅을 떠난 상태
            jumpHoldTime = 0f;
            isJumping = true;
            animator.SetBool("isJumping", true);
            audioSource.PlayOneShot(jumpSound);
            cameraFollow.SetJumping(false);
        }
    }

    // 땅에 닿았을 때만 isGrounded를 true로 설정
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;   // 땅에 닿았을 때만 true로 설정
            if (isJumping)
            {
                isJumping = false;  // 점프 상태 해제
                animator.SetBool("isJumping", false);
            }
        }
    }
}
