using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickHandler : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public RectTransform outerCircle; // 큰 원
    public RectTransform innerCircle; // 작은 원
    private Vector2 inputVector; // 입력 벡터

    private CanvasGroup joystickCanvasGroup; // 조이스틱 표시를 위한 CanvasGroup
    private Vector2 initialPosition; // 초기 위치 저장

    public Vector2 InputVector => inputVector;

    private void Start()
    {
        // 조이스틱 초기 위치 및 CanvasGroup 설정
        initialPosition = outerCircle.anchoredPosition;

        // 조이스틱이 처음에는 보이지 않도록 설정
        joystickCanvasGroup = outerCircle.GetComponent<CanvasGroup>();
        if (joystickCanvasGroup == null)
        {
            joystickCanvasGroup = outerCircle.gameObject.AddComponent<CanvasGroup>();
        }
        joystickCanvasGroup.alpha = 0; // 숨김
        joystickCanvasGroup.blocksRaycasts = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Vector2 screenPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)outerCircle.parent, // 부모 기준으로 위치 계산
            eventData.position,
            eventData.pressEventCamera,
            out screenPos
        );

        // 클릭한 위치로 outerCircle 이동
        outerCircle.anchoredPosition = screenPos;
        innerCircle.anchoredPosition = Vector2.zero; // 내부 원은 초기화

        // 조이스틱 보이기
        joystickCanvasGroup.alpha = 1;
        joystickCanvasGroup.blocksRaycasts = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            outerCircle,
            eventData.position,
            eventData.pressEventCamera,
            out pos
        );

        // 큰 원의 반지름 내에서 입력값 계산
        pos = Vector2.ClampMagnitude(pos, outerCircle.sizeDelta.x / 2f);
        innerCircle.anchoredPosition = pos;

        // 입력 벡터 계산
        inputVector = new Vector2(pos.x / (outerCircle.sizeDelta.x / 2f), pos.y / (outerCircle.sizeDelta.y / 2f));
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        inputVector = Vector2.zero; // 입력 벡터 초기화
        innerCircle.anchoredPosition = Vector2.zero; // 작은 원 초기화
        outerCircle.anchoredPosition = initialPosition; // 큰 원 초기 위치로 복원

        // 조이스틱 숨기기
        joystickCanvasGroup.alpha = 0;
        joystickCanvasGroup.blocksRaycasts = false;
    }
}
