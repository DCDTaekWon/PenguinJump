using UnityEngine;
using UnityEngine.SceneManagement;

public class ProgressBarManager : MonoBehaviour
{
    public Transform progressBarTransform;
    public RectTransform spriteTransform;
    public float startX = 0f;
    public float targetX = 500f;
    public float duration = 240f;

    private float elapsedTime = 0f;

    void Start()
    {
        Vector3 startPosition = spriteTransform.localPosition;
        startPosition.x = startX;
        spriteTransform.localPosition = startPosition;

        StartCoroutine(MoveSprite());
    }

    private System.Collections.IEnumerator MoveSprite()
    {
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            float progress = elapsedTime / duration;
            float newX = Mathf.Lerp(startX, targetX, progress);

            Vector3 currentPosition = spriteTransform.localPosition;
            currentPosition.x = newX;
            spriteTransform.localPosition = currentPosition;

            yield return null;
        }

        if (SecureScoreManager.Instance != null)
        {
            SecureScoreManager.Instance.SetFinalScore(SecureScoreManager.Instance.CurrentScore);
            Debug.Log("Final score set: " + SecureScoreManager.Instance.CurrentScore);
        }



        SceneManager.LoadScene("Clear");
    }
}
