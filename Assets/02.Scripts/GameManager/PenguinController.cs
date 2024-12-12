using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class PenguinController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Jump Settings")]
    public float maxJumpForce = 10f;
    public float minJumpForce = 2f;
    public AudioClip jumpSound;
    public AudioClip deathSound;
    private AudioSource audioSource; // 플레이어의 AudioSource

    private Rigidbody rb;
    private Animator animator;


    public bool isGrounded = true;
    public bool jumpRequested = false;
    public bool isJumping = false;
    public float jumpHoldTime = 0f;
    public float jumpRequestTime = 0f;

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
    public SecureScoreManager scoreManager; // SecureScoreManager 참조

    [Header("Sensitivity Settings")]
    public float joystickSensitivity = 1f; // 기본 민감도 값
    private FeverTimeManager feverTimeManager; // FeverTimeManager 참조

    /// <summary>
    /// 게임 오버 UI 플래그
    /// </summary>
    public bool isGameFlag { get; private set; } = false;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>(); // 현재 오브젝트의 AudioSource 가져오기

        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        mainCamera = Camera.main;

        if (isMobile)
        {
            joystick = FindObjectOfType<JoystickHandler>();
        }
        scoreManager = FindObjectOfType<SecureScoreManager>();
        feverTimeManager = FindObjectOfType<FeverTimeManager>(); // FeverTimeManager 연결
    }



    void Update()
    {
        // Rigidbody가 Kinematic 상태이면 이동 및 점프 로직 비활성화
        if (rb.isKinematic) return;

        HandleMovement();

        if (!isMobile)
        {
            HandleJumpInput(); // 기존 키보드 점프 처리
        }
        else
        {
            HandleMobileJumpInput(); // 모바일 점프 처리
        }

        UpdateIndicator();
        StopGame();
    }


    private void HandleMobileJumpInput()
    {
        // JumpButton 스크립트 가져오기
        JumpButton jumpButton = FindObjectOfType<JumpButton>();
        if (jumpButton == null) return;

        // 점프 충전
        if (jumpButton.IsButtonHeld() && jumpRequested)
        {
            jumpHoldTime = Time.time - jumpRequestTime;
        }
    }


    // HandleMovement 메서드 수정
    private void HandleMovement()
    {
        float moveX = 0f, moveZ = 0f;

        if (!isMobile)
        {
            moveX = Input.GetAxis("Horizontal");
            moveZ = Input.GetAxis("Vertical");
        }
        else if (joystick != null)
        {
            Vector2 joystickInput = joystick.InputVector;
            moveX = joystickInput.x * joystickSensitivity; // 민감도 적용
            moveZ = joystickInput.y * joystickSensitivity; // 민감도 적용
        }

        // 이동 방향 계산
        Vector3 moveDirection = new Vector3(moveX, 0, moveZ).normalized;
        Vector3 targetVelocity = moveDirection * moveSpeed;

        // 움직임 멈춤 처리: 조이스틱 입력이 없을 경우
        if (moveDirection.magnitude < 0.1f)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            animator.SetFloat("moveSpeed", 0);
            return;
        }

        // 속도의 변화율을 부드럽게 하기 위해 Lerp 사용
        Vector3 smoothedVelocity = Vector3.Lerp(rb.velocity, targetVelocity, Time.deltaTime * 10f);

        rb.velocity = new Vector3(smoothedVelocity.x, rb.velocity.y, smoothedVelocity.z);

        if (moveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        animator.SetFloat("moveSpeed", moveDirection.magnitude);
    }



    private void HandleJumpInput()
    {
        if (!isMobile)
        {
            // 키보드 입력 처리
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                RequestJump();
            }

            if (jumpRequested && Input.GetKey(KeyCode.Space))
            {
                jumpHoldTime = Time.time - jumpRequestTime;
            }

            if (Input.GetKeyUp(KeyCode.Space) && jumpRequested)
            {
                ApplyJump();
            }
        }
        // 모바일 입력은 JumpButton.cs에서 처리
    }


    public void ApplyJump()
    {
        if (jumpRequested)
        {
            // 점프 힘 계산
            float jumpForce = Mathf.Clamp(minJumpForce + (jumpHoldTime * maxJumpForce), minJumpForce, maxJumpForce);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            jumpRequested = false;
            jumpHoldTime = 0f; // 충전 시간 초기화
            isGrounded = false;
            isJumping = true;

            animator.SetBool("isJumping", true);
            audioSource.PlayOneShot(jumpSound);
            cameraFollow.SetJumping(false);

            if (scoreManager != null)
            {
                int jumpScore = feverTimeManager != null && feverTimeManager.IsFeverActive ? 500 : 50;
                scoreManager.AddScore(jumpScore);
            }
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
        Debug.Log($"OnTriggerEnter 호출됨! 충돌 대상: {other.gameObject.name}");

        if (other.CompareTag("DeathZone"))
        {
            Debug.Log("DeathZone과 충돌!");

            // 사망 애니메이션 실행
            animator.SetTrigger("Death");
            audioSource.PlayOneShot(deathSound);

            // Eyes Layer의 Weight를 1로 설정 (눈 감기 상태 활성화)
            animator.SetLayerWeight(1, 1f);

            // SoundManager 오브젝트 비활성화
            GameObject soundManager = GameObject.Find("SoundManager");
            if (soundManager != null)
            {
                soundManager.SetActive(false); // SoundManager 비활성화
                Debug.Log("SoundManager 비활성화됨");
            }
            else
            {
                Debug.LogWarning("SoundManager 오브젝트를 찾을 수 없습니다!");
            }

            // 3초 후 게임 멈춤
            StartCoroutine(StopGameAfterDelay(2f));
        }
    }

    private IEnumerator StopGameAfterDelay(float delay)
    {
        // 게임을 멈추기 전 잠시 대기
        yield return new WaitForSeconds(delay);

        // 게임을 멈추기 위한 플래그를 true로 설정
        if (!isGameFlag) // 이미 게임이 멈춘 상태가 아니라면
        {
            isGameFlag = true; // 게임 멈추기
        }
    }

    private void StopGame()
    {
        // isGameFlag가 false일 때만 게임 멈추기
        if (!isGameFlag) return;

        // 게임 멈춤 로직
        Debug.Log("Game Stopped!");
        deathPopupManager?.ShowDeathPopup();

        // 이제는 더 이상 반복적으로 호출되지 않도록 처리
        isGameFlag = false; // 게임을 멈춘 후 플래그를 초기화하여 반복을 방지
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
