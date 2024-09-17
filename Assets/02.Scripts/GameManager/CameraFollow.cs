using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;            // 카메라가 따라갈 플레이어(펭귄) 오브젝트
    [Header("Camera Offset Settings")]
    public Vector3 offset;              // 기본 카메라 오프셋 (자연스럽게 따라가는 위치)
    public float smoothSpeed = 0.125f;  // 카메라 이동의 부드러움 정도

    [Header("Jump Camera Settings")]
    public Vector3 jumpOffsetMultiplier = new Vector3(1f, 1.5f, 1f);  // 점프 시 X, Y, Z 이동 비율
    public float jumpSmoothSpeed = 0.02f;   // 점프 시 카메라 이동의 부드러움 정도
    public float maxZoomOutDistance = 5f;  // 점프 시 카메라가 줌 아웃되는 최대 거리
    public float minZoomOutDistance = 1f;  // 점프 시 카메라가 줌 아웃되는 최소 거리
    public float maxJumpHeight = 10f;  // 예상되는 최대 점프 높이 (카메라 줌 아웃 거리 연동에 사용)

    private bool isJumping = false;
    private Vector3 originalOffset;
    private float originalSmoothSpeed;

    void Start()
    {
        originalOffset = offset;  // 초기 오프셋 저장
        originalSmoothSpeed = smoothSpeed; // 초기 smoothSpeed 저장
    }

    void LateUpdate()
    {
        // 현재 카메라 이동 속도 설정
        float currentSmoothSpeed = isJumping ? jumpSmoothSpeed : originalSmoothSpeed;

        if (isJumping)
        {
            // 점프 높이에 따라 줌 아웃 거리를 계산
            float jumpHeightFactor = Mathf.InverseLerp(0f, maxJumpHeight, target.position.y);
            float dynamicZoomOut = Mathf.Lerp(minZoomOutDistance, maxZoomOutDistance, jumpHeightFactor);

            // 각 축별로 점프 시 카메라 이동을 계산
            Vector3 jumpOffset = new Vector3(
                offset.x * jumpOffsetMultiplier.x,
                offset.y * jumpOffsetMultiplier.y,
                offset.z * jumpOffsetMultiplier.z
            );

            // 점프 높이에 비례해 카메라가 줌 아웃되도록 설정
            offset.z = Mathf.Lerp(offset.z, Mathf.Clamp(originalOffset.z - dynamicZoomOut, -maxZoomOutDistance, originalOffset.z), currentSmoothSpeed);

            // 카메라가 점프 시 각 축에 맞게 이동하도록 설정
            Vector3 desiredPosition = target.position + jumpOffset + Vector3.up * jumpHeightFactor * jumpOffsetMultiplier.y;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, currentSmoothSpeed);

            transform.position = smoothedPosition;
        }
        else
        {
            // 줌 아웃 후 원래 위치로 돌아오도록 설정
            offset.z = Mathf.Lerp(offset.z, originalOffset.z, currentSmoothSpeed);

            // 점프가 아닐 때 카메라의 원래 위치로 돌아옴
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, currentSmoothSpeed);

            transform.position = smoothedPosition;
        }

        // 카메라가 항상 플레이어를 바라보도록 설정
        transform.LookAt(target);
    }

    // 펭귄의 점프 상태를 외부에서 설정할 수 있도록 함수 추가
    public void SetJumping(bool jumping)
    {
        isJumping = jumping;
    }
}












