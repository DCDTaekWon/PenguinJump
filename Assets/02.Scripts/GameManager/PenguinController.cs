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
    public DeathPopupManager deathPopupManager;

    public GameObject groundPrefab; // Ground 인디케이터 프리팹
    public GameObject waterPrefab;  // Water 인디케이터 프리팹
    private GameObject currentIndicator; // 현재 활성화된 인디케이터

    private float groundOffsetY = 0.1f;

    public LayerMask WaterLayer;
    public LayerMask GroundLayer;

    private Camera mainCamera;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        mainCamera = Camera.main;
    }

    void Update()
    {
        HandleMovement();
        HandleJumpInput();
        UpdateIndicator();
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeathZone"))
        {
            deathPopupManager?.ShowDeathPopup();
        }
    }

    private void UpdateIndicator()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, GroundLayer | WaterLayer))
        {
            Vector3 indicatorPosition = hit.point;
            indicatorPosition.y += isGrounded ? -groundOffsetY : groundOffsetY;

            // 현재 활성화된 인디케이터가 있는지 확인 후 교체
            if (((1 << hit.collider.gameObject.layer) & WaterLayer) != 0)
            {
                SetIndicator(waterPrefab, indicatorPosition);
            }
            else if (((1 << hit.collider.gameObject.layer) & GroundLayer) != 0)
            {
                SetIndicator(groundPrefab, indicatorPosition);
            }

            // 인디케이터를 카메라 방향으로 회전
            currentIndicator.transform.LookAt(mainCamera.transform);
            currentIndicator.transform.rotation = Quaternion.Euler(90, currentIndicator.transform.rotation.eulerAngles.y, 0);
        }
        else
        {
            if (currentIndicator != null) currentIndicator.SetActive(false);
        }
    }

    private void SetIndicator(GameObject prefab, Vector3 position)
    {
        // 새로운 인디케이터가 필요하면 기존 인디케이터 삭제 및 새로 생성
        if (currentIndicator == null || currentIndicator.name != prefab.name)
        {
            if (currentIndicator != null)
            {
                Destroy(currentIndicator);
            }
            currentIndicator = Instantiate(prefab, position, Quaternion.identity);
        }
        else
        {
            currentIndicator.transform.position = position;
            currentIndicator.SetActive(true);
        }
    }
}
