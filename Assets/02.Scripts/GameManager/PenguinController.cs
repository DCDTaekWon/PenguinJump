using UnityEngine;

public class PenguinController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Jump Settings")]
    public float maxJumpForce = 10f;
    public float minJumpForce = 2f;
    public AudioClip jumpSound;

    private Rigidbody rb;
    private Animator animator;
    private AudioSource audioSource;

    private bool isGrounded = true;
    private bool jumpRequested = false;
    private bool isJumping = false;
    private float jumpHoldTime = 0f;
    private float jumpRequestTime = 0f;

    [Header("References")]
    public CameraFollow cameraFollow;
    public DeathPopupManager deathPopupManager;

    [Header("Indicator Settings")]
    public GameObject groundPrefab; // Ground 인디케이터 프리팹
    public GameObject waterPrefab;  // Water 인디케이터 프리팹
    private GameObject currentIndicator; // 현재 활성화된 인디케이터
    private float groundOffsetY = 0.1f;

    public LayerMask WaterLayer;
    public LayerMask GroundLayer;

    private Camera mainCamera;

    [Header("Mobile Settings")]
    private JoystickHandler joystick; // 가상 조이스틱
    public bool isMobile = true;      // 모바일 환경 여부 설정 (테스트용)

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        mainCamera = Camera.main;

        if (isMobile)
        {
            joystick = FindObjectOfType<JoystickHandler>();
        }
    }

    void Update()
    {
        HandleMovement();
        HandleJumpInput();
        UpdateIndicator();
    }

    void FixedUpdate()
    {
        // 점프는 FixedUpdate에서 처리하지 않음, 입력과 ApplyJump가 Update로 처리됨
    }

    private void HandleMovement()
    {
        float moveX, moveZ;

        if (!isMobile)
        {
            moveX = Input.GetAxis("Horizontal");
            moveZ = Input.GetAxis("Vertical");
        }
        else
        {
            Vector2 joystickInput = joystick.InputVector;
            moveX = joystickInput.x;
            moveZ = joystickInput.y;
        }

        Vector3 moveDirection = new Vector3(moveX, 0, moveZ);
        if (moveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        rb.velocity = new Vector3(moveDirection.x * moveSpeed, rb.velocity.y, moveDirection.z * moveSpeed);
        animator.SetFloat("moveSpeed", moveDirection.magnitude);
    }

    private void HandleJumpInput()
    {
        // 점프 시작
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            RequestJump();
        }

        // 점프 충전
        if (jumpRequested && Input.GetKey(KeyCode.Space))
        {
            jumpHoldTime = Time.time - jumpRequestTime;
        }

        // 점프 실행
        if (Input.GetKeyUp(KeyCode.Space) && jumpRequested)
        {
            ApplyJump();
        }
    }

    private void ApplyJump()
    {
        if (jumpRequested)
        {
            float jumpForce = Mathf.Clamp(minJumpForce + (jumpHoldTime * maxJumpForce), minJumpForce, maxJumpForce);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            jumpRequested = false;
            jumpHoldTime = 0f;
            isGrounded = false;
            isJumping = true;

            animator.SetBool("isJumping", true);
            audioSource.PlayOneShot(jumpSound);
            cameraFollow.SetJumping(false);
        }
    }

    public void RequestJump()
    {
        if (isGrounded)
        {
            jumpRequested = true;
            jumpRequestTime = Time.time;
            cameraFollow.SetJumping(true);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            isJumping = false;
            animator.SetBool("isJumping", false);
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

            if (((1 << hit.collider.gameObject.layer) & WaterLayer) != 0)
            {
                SetIndicator(waterPrefab, indicatorPosition);
            }
            else if (((1 << hit.collider.gameObject.layer) & GroundLayer) != 0)
            {
                SetIndicator(groundPrefab, indicatorPosition);
            }

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
