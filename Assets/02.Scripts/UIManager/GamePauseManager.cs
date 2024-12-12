using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePauseManager : MonoBehaviour
{
    [Tooltip("일시정지 팝업 패널")]
    public GameObject pausePanel;

    [Tooltip("소리 ON/OFF 버튼")]
    public Button soundToggleButton;

    [Tooltip("소리 ON/OFF 스프라이트")]
    public Sprite soundOnSprite;
    public Sprite soundOffSprite;

    [Tooltip("타이틀로 이동하는 버튼")]
    public Button restartButton;

    [Tooltip("게임 재개 버튼")]
    public Button resumeButton;

    private bool isPaused = false;
    private bool isSoundOn = true; // 기본 소리 상태: 켜짐

    void Start()
    {
        // 시작 시 팝업 패널 비활성화
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        // 소리 ON/OFF 버튼 초기화
        if (soundToggleButton != null)
        {
            isSoundOn = AudioListener.volume > 0;
            UpdateSoundButtonImage();
            soundToggleButton.onClick.AddListener(ToggleSound);
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

    void Update()
    {
        // ESC 키 입력 감지
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Debug.Log("ESC pressed: Resuming game.");
                ResumeGame();
            }
            else
            {
                Debug.Log("ESC pressed: Pausing game.");
                PauseGame();
            }
        }
    }

    // 게임 일시정지
    public void PauseGame()
    {
        if (isPaused) return; // 이미 일시정지 상태라면 무시

        Debug.Log("PauseGame called.");
        isPaused = true;
        Time.timeScale = 0f; // 게임 시간 정지
        if (pausePanel != null)
        {
            pausePanel.SetActive(true); // 팝업 패널 활성화
        }
        //Cursor.lockState = CursorLockMode.None;
        //Cursor.visible = true;
    }

    // 게임 재개
    public void ResumeGame()
    {
        if (!isPaused) return; // 이미 재개 상태라면 무시

        Debug.Log("ResumeGame called.");
        isPaused = false;
        Time.timeScale = 1f; // 게임 시간 정상화
        if (pausePanel != null)
        {
            pausePanel.SetActive(false); // 팝업 패널 비활성화
        }
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

    // 소리 ON/OFF 버튼 토글 기능
    private void ToggleSound()
    {
        isSoundOn = !isSoundOn;
        AudioListener.volume = isSoundOn ? 1f : 0f;
        UpdateSoundButtonImage();
        Debug.Log("Sound toggled. isSoundOn: " + isSoundOn);
    }

    // 소리 버튼 이미지 업데이트
    private void UpdateSoundButtonImage()
    {
        if (soundToggleButton != null)
        {
            soundToggleButton.image.sprite = isSoundOn ? soundOnSprite : soundOffSprite;
        }
    }

    // 타이틀로 이동
    public void RestartToCutscene()
    {
        Debug.Log("Restart button clicked. Returning to title scene.");
        Time.timeScale = 1f; // 씬 이동 전 시간 정상화
        SceneManager.LoadScene("Title"); // 타이틀 씬 이름으로 교체
    }
}
