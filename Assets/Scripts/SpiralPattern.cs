using System.Collections;
using UnityEngine;

public class SpiralPattern : MonoBehaviour
{
    [Header("프리팹 설정")]
    public GameObject bugPrefab;
    public GameObject warningCirclePrefab; // 원형 경고 프리팹

    [Header("기본 나선 설정")]
    public int totalBullets = 60;      // 총 발사 횟수
    public int streamCount = 2;        // 나선 줄기 개수 (이중 나선 등)
    public float spawnInterval = 0.05f;
    public float baseAngleStep = 10f;

    [Header("역동적 변화 설정")]
    public float speedAmplitude = 2f;      // 속도 변화 폭
    public float baseBulletSpeed = 5f;     // 기본 속도
    public float angleAcceleration = 0.1f; // 각도 가속도

    public void Execute()
    {
        StartCoroutine(EnhancedSpiralShoot());
    }

    IEnumerator EnhancedSpiralShoot()
    {
        // 1. [경고 연출] 중앙에서 원형 경고창 깜빡임
        if (warningCirclePrefab != null)
        {
            GameObject warning = Instantiate(warningCirclePrefab, transform.position, Quaternion.identity);
            warning.transform.localScale = new Vector3(2.5f, 2.5f, 1f); // 경고 원 크기
            
            // 3번 깜빡이기
            for (int j = 0; j < 3; j++)
            {
                warning.SetActive(true);
                yield return new WaitForSeconds(0.1f);
                warning.SetActive(false);
                yield return new WaitForSeconds(0.1f);
            }
            Destroy(warning); // 경고가 끝난 후 파괴
        }

        // 2. [발사 로직] 본격적인 나선 발사 시작
        float currentAngle = 0f;
        float currentAngleStep = baseAngleStep;

        for (int i = 0; i < totalBullets; i++)
        {
            // 줄기 개수(streamCount)만큼 동시에 발사
            for (int s = 0; s < streamCount; s++)
            {
                // 줄기별로 각도를 분산 (예: 2줄기면 0도, 180도 방향)
                float streamAngle = currentAngle + (s * (360f / streamCount));
                
                float dirX = Mathf.Cos(streamAngle * Mathf.Deg2Rad);
                float dirY = Mathf.Sin(streamAngle * Mathf.Deg2Rad);
                Vector2 moveDir = new Vector2(dirX, dirY).normalized;

                // 속도에 사인파를 적용해 물결 효과 부여
                float dynamicSpeed = baseBulletSpeed + Mathf.Sin(i * 0.2f) * speedAmplitude;

                GameObject go = Instantiate(bugPrefab, transform.position, Quaternion.identity);
                Rigidbody2D rb = go.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.linearVelocity = moveDir * dynamicSpeed;
                }
            }

            // 회전 각도 가속 및 대기
            currentAngleStep += angleAcceleration;
            currentAngle += currentAngleStep;

            yield return new WaitForSeconds(spawnInterval);
        }
    }
}