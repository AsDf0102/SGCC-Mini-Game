using UnityEngine;

public class Boundary : MonoBehaviour
{
    // 탄환(Bug)이 이 영역을 '나갈 때' 호출됩니다.
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
        }
    }
}