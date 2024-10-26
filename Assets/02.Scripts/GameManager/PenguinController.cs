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

    private bool isGrounded = true;
    private bool jumpRequested = false;
    private bool isJumping = false;
    private bool moveKeyPressed = false;

    private float jumpHoldTime = 0f;
    private float jumpRequestTime = 0f;

    public CameraFollow cameraFollow;
    public DeathPopupManager deathPopupManager; // DeathPopupManager 참조 추가

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    void Update()
    {
        HandleMovement();
        HandleJumpInput();
    }

    void FixedUpdate()
    {
        ApplyJump();
    }

    private void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(moveX, 0, moveZ);
        moveKeyPressed = moveDirection.magnitude > 0.1f;

        if (moveKeyPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        rb.velocity = new Vector3(moveDirection.x * moveSpeed, rb.velocity.y, moveDirection.z * moveSpeed);
        animator.SetFloat("moveSpeed", moveDirection.magnitude);
    }

    private void HandleJumpInput()
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
    }

    private void ApplyJump()
    {
        if (jumpRequested && !Input.GetKey(KeyCode.Space))
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

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            if (isJumping)
            {
                isJumping = false;
                animator.SetBool("isJumping", false);
            }
        }
    }

    // DeathZone과의 충돌 처리
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeathZone"))
        {
            // DeathPopupManager 호출
            if (deathPopupManager != null)
            {
                deathPopupManager.ShowDeathPopup();
            }
        }
    }
}
