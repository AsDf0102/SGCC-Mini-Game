using UnityEngine;
using UnityEngine.SceneManagement; // 씬 전환을 위해 필수!

public class TitleMenu : MonoBehaviour
{
    [Header("오디오")]
    public AudioSource audioSource; // 효과음을 재생할 오디오 소스
    public AudioClip clickSound;    // 버튼 클릭 소리 파일

    public void StartGame()
    {
        // 버튼을 누르자마자 소리 재생
        if (audioSource != null && clickSound != null)
            audioSource.PlayOneShot(clickSound, 2f);

        Invoke("LoadGameScene", 0.2f);
    }

    void LoadGameScene()
    {
        SceneManager.LoadScene("PlayingGame"); // 본인의 게임 씬 이름 확인!
    }
}