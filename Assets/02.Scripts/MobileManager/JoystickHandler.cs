using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickHandler : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public RectTransform outerCircle; // ū ��
    public RectTransform innerCircle; // ���� ��
    private Vector2 inputVector; // �Է� ����

    private CanvasGroup joystickCanvasGroup; // ���̽�ƽ ǥ�ø� ���� CanvasGroup
    private Vector2 initialPosition; // �ʱ� ��ġ ����

    public Vector2 InputVector => inputVector;

    private void Start()
    {
        // ���̽�ƽ �ʱ� ��ġ �� CanvasGroup ����
        initialPosition = outerCircle.anchoredPosition;

        // ���̽�ƽ�� ó������ ������ �ʵ��� ����
        joystickCanvasGroup = outerCircle.GetComponent<CanvasGroup>();
        if (joystickCanvasGroup == null)
        {
            joystickCanvasGroup = outerCircle.gameObject.AddComponent<CanvasGroup>();
        }
        joystickCanvasGroup.alpha = 0; // ����
        joystickCanvasGroup.blocksRaycasts = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Vector2 screenPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)outerCircle.parent, // �θ� �������� ��ġ ���
            eventData.position,
            eventData.pressEventCamera,
            out screenPos
        );

        // Ŭ���� ��ġ�� outerCircle �̵�
        outerCircle.anchoredPosition = screenPos;
        innerCircle.anchoredPosition = Vector2.zero; // ���� ���� �ʱ�ȭ

        // ���̽�ƽ ���̱�
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

        // ū ���� ������ ������ �Է°� ���
        pos = Vector2.ClampMagnitude(pos, outerCircle.sizeDelta.x / 2f);
        innerCircle.anchoredPosition = pos;

        // �Է� ���� ���
        inputVector = new Vector2(pos.x / (outerCircle.sizeDelta.x / 2f), pos.y / (outerCircle.sizeDelta.y / 2f));
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        inputVector = Vector2.zero; // �Է� ���� �ʱ�ȭ
        innerCircle.anchoredPosition = Vector2.zero; // ���� �� �ʱ�ȭ
        outerCircle.anchoredPosition = initialPosition; // ū �� �ʱ� ��ġ�� ����

        // ���̽�ƽ �����
        joystickCanvasGroup.alpha = 0;
        joystickCanvasGroup.blocksRaycasts = false;
    }
}
