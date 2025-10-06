using UnityEngine;

public class AutoDestroyEffect : MonoBehaviour
{
    [Tooltip("이 시간이 지나면 오브젝트를 파괴합니다.")]
    public float lifetime = 1f;

    void Start()
    {
        // lifetime 초 후에 이 게임 오브젝트를 파괴하도록 예약합니다.
        Destroy(gameObject, lifetime);
    }
}