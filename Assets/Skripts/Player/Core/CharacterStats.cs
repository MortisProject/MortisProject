// Assets/Scripts/Player/Core/CharacterStats.cs
using Player.Data;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// 캐릭터(플레이어, 몬스터 등)가 공통적으로 가지는 기본 스탯을 정의합니다.
    /// </summary>
    public class CharacterStats : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerSO _data; // SO 데이터 참조

        [Header("Core Stats")]
        [Tooltip("최대 체력입니다.")]
        public float maxHp = 120f;
        [Tooltip("현재 체력을 나타냅니다.")]
        public float currentHp;

        [Tooltip("최대 아스트(Ast) 양입니다.")]
        public float maxAst = 10f;
        [Tooltip("현재 아스트(Ast) 양입니다.")]
        public float CurrentAst;

        [Tooltip("최대 버스트(Bust) 양입니다.")]
        public float maxBust = 10f;
        [Tooltip("현재 아스트(Bust) 양입니다.")]
        public float CurrentBust;

        [Tooltip("현재 공격력")]
        public float attackValue = 10;
        [Tooltip("현재 방어력")]
        public float defenceValue = 10;
        [Tooltip("현재 공격 속도")]
        public float attackSpeed = 1;


        // TODO: 방어력 공격력등 플레이어와 몬스터가 공유하는 스텟 추가
        [Header("Abilities")]
        [Tooltip("현재 더블 점프가 가능한지 여부를 나타냅니다.")]
        public bool CanDoubleJump { get; private set; }

        [Header("Combat")]
        [Tooltip("현재 장착하고 있는 무기의 종류입니다.")]
        public WeaponType CurrentWeapon { get; private set; }

        /// <summary>
        /// 스크립트 인스턴스가 로드될 때 호출됩니다.
        /// </summary>
        private void Awake()
        {
            // 게임 시작 시 현재 체력을 최대 체력으로 초기화합니다.
            currentHp = maxHp;
            CurrentAst = maxAst;
        }

        /// <summary>
        /// 현재 무기를 지정된 타입으로 변경합니다.
        /// </summary>
        /// <param name="newWeapon">새롭게 장착할 무기 타입</param>
        public void ChangeWeapon(WeaponType newWeapon)
        {
            CurrentWeapon = newWeapon;
            // TODO: 무기 교체 시 시각 효과(VFX)나 음향 효과(SFX)를 여기서 재생할 수 있습니다.
        }

        /// <summary>
        /// 더블 점프 기회를 사용했음을 처리합니다.
        /// </summary>
        public void UseDoubleJump()
        {
            CanDoubleJump = false;
        }

        /// <summary>
        /// 더블 점프 기회를 초기화(재충전)합니다.
        /// </summary>
        public void ResetDoubleJump()
        {
            CanDoubleJump = true;
        }

        /// <summary>
        /// 지정된 양의 아스트를 소모합니다. 성공 시 true, 실패 시 false를 반환합니다.
        /// </summary>
        public bool ConsumeAst(float amount)
        {
            if (CurrentAst >= amount)
            {
                CurrentAst -= amount;
                // TODO: UI 업데이트 이벤트 호출
                return true;
            }
            return false;
        }

        /// <summary>
        /// 지정된 양의 아스트를 획득합니다.
        /// </summary>
        public void AddAst(float amount)
        {
            CurrentAst = Mathf.Min(CurrentAst + amount, maxAst);
            // TODO: UI 업데이트 이벤트 호출
        }
    }
}