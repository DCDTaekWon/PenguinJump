using UnityEngine;
using UnityEngine.SceneManagement;  // �� �ε带 ���� �ʿ�

public class GameRestart : MonoBehaviour
{
    void Update()
    {
        // R Ű�� ������ �� PlayGame �� �����
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }

    void RestartGame()
    {
        // PlayGame ���� �ٽ� �ε�
        SceneManager.LoadScene("PlayGame");
        Debug.Log("�����");
    }
}

