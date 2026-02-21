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
    public UIManager uiManager;
    public int maxLife = 3;
    private int currentLife;
     

        // Collide Effect
    public float invincibilityDuration = 1.0f; // 무적 및 색상 변경 지속 시간
    private bool isInvincible = false; // 현재 무적 상태인지 확인
    private SpriteRenderer spriteRenderer; // 색상 변경
    private Color originalColor;
    private CameraShake cameraShake;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        cameraShake = Camera.main.GetComponent<CameraShake>();
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
    }
    void FixedUpdate()
    {
        // Move KeyBoard
        //   2. 물리 엔진을 통한 실제 이동 구현
        rigid.linearVelocity = moveInput * moveSpeed;
    }
    // FixedUpdate가 물리 연산을 끝낸 후, 마지막에 호출되어 좌표를 강제로 고정합니다.
    void LateUpdate()
    {
        // 1. 카메라가 현재 보고 있는 월드 좌표의 끝과 끝을 가져옵니다.
        // Viewport (0,0)은 왼쪽 아래 끝, (1,1)은 오른쪽 위 끝입니다.
        Vector3 bottomLeft = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
        Vector3 topRight = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));

        // 2. 캐릭터의 절반 크기(Padding)를 고려합니다. (0.5f는 캐릭터 크기에 맞춰 조절)
        float paddingX = 0.5f; 
        float paddingY = 0.5f;

        // 3. 실시간으로 계산된 경계(min/max)로 위치를 제한합니다.
        float clampedX = Mathf.Clamp(transform.position.x, bottomLeft.x + paddingX, topRight.x - paddingX);
        float clampedY = Mathf.Clamp(transform.position.y, bottomLeft.y + paddingY, topRight.y - paddingY);

        // 4. 최종 좌표 고정
        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }
        
    // Collide Trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Collide Enemy
        if (collision.CompareTag("Enemy") && !isInvincible)
        {
            if (uiManager != null)
            {
                uiManager.PlayHitSound();
                uiManager.UpdateLifeUI(currentLife - 1); // 라이프 감소 및 UI 업데이트
            }
            // Life Down
            currentLife--;
            Destroy(collision.gameObject); // 버그와 부딪히면 해당 버그 제거
        
            if (currentLife > 0)
            {
                // Collide Effect
                cameraShake.TriggerShake(0.2f, 0.2f); // 화면 흔들림 발동 (시간, 강도)
                StartCoroutine(HitReaction());
            }
            else
            {
                Debug.Log("게임 오버!");
                // 이곳에 나중에 Game Over시 Effect 추가
            }
        }
    }

    // Collide Effect
    IEnumerator HitReaction()
    {
        isInvincible = true; // 무적 on
        float timer = 0f;

        while (timer < invincibilityDuration)
        {
            // 알파값(투명도)을 0.2와 1.0 사이에서 빠르게 전환
            float alpha = spriteRenderer.color.a == 1.0f ? 0.2f : 1.0f;
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            // 0.1초 간격으로 깜빡임
            yield return new WaitForSeconds(0.1f);
            timer += 0.1f;
        }

        spriteRenderer.color = originalColor;
        isInvincible = false; // 무적 off
    }
}


