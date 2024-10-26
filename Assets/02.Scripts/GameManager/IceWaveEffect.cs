using UnityEngine;
using DG.Tweening;

public class IceWaveEffect : MonoBehaviour
{
    public float duration = 3f;  // 회전 애니메이션 지속 시간
    public float returnDuration = 2f; // 원래 위치로 돌아오는 시간

    void Start()
    {
        AnimateIce();
    }

    void AnimateIce()
    {
        float randomRotation = Random.Range(-45f, 45f); // -45도에서 45도 사이의 랜덤 값
        transform.DOLocalRotate(new Vector3(0, 0, randomRotation), duration, RotateMode.LocalAxisAdd)
            .SetEase(Ease.InOutSine)
            .OnComplete(() => {
                // 회전이 끝난 후 원래 각도로 돌아가기
                transform.DOLocalRotate(Vector3.zero, returnDuration).SetEase(Ease.InOutSine)
                    .OnComplete(() => {
                        // 새로운 랜덤 각도로 애니메이션 반복
                        AnimateIce();
                    });
            });
    }
}
