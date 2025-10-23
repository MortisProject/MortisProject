// Assets/Skripts/World/TeleportTrigger.cs
using Player;
using UnityEngine;
using World.Manager; // FadeManager를 사용하기 위해 추가

namespace World
{
    /// <summary>
    /// 플레이어의 진입을 감지하여 FadeManager에게 텔레포트를 요청하는 트리거입니다.
    /// (예: 낙사 존)
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class TeleportTrigger : MonoBehaviour
    {
        [Header("텔레포트 설정")]
        [Tooltip("플레이어가 텔레포트될 목적지(Transform)입니다.")]
        [SerializeField] private Transform _teleportDestination;

        // 한 번의 추락으로 중복 요청되는 것을 방지하는 플래그
        private bool _isTriggered = false;

        private void Awake()
        {
            // 이 스크립트는 반드시 트리거로 작동해야 함
            GetComponent<Collider>().isTrigger = true;
        }

        /// <summary>
        /// 트리거 영역에 누군가 들어왔을 때 호출됩니다.
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            // 이미 요청 중이거나 목적지가 설정되지 않았다면 무시
            if (_isTriggered || _teleportDestination == null) return;

            // 들어온 대상이 "Player" 태그인지 확인
            if (other.CompareTag("Player"))
            {
                // Player 컴포넌트를 가져오는데 성공했다면
                if (other.TryGetComponent<Player.Player>(out Player.Player player))
                {
                    Debug.Log($"플레이어 낙사 감지! {_teleportDestination.name} 위치로 이동을 요청합니다.");
                    _isTriggered = true; // 요청 잠금

                    // FadeManager에게 연출 및 텔레포트 실행을 '요청'
                    FadeManager.Instance.StartFadeAndTeleport(player, _teleportDestination.position);
                }
            }
        }

        /// <summary>
        /// 트리거 영역에서 누군가 빠져나갔을 때 호출됩니다.
        /// </summary>
        private void OnTriggerExit(Collider other)
        {
            // 플레이어가 (텔레포트 등으로) 영역을 벗어났다면, 다시 트리거가 작동할 수 있도록 플래그를 해제
            if (other.CompareTag("Player"))
            {
                _isTriggered = false;
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// 씬 뷰에서 텔레포트 경로를 시각적으로 표시합니다. (디자인 편의용)
        /// </summary>
        private void OnDrawGizmos()
        {
            if (_teleportDestination != null)
            {
                Gizmos.color = Color.cyan; // 청록색
                // 트리거 위치에서 목적지까지 선을 그립니다.
                Gizmos.DrawLine(transform.position, _teleportDestination.position);
                // 목적지 위치에 구체를 그립니다.
                Gizmos.DrawWireSphere(_teleportDestination.position, 1f);
                Gizmos.DrawIcon(_teleportDestination.position + Vector3.up * 1.5f, "d_AvatarMask On Icon", true);
            }

            // 트리거 자체의 범위도 표시 (SpawnTrigger.cs 참조)
            var col = GetComponent<Collider>();
            if (col == null) return;

            Gizmos.color = new Color(1, 0, 0, 0.3f); // 반투명 빨간색
            if (col is BoxCollider boxCollider)
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawCube(boxCollider.center, boxCollider.size);
            }
        }
#endif
    }
}