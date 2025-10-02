// Assets/Scripts/Player/Core/CharacterStats.cs
using UnityEngine;

namespace Player
{
    /// <summary>
    /// 플레이어 캐릭터의 모든 수치 데이터를 정의하고 관리합니다.
    /// 다른 모든 컴포넌트들이 이 스크립트를 참조하여 필요한 능력치 값을 가져갑니다.
    /// </summary>
    public class CharacterStats : MonoBehaviour
    {
        [Header("Core Stats")]
        [Tooltip("최대 체력입니다.")]
        public float maxHp = 120f;

        [Tooltip("현재 체력을 나타냅니다.")]
        public float currentHp;

        [Header("Movement Stats")]
        [Tooltip("걷기 속도입니다.")]
        public float walkSpeed = 5f;
        [Tooltip("달리기 속도입니다.")]
        public float runSpeed = 9f;

        [Header("Acrobatic Stats")]
        [Tooltip("점프 시 도달할 목표 높이입니다.")]
        public float jumpHeight = 2.0f;

        // TODO: 닷지, 공격 등 새로운 상태가 추가될 때 필요한 스탯들을 여기에 추가합니다.
        // 예: public float dodgeDuration = 0.5f;
        // 예: public float attackDamage = 20f;

        /// <summary>
        /// 스크립트 인스턴스가 로드될 때 호출됩니다.
        /// </summary>
        private void Awake()
        {
            // 게임 시작 시 현재 체력을 최대 체력으로 초기화합니다.
            currentHp = maxHp;
        }
    }
}