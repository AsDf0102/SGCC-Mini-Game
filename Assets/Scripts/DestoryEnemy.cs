using UnityEngine;

public class DestoryEnemy : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, 3f); // 3초 뒤 자동 삭제
    }
}
