using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI lifeText;
    public TextMeshProUGUI timerText;
    public float remainingTime = 3f; // 제한 시간 : 2분(현재 Debug 위해 3초로 임시 저장)
    private bool isGameActive = true; // 게임이 진행 중인지 확인

    // Result Scene UI
    public GameObject resultPanel;
    public TextMeshProUGUI finalScoreText;
    private int currentScore = 0;

    void Update()
    {
        if (!isGameActive) return; // 게임이 종료되었다면 아래 로직 실행 X

        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            UpdateTimerUI();
        }
        else
        {
            remainingTime = 0;
            // Debug.Log("시간 종료! 게임 끝."); // 나중에 결과창 연결
            GameOver(); // 시간 종료 시 GameOver 함수 호출
        }
    }
    void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        
        // remainingTime이 음수일 때 방지
        if (remainingTime <= 0)
        {
            minutes = 0; 
            seconds = 0;
        }

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    public void UpdateScoreUI(int score)
    {
        scoreText.text = "SCORE: " + score;
        this.currentScore = score; // UIManager에 현재 스코어 저장 -> GameOver에 사용
    }
    public void UpdateLifeUI(int currentLife)
    {
        lifeText.text = "LIFE: "+currentLife;

        if (currentLife <= 0)
        {
            GameOver();
        }
    }
    void GameOver()
    {
        isGameActive = false;
        Debug.Log("2분 경과! 게임 종료.");

        // 게임을 일시정지 시키는 방법 : 물리 연산 중단
        Time.timeScale = 0;

        // Result Scene UI
        resultPanel.SetActive(true);
        finalScoreText.text="Final Score: "+currentScore;
        Debug.Log("게임 종료 및 결과창 출력");
    }

    // Restart Button
    public void OnRestartButtonClicked()
    {
        Time.timeScale = 1; // 멈췄던 시간 다시 흐르게
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // 현재 씬 재시작
    }
}
