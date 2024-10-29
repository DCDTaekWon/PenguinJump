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

    public GameObject groundIndicatorPrefab;
    private GameObject groundIndicator;
    private float groundOffsetY = 0.1f;

    public LayerMask WaterLayer;
    public LayerMask GroundLayer;

    public Sprite waterIndicatorSprite;
    public Sprite groundIndicatorSprite;

    private SpriteRenderer indicatorSpriteRenderer;
    private Camera mainCamera; // 카메라 참조

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        groundIndicator = Instantiate(groundIndicatorPrefab);
        indicatorSpriteRenderer = groundIndicator.GetComponent<SpriteRenderer>();

        // 메인 카메라 참조 가져오기
        mainCamera = Camera.main;
    }

    void Update()
    {
        HandleMovement();
        HandleJumpInput();
        UpdateGroundIndicator();
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
            if (deathPopupManager != null)
            {
                deathPopupManager.ShowDeathPopup();
            }
        }
    }

    private void UpdateGroundIndicator()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;

        // Ground 또는 Water 레이어에 닿는지 확인
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, GroundLayer | WaterLayer))
        {
            Vector3 indicatorPosition = hit.point;

            // 공중에 있을 때와 바닥에 있을 때 Y 위치 조정
            if (isGrounded) // 펭귄이 땅에 있는 경우
            {
                indicatorPosition.y -= groundOffsetY; // 겹치지 않도록 살짝 아래로 이동
            }
            else // 공중에 있을 경우
            {
                indicatorPosition.y += groundOffsetY; // 원래 위치로 표시
            }

            groundIndicator.transform.position = indicatorPosition;
            groundIndicator.SetActive(true);

            // 레이어에 따른 스프라이트 설정
            if (((1 << hit.collider.gameObject.layer) & WaterLayer) != 0)
            {
                indicatorSpriteRenderer.sprite = waterIndicatorSprite;
            }
            else if (((1 << hit.collider.gameObject.layer) & GroundLayer) != 0)
            {
                indicatorSpriteRenderer.sprite = groundIndicatorSprite;
            }

            // 스프라이트가 항상 카메라를 바라보도록 회전 설정 (크기는 고정)
            groundIndicator.transform.LookAt(mainCamera.transform);
            groundIndicator.transform.rotation = Quaternion.Euler(90, groundIndicator.transform.rotation.eulerAngles.y, 0); // Y축 회전만 반영
        }
        else
        {
            groundIndicator.SetActive(false);
        }
    }

}
