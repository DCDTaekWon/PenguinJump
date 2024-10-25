using UnityEngine;
using UnityEngine.SceneManagement;  // 씬 로드를 위해 필요

public class GameRestart : MonoBehaviour
{
    void Update()
    {
        // R 키를 눌렀을 때 PlayGame 씬 재시작
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }

    void RestartGame()
    {
        // PlayGame 씬을 다시 로드
        SceneManager.LoadScene("PlayGame");
        Debug.Log("재시작");
    }
}

