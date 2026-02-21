using System.Collections;
using UnityEngine;

public class CirclePattern : MonoBehaviour
{
    public GameObject bugPrefab;
    public GameObject warningCirclePrefab; // 원형 프리팹 연결
    public int bulletCount = 20;
    public float bulletSpeed = 10f;

    public void Execute()
    {
        StartCoroutine(CircleWithWarning());
    }

    IEnumerator CircleWithWarning()
    {
        // 1. 경고 원 생성
        GameObject warning = Instantiate(warningCirclePrefab, transform.position, Quaternion.identity);
        warning.transform.localScale = Vector3.zero;

        // 2. 경고 연출 (원이 점점 커짐)
        float timer = 0;
        float duration = 0.5f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float scale = Mathf.Lerp(0, 3f, timer / duration); // 0에서 3까지 커짐
            warning.transform.localScale = new Vector3(scale, scale, 1);
            yield return null;
        }

        Destroy(warning);

        // 3. 실제 발사 로직
        float angleStep = 360f / bulletCount;
        float angle = 0f;
        for (int i = 0; i < bulletCount; i++)
        {
            float dirX = transform.position.x + Mathf.Cos(angle * Mathf.Deg2Rad);
            float dirY = transform.position.y + Mathf.Sin(angle * Mathf.Deg2Rad);
            Vector2 moveDir = (new Vector2(dirX, dirY) - (Vector2)transform.position).normalized;

            GameObject go = Instantiate(bugPrefab, transform.position, Quaternion.identity);
            go.GetComponent<Rigidbody2D>().linearVelocity = moveDir * bulletSpeed;
            angle += angleStep;
        }
    }
}