using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject bugPrefab;
    public GameObject itemPrefab;
    
    public float spawnRangeX = 10f; // 화면 밖 X 범위
    public float spawnRangeY = 6f; // 화면 밖 Y 범위

    // Level up
    public float baseSpawnInterval = 1.2f; // 시작 생성 간격(시작 단계)
    public float minSpawnInterval = 0.5f; // 최저 생성 간격 (마지막 단계)
    public float difficultyScale = 0.03f; // 초당 난이도가 오르는 수치

    public float bugSpeed = 3f; // 시작 버그 속도
    public float maxBugSpeed =  8f; // 최대 버그 속도

    public float timer = 0f;
    public float elaspedTime = 0f; // 게임 시작 후 경과 시간

    void Update()
    {
        // Level up
        //   1. 게임 진행 시간 누적
        elaspedTime += Time.deltaTime; 
        //   2. 현재 난이도에 따라 스폰 간격 계산 
        float curSpawnInterval = Mathf.Max(minSpawnInterval, baseSpawnInterval - (elaspedTime * difficultyScale));
        //   3. 타이머 체크 후 스폰 : InvokeRepeating의 수동 구현
        timer += Time.deltaTime; // timer(스톱워치) : 지난 스폰 이후로 얼마나 지났는지
        if (timer >= curSpawnInterval)
        {
            SpawnBug(elaspedTime);
            timer = 0f;
        }

    }

    // Random Spawn Bug(80%) or Item(20%)
    void SpawnBug(float time)
    {
        // Debug.Log("버그 생성 시도!"); 

        // 1. 랜덤 스폰 위치 반환
        Vector3 spawnPos = GetRandomSpawnPos();
        // 2. Bug or Item 생성
        GameObject prefabToSpawn = (Random.value < 0.2f) ? itemPrefab : bugPrefab; // 20% 확률로 아이템 생성
        GameObject spawnObject = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
        // 3. 생성된 버그가 플레이어(중앙) 방향으로 날아가도록 설정
        // 4. 시간에 따른 이동 속도 계산(점점 빨라지도록)
        float currentSpeed = Mathf.Min(maxBugSpeed, bugSpeed + (time * difficultyScale * 2));
        Vector2 direction = (Vector2.zero - (Vector2)spawnPos).normalized;
        spawnObject.GetComponent<Rigidbody2D>().linearVelocity = direction * currentSpeed;
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
