using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;            // 카메라가 따라갈 플레이어(펭귄) 오브젝트
    public Vector3 offset;              // 기본 카메라 오프셋 (자연스럽게 따라가는 위치)
    public float smoothSpeed = 0.125f;  // 카메라 이동의 부드러움 정도
    public float jumpHeightOffset = 2.0f;  // 점프 시 카메라가 올라가는 정도
    public float jumpSmoothSpeed = 0.02f;   // 점프 시 카메라 이동의 부드러움 정도
    public float zoomOutDistance = 5f;   // 점프 시 카메라가 줌 아웃되는 거리
    public float maxZoomOutDistance = 3f;  // 카메라가 최대 줌 아웃할 수 있는 제한 거리

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
        Vector3 desiredPosition = target.position + offset;

        // 점프 중일 때 카메라를 약간 위로 이동 및 줌 아웃
        if (isJumping)
        {
            desiredPosition.y += jumpHeightOffset;  // 점프 시 카메라 위로 이동

            // 줌 아웃 거리가 최대값을 넘지 않도록 제한
            offset.z = Mathf.Clamp(offset.z - zoomOutDistance, -maxZoomOutDistance, originalOffset.z);
        }
        else
        {
            // 점프가 아닐 때는 줌 아웃을 원래대로 복원
            offset = originalOffset;
        }

        // 점프 중일 때와 아닐 때의 smoothSpeed 조정
        float currentSmoothSpeed = isJumping ? jumpSmoothSpeed : originalSmoothSpeed;

        // 카메라의 현재 위치와 목표 위치 사이를 부드럽게 이동
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, currentSmoothSpeed);
        transform.position = smoothedPosition;

        // 카메라가 항상 펭귄을 바라보도록 설정
        transform.LookAt(target);
    }

    // 펭귄의 점프 상태를 외부에서 설정할 수 있도록 함수 추가
    public void SetJumping(bool jumping)
    {
        isJumping = jumping;
    }
}





