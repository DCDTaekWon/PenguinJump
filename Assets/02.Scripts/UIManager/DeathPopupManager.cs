using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathPopupManager : MonoBehaviour
{
    [Tooltip("�Ͻ����� �˾� �г�")]
    public GameObject DeathPanel;

    [SerializeField] private GameObject deathPopupPanel;
    [Tooltip("Ÿ��Ʋ�� �̵��ϴ� ��ư")]
    public Button restartButton;
    [Tooltip("���� �簳 ��ư")]
    public Button resumeButton;

    private bool isPaused = false;

    void Start()
    {
        // ���� �� �˾� �г� ��Ȱ��ȭ
        if (DeathPanel != null)
        {
            DeathPanel.SetActive(false);
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


    public void ShowDeathPopup()
    {
        if (isPaused)
        {
            // �˾��� �̹� Ȱ��ȭ�Ǿ� �ִٸ� �߰� �۾� ���� return
            return;
        }

        Debug.Log("�˾� Ȱ��ȭ!");
        isPaused = true;
        Time.timeScale = 0f; // ���� �Ͻ�����

        if (deathPopupPanel != null)
        {
            deathPopupPanel.SetActive(true); // �˾� Ȱ��ȭ
        }

        Cursor.lockState = CursorLockMode.None; // ���콺 ��� ����
        Cursor.visible = true; // ���콺 ǥ��
    }

    public void ResumeGame()
    {
        if (!isPaused) return; // �̹� �簳 ���¶�� ����

        Debug.Log("ResumeGame called.");
        isPaused = false;
        Time.timeScale = 1f; // ���� �ð� ����ȭ

        // ���� ���� �ٽ� �ε��Ͽ� ������ �����
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        // �˾� �г� ��Ȱ��ȭ
        if (
            DeathPanel != null)
        {
            DeathPanel.SetActive(false);
        }

        // ���콺 Ŀ�� ó��
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }


    public void RestartToCutscene()
    {
        Debug.Log("Restart button clicked. Returning to title scene.");
        Time.timeScale = 1f; // �� �̵� �� �ð� ����ȭ
        SceneManager.LoadScene("Title"); // Ÿ��Ʋ �� �̸����� ��ü
    }
}
