using System.Collections;
using UnityEngine;

// Enemy Collide Effect : Camera Shake
public class CameraShake : MonoBehaviour
{
    private Vector3 originalPos;

    void Awake()
    {
        // 카메라의 원래 시작 위치를 저장해둡니다.
        originalPos = transform.localPosition;
    }

    public void TriggerShake(float duration, float magnitude)
    {
        StopAllCoroutines(); // 이미 흔들리고 있다면 멈추고 새로 시작
        StartCoroutine(Shake(duration, magnitude));
    }

    IEnumerator Shake(float duration, float magnitude)
    {
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            // 무작위 위치 계산
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(x, y, originalPos.z);

            elapsed += Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }

        // 흔들림이 끝나면 정확히 원래 위치로 복구
        transform.localPosition = originalPos;
    }
}