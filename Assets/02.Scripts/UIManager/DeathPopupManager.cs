using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathPopupManager : MonoBehaviour
{
    [Tooltip("일시정지 팝업 패널")]
    public GameObject DeathPanel;

    [SerializeField] private GameObject deathPopupPanel;
    [Tooltip("타이틀로 이동하는 버튼")]
    public Button restartButton;
    [Tooltip("게임 재개 버튼")]
    public Button resumeButton;

    private bool isPaused = false;

    void Start()
    {
        // 시작 시 팝업 패널 비활성화
        if (DeathPanel != null)
        {
            DeathPanel.SetActive(false);
        }

        // 타이틀로 이동 버튼 설정
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartToCutscene);
        }

        // 게임 재개 버튼 설정
        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(ResumeGame);
        }

        Debug.Log("GamePauseManager initialized.");
    }


    public void ShowDeathPopup()
    {
        if (isPaused)
        {
            // 팝업이 이미 활성화되어 있다면 추가 작업 없이 return
            return;
        }

        Debug.Log("팝업 활성화!");
        isPaused = true;
        Time.timeScale = 0f; // 게임 일시정지

        if (deathPopupPanel != null)
        {
            deathPopupPanel.SetActive(true); // 팝업 활성화
        }

        Cursor.lockState = CursorLockMode.None; // 마우스 잠금 해제
        Cursor.visible = true; // 마우스 표시
    }

    public void ResumeGame()
    {
        if (!isPaused) return; // 이미 재개 상태라면 무시

        Debug.Log("ResumeGame called.");
        isPaused = false;
        Time.timeScale = 1f; // 게임 시간 정상화

        // 현재 씬을 다시 로드하여 게임을 재시작
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        // 팝업 패널 비활성화
        if (
            DeathPanel != null)
        {
            DeathPanel.SetActive(false);
        }

        // 마우스 커서 처리
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }


    public void RestartToCutscene()
    {
        Debug.Log("Restart button clicked. Returning to title scene.");
        Time.timeScale = 1f; // 씬 이동 전 시간 정상화
        SceneManager.LoadScene("Title"); // 타이틀 씬 이름으로 교체
    }
}
