using System.Collections;
using UnityEngine;

public class TargetingPattern : MonoBehaviour
{
    // 프리팹 설정
    public GameObject bugPrefab;    // 소환할 프리팹
    public GameObject warningLinePrefab;

    // 스폰 범위(화면 밖)
    public float spawnRangeX = 11f; 
    public float spawnRangeY = 7f;

    // 속도 및 타이밍
    public float bulletSpeed = 200f;  // 조준탄은 보통 일반탄보다 매우 빠름
    public float warningDuration = 0.5f; // 경고 지속 시간
    
    private Transform player;

    void Start()
    {
        // 플레이어를 태그로 찾습니다.
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    public void Execute()
    {
        if (player == null) return;
        StartCoroutine(ShootWithWarning());
    }

    IEnumerator ShootWithWarning()
    {
      // 1. 화면 밖 랜덤 생성 위치 계산
      Vector3 spawnPos = GetRandomEdgePos();
      
      // 2. 플레이어를 향한 방향 및 각도 계산
      Vector2 direction = ((Vector2)player.position - (Vector2)spawnPos).normalized;
      float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

      // 3. 경고선 생성 (생성 위치를 화면 밖 spawnPos로 설정)
      GameObject warning = Instantiate(warningLinePrefab, spawnPos, Quaternion.Euler(0, 0, angle - 90));
      
      yield return new WaitForSeconds(warningDuration);

      // 4. 경고선 제거 및 적 생성 (동일한 spawnPos에서 생성)
      if (warning != null) Destroy(warning);
      
      GameObject go = Instantiate(bugPrefab, spawnPos, Quaternion.identity);
      
      // 회전 설정: 적이 이동 방향을 바라보게 함
      go.transform.rotation = Quaternion.Euler(0, 0, angle - 90);

      Rigidbody2D rb = go.GetComponent<Rigidbody2D>();
      if (rb != null)
      {
          rb.linearVelocity = direction * bulletSpeed;
      }
    }

    // 화면 밖 상하좌우 중 한 곳의 위치를 반환하는 함수
    private Vector3 GetRandomEdgePos()
    {
        int side = Random.Range(0, 4);
        Vector3 pos = Vector3.zero;

        if (side == 0) { // 상
            pos = new Vector3(Random.Range(-spawnRangeX, spawnRangeX), spawnRangeY, 0);
        } else if (side == 1) { // 하
            pos = new Vector3(Random.Range(-spawnRangeX, spawnRangeX), -spawnRangeY, 0);
        } else if (side == 2) { // 좌
            pos = new Vector3(-spawnRangeX, Random.Range(-spawnRangeY, spawnRangeY), 0);
        } else if (side == 3) { // 우
            pos = new Vector3(spawnRangeX, Random.Range(-spawnRangeY, spawnRangeY), 0);
        }
        return pos;
    }
}