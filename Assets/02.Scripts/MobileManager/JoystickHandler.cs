using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickHandler : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    public RectTransform outerCircle; // 큰 원
    public RectTransform innerCircle; // 작은 원
    private Vector2 inputVector;

    public Vector2 InputVector => inputVector;

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

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        inputVector = Vector2.zero;
        innerCircle.anchoredPosition = Vector2.zero;
    }
}
