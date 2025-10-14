// Assets/Scripts/Monster/Core/SpawnTrigger.cs
using UnityEngine;

namespace Monster
{
    /// <summary>
    /// 플레이어의 진입을 감지하여 연결된 MonsterSpawner를 활성화시키는 트리거입니다.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class SpawnTrigger : MonoBehaviour
    {
        [Header("연결 설정")]
        [Tooltip("이 트리거가 활성화시킬 MonsterSpawner를 할당합니다.")]
        [SerializeField] private MonsterSpawner _spawnerToTrigger;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (_spawnerToTrigger != null)
                {
                    // 연결된 스포너에게 활성화 신호를 보냅니다.
                    _spawnerToTrigger.ActivateSpawner();

                    // 트리거는 한 번만 작동하도록 자신의 게임 오브젝트를 비활성화합니다.
                    gameObject.SetActive(false);
                }
                else
                {
                    Debug.LogWarning("SpawnTrigger에 연결된 MonsterSpawner가 없습니다!", this);
                }
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            // 트리거의 범위를 시각적으로 표시합니다.
            var col = GetComponent<Collider>();
            if (col == null) return;
            
            Gizmos.color = new Color(0, 1, 1, 0.5f); // 청록색
            
            // BoxCollider인 경우 회전을 포함하여 정확한 와이어 큐브를 그립니다.
            if (col is BoxCollider boxCollider)
            {
                // Gizmos.matrix를 사용하면 로컬 좌표계 기준으로 그릴 수 있어 회전이 적용됩니다.
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
            }
            else // 다른 종류의 콜라이더라면 bounds를 기반으로 그립니다 (회전 미적용).
            {
                Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
            }
        }
#endif
    }
}