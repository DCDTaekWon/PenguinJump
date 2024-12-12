using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePauseManager : MonoBehaviour
{
    [Tooltip("�Ͻ����� �˾� �г�")]
    public GameObject pausePanel;

    [Tooltip("�Ҹ� ON/OFF ��ư")]
    public Button soundToggleButton;

    [Tooltip("�Ҹ� ON/OFF ��������Ʈ")]
    public Sprite soundOnSprite;
    public Sprite soundOffSprite;

    [Tooltip("Ÿ��Ʋ�� �̵��ϴ� ��ư")]
    public Button restartButton;

    [Tooltip("���� �簳 ��ư")]
    public Button resumeButton;

    private bool isPaused = false;
    private bool isSoundOn = true; // �⺻ �Ҹ� ����: ����

    void Start()
    {
        // ���� �� �˾� �г� ��Ȱ��ȭ
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        // �Ҹ� ON/OFF ��ư �ʱ�ȭ
        if (soundToggleButton != null)
        {
            isSoundOn = AudioListener.volume > 0;
            UpdateSoundButtonImage();
            soundToggleButton.onClick.AddListener(ToggleSound);
        }

        // Ÿ��Ʋ�� �̵� ��ư ����
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartToCutscene);
        }

        // ���� �簳 ��ư ����
        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(ResumeGame);
        }

        Debug.Log("GamePauseManager initialized.");
    }

    void Update()
    {
        // ESC Ű �Է� ����
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

    // ���� �Ͻ�����
    public void PauseGame()
    {
        if (isPaused) return; // �̹� �Ͻ����� ���¶�� ����

        Debug.Log("PauseGame called.");
        isPaused = true;
        Time.timeScale = 0f; // ���� �ð� ����
        if (pausePanel != null)
        {
            pausePanel.SetActive(true); // �˾� �г� Ȱ��ȭ
        }
        //Cursor.lockState = CursorLockMode.None;
        //Cursor.visible = true;
    }

    // ���� �簳
    public void ResumeGame()
    {
        if (!isPaused) return; // �̹� �簳 ���¶�� ����

        Debug.Log("ResumeGame called.");
        isPaused = false;
        Time.timeScale = 1f; // ���� �ð� ����ȭ
        if (pausePanel != null)
        {
            pausePanel.SetActive(false); // �˾� �г� ��Ȱ��ȭ
        }
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

    // �Ҹ� ON/OFF ��ư ��� ���
    private void ToggleSound()
    {
        isSoundOn = !isSoundOn;
        AudioListener.volume = isSoundOn ? 1f : 0f;
        UpdateSoundButtonImage();
        Debug.Log("Sound toggled. isSoundOn: " + isSoundOn);
    }

    // �Ҹ� ��ư �̹��� ������Ʈ
    private void UpdateSoundButtonImage()
    {
        if (soundToggleButton != null)
        {
            soundToggleButton.image.sprite = isSoundOn ? soundOnSprite : soundOffSprite;
        }
    }

    // Ÿ��Ʋ�� �̵�
    public void RestartToCutscene()
    {
        Debug.Log("Restart button clicked. Returning to title scene.");
        Time.timeScale = 1f; // �� �̵� �� �ð� ����ȭ
        SceneManager.LoadScene("Title"); // Ÿ��Ʋ �� �̸����� ��ü
    }
}
