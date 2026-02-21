using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("UI 요소")]
    public TextMeshProUGUI scoreText; // 점수 표시 텍스트
    public TextMeshProUGUI lifeText;  // 생명 표시 텍스트
    public TextMeshProUGUI timerText; // 남은 시간 표시 텍스트

    [Header("결과창 UI")]
    public GameObject resultPanel;    // 결과 화면 패널
    public TextMeshProUGUI resultTitleText; // [추가] "GAME OVER" 또는 "VICTORY" 제목
    public TextMeshProUGUI finalScoreText; // 최종 점수 텍스트
    public TextMeshProUGUI highScoreText; // 최고 점수 텍스트

    [Header("게임 설정")]
    public float remainingTime = 120f; // 제한 시간 : 2분 (기획서에 따라 120초로 수정)
    private bool isGameActive = true; // 게임 진행 상태 (true: 진행 중, false: 종료)
    private int currentScore = 0;     // 현재 점수 저장용
    private int highScore = 0; // 최고 점수 저장용

    [Header("사운드 설정")]
    [Header("--- 배경음악 설정 ---")]
    public AudioSource bgmSource;      // 배경음악용 (Loop 체크 필수)
    [Header("--- 효과음 설정 ---")]
    public AudioSource sfxSource;      // 효과음용 (PlayOneShot용)

    [Header("음악")]
    public AudioClip gameplayBGM;      // 게임 배경 음악
    public AudioClip gameOverSound;    // 패배 시 음악
    public AudioClip victorySound;     // 승리 시 음악
    public AudioClip hitSound;         // 피격음
    public AudioClip buttonClickSound; // 버튼 클릭 소리

    void Start()
    {
        // PlayerPrefs.DeleteAll(); // 모든 저장 데이터 삭제

        // 1. 저장된 값을 불러옵니다.
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        Debug.Log("불러온 최고 기록: " + highScore); // 시작할 때 로그 확인

        resultPanel.SetActive(false);
        Time.timeScale = 1;

        // 2. 시작할 때 미리 텍스트를 업데이트해 둡니다.
        if (highScoreText != null)
        {
            highScoreText.text = "High Score: " + highScore.ToString();
        }

        // 배경음악 재생 시작
        if (bgmSource != null && gameplayBGM != null)
        {
            bgmSource.clip = gameplayBGM;
            bgmSource.loop = true; // 반복 재생
            bgmSource.Play();
        }
    }

    void Update()
    {
        if (!isGameActive) return; // 게임이 종료되었다면 더 이상 시간을 흐르게 하지 않음

        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime; // 시간 감소
            UpdateTimerUI(); // 타이머 UI 갱신

            // 시간 기반 점수 계산 : 120초에서 남은 시간을 빼서 '생존 시간'을 구하고 점수화 (초당 10점)
            float survivalTime = 120f - remainingTime;
            currentScore = Mathf.FloorToInt(survivalTime * 10);
            scoreText.text = "SCORE: " + currentScore;
        }
        else
        { 
            remainingTime = 0;
            WinGame(); // 120초 버티면 승리!
        }
    }

    // 타이머 텍스트 갱신 함수 (분:초 형식)
    void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        
        // 음수 방지
        if (remainingTime <= 0)
        {
            minutes = 0; 
            seconds = 0;
        }

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // 외부(PlayerController 등)에서 점수를 강제로 올릴 때 사용
    public void UpdateScoreUI(int score)
    {
        // 시간 기반 점수 외에 추가 점수가 필요 없다면 이 함수는 비워두거나 제거
        // 현재는 Update에서 실시간으로 갱신 중입니다.
    }

    // 생명 UI 갱신 (PlayerController에서 호출)
    public void UpdateLifeUI(int currentLife)
    {
        lifeText.text = "LIFE: " + currentLife;

        if (currentLife <= 0)
        {
            GameOver(); // 생명이 0이 되면 게임 오버
        }
    }

    // 피격음 재생
    public void PlayHitSound()
    {
        if (sfxSource != null && hitSound != null)
        {
            sfxSource.PlayOneShot(hitSound);
        }
    }

    // 게임 승리 시 함수
    void WinGame()
    {
        if (!isGameActive) return;
        isGameActive = false;
        Time.timeScale = 0;

        // 배경음악 정지 및 승리 효과음 재생
        // if (bgmSource != null) bgmSource.Stop();
        if (sfxSource != null && victorySound != null)
            sfxSource.PlayOneShot(victorySound);

        if (resultTitleText != null) resultTitleText.text = "VICTORY";

        ShowResult();
    }
    
    // 게임 종료 처리 함수
    void GameOver()
    {
        if (!isGameActive) return;
        isGameActive = false;
        Time.timeScale = 0;

        // 배경음악 멈추고 게임오버 음악 재생
        // if (bgmSource != null) bgmSource.Stop();
        if (sfxSource != null && gameOverSound != null) sfxSource.PlayOneShot(gameOverSound);

        if (resultTitleText != null) resultTitleText.text = "GAME OVER";

        ShowResult();
    }

    // [공통] 결과 화면 표시 및 점수 저장 로직
    void ShowResult()
    {
        // 최고 점수 갱신
        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }

        resultPanel.SetActive(true);

        if (bgmSource != null)
        {
            bgmSource.volume = 0.3f; // 배경음악을 은은하게 낮춤
        }
        
        // 텍스트 정리 (불필요한 별표 ★ 제거하여 폰트 에러 방지)
        if (finalScoreText != null) 
            finalScoreText.text = "Score: " + currentScore.ToString("N0");
            
        if (highScoreText != null) 
            highScoreText.text = "Best Record: " + highScore.ToString("N0");
    }

    // 재시작 버튼 기능 (UI 버튼 OnClick에 연결)
    public void OnRestartButtonClicked()
    {
        if (bgmSource != null) bgmSource.volume = 1.0f; // 볼륨 복구

        if (sfxSource != null && buttonClickSound != null)
            sfxSource.PlayOneShot(buttonClickSound); 
        
        Time.timeScale = 1; // 멈췄던 시간을 다시 흐르게 설정
        Invoke("ExecuteRestart", 0.1f);
    }

    // 실제 재시작 로직을 담은 별도 함수
    void ExecuteRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // 현재 씬을 다시 로드하여 재시작
    }

    // "타이틀로" 버튼
    public void OnTitleButtonClicked()
    {
        if (sfxSource != null && buttonClickSound != null)
            sfxSource.PlayOneShot(buttonClickSound); 

        Time.timeScale = 1f; // 중요: 멈춘 시간 다시 돌리기
        SceneManager.LoadScene("TitleScene"); // 타이틀 씬으로 이동
    }

    // 게임 종료(나가기) 버튼 기능
    public void OnExitButtonClicked()
    {   
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // 유니티 에디터에서 실행 중일 때는 플레이 모드를 중지
        #else
            Application.Quit(); // 실제 빌드된 게임에서는 어플리케이션 종료
        #endif
    }
}
