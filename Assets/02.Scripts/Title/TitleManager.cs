using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    // 시작 버튼 클릭 시 호출
    public void OnStartButtonClicked()
    {
        SceneManager.LoadScene("PlayGame"); // "PlayGame"을 실제 게임 씬 이름으로 변경
    }

    // 종료 버튼 클릭 시 호출
    public void OnEndButtonClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
