using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject bugPrefab;
    public GameObject itemPrefab;
    public float spawnInterval = 1.0f; // 생성 간격
    public float spawnRangeX = 10f; // 화면 밖 X 범위
    public float spawnRangeY = 6f; // 화면 밖 Y 범위

    void Start()
    {
        InvokeRepeating("SpawnBug", 0f, spawnInterval); // 일정 시간마다 SpawnBug 함수 반복 실행
    }

    // Random Spawn Bug(80%) or Item(20%)
    void SpawnBug()
    {
        // Debug.Log("버그 생성 시도!"); 

        // 1. 랜덤 스폰 위치 반환
        Vector3 spawnPos = GetRandomSpawnPos();
        // 2. Bug or Item 생성
        GameObject prefabToSpawn = (Random.value < 0.2f) ? itemPrefab : bugPrefab; // 20% 확률로 아이템 생성
        GameObject spawnObject = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
        // 3. 생성된 버그가 플레이어(중앙) 방향으로 날아가도록 설정
        Vector2 direction = (Vector2.zero - (Vector2)spawnPos).normalized;
        spawnObject.GetComponent<Rigidbody2D>().linearVelocity = direction * 3f;
    }

    // Random Spawn Position 
    //   상하좌우 중 랜덤하게 한 면을 선택해 화면 밖 좌표(Object 생성 위치) 반환
    Vector3 GetRandomSpawnPos()
    {
        int side = Random.Range(0, 4);
        Vector3 spawnPos = new Vector3(0, 0, 0);

        // 상
        if (side==0) {
            spawnPos.x=Random.Range(-spawnRangeX, spawnRangeX); 
            spawnPos.y=spawnRangeY;
        }
        // 하
        else if (side==1)
        {
            spawnPos.x=Random.Range(-spawnRangeX, spawnRangeX); 
            spawnPos.y=-spawnRangeY;
        }
        // 좌
        else if (side==2)
        {
            spawnPos.x=-spawnRangeX; 
            spawnPos.y=Random.Range(-spawnRangeY, spawnRangeY);
        }
        // 우
        else if (side==3)
        {
            spawnPos.x=spawnRangeX; 
            spawnPos.y=Random.Range(-spawnRangeY, spawnRangeY);
        }

        return spawnPos;
    }
}
