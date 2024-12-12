using UnityEngine;
using UnityEngine.EventSystems;

public class JumpButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public PenguinController penguinController;
    private bool isButtonHeld = false; // ��ư ���� ����

    public void OnPointerDown(PointerEventData eventData)
    {
        isButtonHeld = true;
        penguinController?.RequestJump(); // ���� ��û
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isButtonHeld = false;
        penguinController?.ApplyJump(); // ���� ����
    }

    public bool IsButtonHeld()
    {
        return isButtonHeld; // ���� ��ư ���� ��ȯ
    }
}
