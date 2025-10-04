// Assets/Scripts/Player/Core/CharacterStats.cs
using UnityEngine;

namespace Player
{
    /// <summary>
    /// 캐릭터(플레이어, 몬스터 등)가 공통적으로 가지는 기본 스탯을 정의합니다.
    /// </summary>
    public class CharacterStats : MonoBehaviour
    {
        [Header("Core Stats")]
        [Tooltip("최대 체력입니다.")]
        public float maxHp = 120f;

        [Tooltip("현재 체력을 나타냅니다.")]
        public float currentHp;

        // TODO: 방어력 공격력등 플레이어와 몬스터가 공유하는 스텟 추가

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