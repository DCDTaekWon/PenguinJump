using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // TextMeshPro 네임스페이스 추가

public class TitleManager : MonoBehaviour
{
    // 시작 버튼 클릭 시 호출
    public void OnStartButtonClicked()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; // 씬 로드 이벤트 등록
        SceneManager.LoadScene("PlayGame"); // "PlayGame"을 실제 게임 씬 이름으로 변경
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        // 타이틀 씬에서는 UI를 재연결하지 않음
        if (scene.name == "Title")
        {
            Debug.Log("Title scene loaded, no UI to rebind.");
            return;
        }

        // 게임 씬 또는 클리어 씬에서 UI 재연결
        TMP_Text newScoreText = GameObject.Find("ScoreText")?.GetComponent<TMP_Text>();
        TMP_Text newHighScoreText = GameObject.Find("HighScoreText")?.GetComponent<TMP_Text>();

        if (SecureScoreManager.Instance != null && newScoreText != null && newHighScoreText != null)
        {
            SecureScoreManager.Instance.RebindUI(newScoreText, newHighScoreText);
        }
        else
        {
            Debug.LogWarning("Failed to rebind SecureScoreManager UI in scene: " + scene.name);
        }
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
