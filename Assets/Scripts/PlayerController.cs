using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Move KeyBoard
    public float moveSpeed = 1000f;
    private Vector2 moveInput;
    private Rigidbody2D rigid;

    // Collide Bugs
        // Life System
    public int maxLife = 3;
    private int currentLife;

        // Collide Effect
    public float invincibilityDuration = 1.0f; // 무적 및 색상 변경 지속 시간
    private bool isInvincible = false; // 현재 무적 상태인지 확인
    private SpriteRenderer spriteRenderer; // 색상 변경
    private Color originalColor;

    // Collide Item : Score
    public int score = 0;
    public UIManager uiManager; 

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }
    void Start()
    {
        currentLife = maxLife;
    }
    void Update()
    {
        // Move KeyBoard
        //   1. 키보드 입력받기
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput.Normalize(); // 대각선 이동 시 속도 증가 방지

        // Move Restrict : 화면 밖 이동 제한
        float xBound = 8.5f;
        float yBound = 4.5f;

        float clampedX = Math.Clamp(transform.position.x, -xBound, xBound);
        float clampedY = Math.Clamp(transform.position.y, -yBound, yBound);

        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }
    void FixedUpdate()
    {
        // Move KeyBoard
        //   2. 물리 엔진을 통한 실제 이동 구현
        rigid.linearVelocity = moveInput * moveSpeed;
    }
    
    // Collide Trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Collide Bugs
        if (collision.CompareTag("Bug") && !isInvincible)
        {
            // Life Down
            currentLife--;
            //   Debug.Log("충돌 발생! 남은 라이프 : " + currentLife);
            //   Update UI 
            if (uiManager != null)
            {
                uiManager.UpdateLifeUI(currentLife);
            }

            Destroy(collision.gameObject); // 버그와 부딪히면 해당 버그 제거
        
            if (currentLife > 0)
            {
                // Collide Effect
                StartCoroutine(HitReaction());
            }
            else
            {
                Debug.Log("게임 오버!");
                // 이곳에 나중에 Game Over UI 추가
            }
        }
        // Collide Items : score
        else if (collision.CompareTag("Item"))
        {
            score += 100;

            if (uiManager != null)
            {
                uiManager.UpdateScoreUI(score);
            }

            Debug.Log("아이템 획득! 현재 점수 : " + score);
            Destroy(collision.gameObject);
        }
    }

    // Collide Effect
    IEnumerator HitReaction()
    {
        isInvincible = true; // 무적 on
        spriteRenderer.color = Color.blue;

        yield return new WaitForSeconds(invincibilityDuration); // 1초 대기

        spriteRenderer.color = originalColor;
        isInvincible = false; // 무적 off
    }
}


