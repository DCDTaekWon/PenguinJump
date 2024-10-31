using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    // ���� ��ư Ŭ�� �� ȣ��
    public void OnStartButtonClicked()
    {
        SceneManager.LoadScene("PlayGame"); // "PlayGame"�� ���� ���� �� �̸����� ����
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
