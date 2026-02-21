using UnityEngine;

public class BasicPattern : MonoBehaviour
{
    [Header("설정")]
    public GameObject bugPrefab;
    public float spawnRangeX = 10f;
    public float spawnRangeY = 6f;

    // Spawner로부터 현재 난이도에 맞는 속도를 전달받아 생성합니다.
    public void Execute(float currentSpeed)
    {
        Vector3 spawnPos = GetRandomSpawnPos();
        GameObject go = Instantiate(bugPrefab, spawnPos, Quaternion.identity);

        // 중앙(0,0)을 향하는 방향 계산
        Vector2 direction = ((Vector2)Vector3.zero - (Vector2)spawnPos).normalized;

        Rigidbody2D rb = go.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * currentSpeed;
        }
    }

    // 화면 밖 랜덤 위치 계산 (기존 로직 이동)
    private Vector3 GetRandomSpawnPos()
    {
        int side = Random.Range(0, 4);
        Vector3 spawnPos = Vector3.zero;

        if (side == 0) { // 상
            spawnPos.x = Random.Range(-spawnRangeX, spawnRangeX);
            spawnPos.y = spawnRangeY;
        }
        else if (side == 1) { // 하
            spawnPos.x = Random.Range(-spawnRangeX, spawnRangeX);
            spawnPos.y = -spawnRangeY;
        }
        else if (side == 2) { // 좌
            spawnPos.x = -spawnRangeX;
            spawnPos.y = Random.Range(-spawnRangeY, spawnRangeY);
        }
        else if (side == 3) { // 우
            spawnPos.x = spawnRangeX;
            spawnPos.y = Random.Range(-spawnRangeY, spawnRangeY);
        }
        return spawnPos;
    }
}