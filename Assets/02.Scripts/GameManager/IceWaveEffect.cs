using UnityEngine;
using DG.Tweening;

public class IceWaveEffect : MonoBehaviour
{
    public float duration = 3f;  // ȸ�� �ִϸ��̼� ���� �ð�
    public float returnDuration = 2f; // ���� ��ġ�� ���ƿ��� �ð�

    void Start()
    {
        AnimateIce();
    }

    void AnimateIce()
    {
        float randomRotation = Random.Range(-45f, 45f); // -45������ 45�� ������ ���� ��
        transform.DOLocalRotate(new Vector3(0, 0, randomRotation), duration, RotateMode.LocalAxisAdd)
            .SetEase(Ease.InOutSine)
            .OnComplete(() => {
                // ȸ���� ���� �� ���� ������ ���ư���
                transform.DOLocalRotate(Vector3.zero, returnDuration).SetEase(Ease.InOutSine)
                    .OnComplete(() => {
                        // ���ο� ���� ������ �ִϸ��̼� �ݺ�
                        AnimateIce();
                    });
            });
    }
}
