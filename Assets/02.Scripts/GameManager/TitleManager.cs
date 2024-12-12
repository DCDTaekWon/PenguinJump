using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // TextMeshPro ���ӽ����̽� �߰�

public class TitleManager : MonoBehaviour
{
    // ���� ��ư Ŭ�� �� ȣ��
    public void OnStartButtonClicked()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; // �� �ε� �̺�Ʈ ���
        SceneManager.LoadScene("PlayGame"); // "PlayGame"�� ���� ���� �� �̸����� ����
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        // Ÿ��Ʋ �������� UI�� �翬������ ����
        if (scene.name == "Title")
        {
            Debug.Log("Title scene loaded, no UI to rebind.");
            return;
        }

        // ���� �� �Ǵ� Ŭ���� ������ UI �翬��
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


    // ���� ��ư Ŭ�� �� ȣ��
    public void OnEndButtonClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
