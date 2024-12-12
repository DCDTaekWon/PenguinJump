using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickHandler : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    public RectTransform outerCircle; // ū ��
    public RectTransform innerCircle; // ���� ��
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

        // ū ���� ������ ������ �Է°� ���
        pos = Vector2.ClampMagnitude(pos, outerCircle.sizeDelta.x / 2f);
        innerCircle.anchoredPosition = pos;

        // �Է� ���� ���
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
