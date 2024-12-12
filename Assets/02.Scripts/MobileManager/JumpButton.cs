using UnityEngine;
using UnityEngine.EventSystems;

public class JumpButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public PenguinController penguinController;
    private bool isButtonHeld = false; // 버튼 눌림 상태

    public void OnPointerDown(PointerEventData eventData)
    {
        isButtonHeld = true;
        penguinController?.RequestJump(); // 점프 요청
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isButtonHeld = false;
        penguinController?.ApplyJump(); // 점프 실행
    }

    public bool IsButtonHeld()
    {
        return isButtonHeld; // 현재 버튼 상태 반환
    }
}
