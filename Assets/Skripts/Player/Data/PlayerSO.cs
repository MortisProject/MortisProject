// Assets/Scripts/Player/Data/PlayerSO.cs
using UnityEngine;

namespace Player.Data
{
    // CreateAssetMenu를 사용하면 유니티 에디터의 Create 메뉴에서 이 에셋을 쉽게 생성할 수 있습니다.
    [CreateAssetMenu(fileName = "NewPlayerData", menuName = "Data/Player Data")]
    public class PlayerSO : ScriptableObject
    {
        [Header("Movement Stats")]
        [Tooltip("걷기 속도입니다.")]
        public float walkSpeed = 5f;
        [Tooltip("달리기 속도입니다.")]
        public float runSpeed = 9f;

        [Header("Acrobatic Stats")]
        [Tooltip("점프 시 도달할 목표 높이입니다.")]
        public float jumpHeight = 2.0f;

        [Header("Wire Action Stats")]
        [Tooltip("와이어의 최대 사거리 및 스윙 반경입니다.")]
        public float wireMaxLength = 30f;

        [Tooltip("화면 중앙에서 와이어 포인트를 탐색할 반경입니다. (0~1 사이 값)")]
        [Range(0f, 1f)]
        public float wireAimSearchRadius = 0.5f;

        [Tooltip("와이어 발사 후 대쉬가 시작되기까지의 대기 시간입니다.")]
        public float wireLaunchDelay = 0.3f;

        [Tooltip("와이어 이동 시작 시의 초기 대쉬 속도입니다.")]
        public float wireLaunchSpeed = 25f;

        [Tooltip("공중에서 사용하는 추가 대쉬의 속도입니다.")]
        public float wireAirDashSpeed = 30f;

        [Tooltip("와이어 이동 중 A/D키로 방향을 트는 속도입니다.")]
        public float wireTurnSpeed = 180f; // 초당 180도

        // TODO: 닷지 지속시간, 와이어 속도 등 플레이어 전용 데이터를 여기에 계속 추가합니다.
    }
}