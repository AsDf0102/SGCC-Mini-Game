using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("난이도 조절(Level up)")]
    public float baseSpawnInterval = 1.2f; // 초기 생성 간격 (가장 느릴 때)
    public float minSpawnInterval = 0.3f;  // 최소 생성 간격 (가장 빠를 때)
    public float difficultyScale = 0.008f;  // 초당 난이도가 올라가는 정도
    public float enemySpeed = 3f;     // 초기 벌레 이동 속도
    public float maxEnemySpeed =  10f; // 최대 벌레 이동 속도

    public float timer = 0f;
    public float elapsedTime = 0f; // 게임 시작 후 흐른 시간 (오타 수정: elasped -> elapsed)
    private int lastPatternMilestone = -1;

    [Header("탄막 패턴")]
    private BasicPattern basicPattern; // 기본 패턴
    private CirclePattern circlePattern; // 원형 패턴
    private TargetingPattern targetingPattern; // 조준 패턴
    private RainPattern rainPattern; // 비 내리는 패턴
    private SpiralPattern spiralPattern; // 나선형 패턴

    void Start()
    {
        basicPattern = GetComponent<BasicPattern>();
        circlePattern = GetComponent<CirclePattern>();
        targetingPattern = GetComponent<TargetingPattern>();
        rainPattern = GetComponent<RainPattern>();
        spiralPattern = GetComponent<SpiralPattern>();
    }
    
    void Update()
    {
        elapsedTime += Time.deltaTime; // 게임 진행 시간 게속 더하기
        timer += Time.deltaTime; // timer : 마지막 스폰 이후 흐른 시간
        
        int phase = GetCurrentPhase(); // 난이도 조절 로직 (Level up)

        if (timer >= GetCurrentInterval())
        {
            HandlePhaseSpawning(phase);
            timer = 0f;
        }
    }

    int GetCurrentPhase()
    {
        if (elapsedTime < 30f) return 1;  // 0~30초: 튜토리얼 (조준탄만 가끔)
        if (elapsedTime < 60f) return 2;  // 30~60초: 중급 (비 패턴 추가)
        if (elapsedTime < 90f) return 3;  // 60~90초: 고급 (나선형 추가)
        return 4;                         // 90~120초: 극한 (아수라장)
    }

    void HandlePhaseSpawning(int phase)
    {
        int currentSecond = (int)elapsedTime;
        // 시간에 따라 서서히 빨라지는 현재 속도 계산
        float currentSpeed = Mathf.Min(maxEnemySpeed, enemySpeed + (elapsedTime * 0.05f));

        // 1. 공통: 10초 주기로 원형 패턴 (난이도와 상관없이 주기적으로 압박)
        if (currentSecond % 10 == 0 && lastPatternMilestone != currentSecond)
        {
            circlePattern.Execute();
            lastPatternMilestone = currentSecond;
            return;
        }

        // 2. 페이즈별 특수 패턴 조합
        switch (phase)
        {
            case 1: 
                // 5초마다 조준탄, 나머지는 일반탄
                if (currentSecond % 5 == 0 && lastPatternMilestone != currentSecond)
                    targetingPattern.Execute();
                else
                    basicPattern.Execute(currentSpeed);
                break;

            case 2: 
                // 15초마다 비 패턴, 일반탄 속도 증가
                if (currentSecond % 15 == 0 && lastPatternMilestone != currentSecond)
                    rainPattern.Execute();
                else
                    basicPattern.Execute(currentSpeed + 1f);
                break;

            case 3: 
                // 20초마다 나선형, 4초마다 조준탄 (주기 단축)
                if (currentSecond % 20 == 0 && lastPatternMilestone != currentSecond)
                    spiralPattern.Execute();
                else if (currentSecond % 4 == 0 && lastPatternMilestone != currentSecond)
                    targetingPattern.Execute();
                else
                    basicPattern.Execute(currentSpeed + 2f);
                break;

            case 4: 
                // 90초 이후 극한 상황: 2.5초마다 랜덤 패턴 난사
                if (currentSecond % 3 == 0 && lastPatternMilestone != currentSecond)
                {
                    ExecuteRandomPattern();
                }
                basicPattern.Execute(currentSpeed + 3f);
                break;
        }
        
        // 패턴 중복 실행 방지 체크
        if (lastPatternMilestone != currentSecond && currentSecond % 1 == 0) 
            lastPatternMilestone = currentSecond;
    }
        

    // 4페이즈용 무작위 패턴 실행 함수
    void ExecuteRandomPattern()
    {
        int rand = Random.Range(0, 3);
        if (rand == 0) targetingPattern.Execute();
        else if (rand == 1) rainPattern.Execute();
        else spiralPattern.Execute();
    }

    // 현재 시간에 따른 생성 간격을 계산 (기본 간격에서 난이도만큼 차감)
    float GetCurrentInterval()
    {
        // elapsedTime에 따라 간격이 줄어들되, minSpawnInterval보다는 작아지지 않게 방어
        float currentInterval = baseSpawnInterval - (elapsedTime * difficultyScale);
        return Mathf.Max(currentInterval, minSpawnInterval);
    }
}
