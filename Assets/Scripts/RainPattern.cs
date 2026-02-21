using System.Collections;
using UnityEngine;

public class RainPattern : MonoBehaviour
{
    [Header("프리팹 설정")]
    public GameObject bugPrefab;
    public GameObject warningLinePrefab;

    [Header("스폰 설정")]
    public int spawnCount = 12;           // 빗줄기 개수
    public float spawnHeight = 6f;
    public float spawnRangeX = 8f;
    public float warningDuration = 0.5f;
    public float spawnInterval = 0.1f;    // 빗방울 사이의 간격 (중요!)

    [Header("속도 설정")]
    public float minSpeed = 5f;           // 비는 조금 빠른 게 제맛입니다.
    public float maxSpeed = 10f;

    public void Execute()
    {
        StartCoroutine(RainWithWarning());
    }

    IEnumerator RainWithWarning()
    {
        // 1. 위치 미리 선정 및 경고창 생성
        float[] spawnXPositions = new float[spawnCount];
        GameObject[] warnings = new GameObject[spawnCount];

        for (int i = 0; i < spawnCount; i++)
        {
            spawnXPositions[i] = Random.Range(-spawnRangeX, spawnRangeX);
            Vector3 warningPos = new Vector3(spawnXPositions[i], 0f, 0f);
            
            warnings[i] = Instantiate(warningLinePrefab, warningPos, Quaternion.identity);
            warnings[i].transform.localScale = new Vector3(0.3f, 20f, 1f); // 얇고 긴 경고선
        }

        // 2. 경고 대기
        yield return new WaitForSeconds(warningDuration);

        // 3. 경고 제거 및 '순차적' 소환
        for (int i = 0; i < spawnCount; i++)
        {
            if (warnings[i] != null) Destroy(warnings[i]);

            // 실제 적 생성
            Vector3 spawnPos = new Vector3(spawnXPositions[i], spawnHeight, 0);
            GameObject bug = Instantiate(bugPrefab, spawnPos, Quaternion.identity);

            // 속도와 방향 설정
            Rigidbody2D rb = bug.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // 각 빗방울마다 속도를 다르게 주면 훨씬 자연스럽습니다.
                float randomSpeed = Random.Range(minSpeed, maxSpeed);
                rb.linearVelocity = Vector2.down * randomSpeed;
            }

            // [핵심] 다음 빗방울이 떨어지기 전까지 잠깐 대기
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}