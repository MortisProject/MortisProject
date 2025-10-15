// Assets/Scripts/Player/Core/CharacterStats.cs
using Player.States;
using Player.Data;
using System.Collections.Generic;
using UnityEngine;
using Monster.Data;
using System.Collections;

namespace Player
{
    /// <summary>
    /// 플레이어가 공통적으로 가지는 기본 스탯을 정의합니다.
    /// </summary>
    public class CharacterStats : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerSO _data; // SO 데이터 참조
        [SerializeField] private PlayerStateMachine _stateMachine;
        [SerializeField] private PlayerMotor _motor;

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

        [Header("Guard Stats")]
        [Tooltip("최대 가드 게이지입니다.")]
        public float maxGuardGauge = 100f;
        [Tooltip("현재 가드 게이지입니다.")]
        public float CurrentGuardGauge { get; private set; }

        [Header("Abilities")]
        [Tooltip("현재 더블 점프가 가능한지 여부를 나타냅니다.")]
        public bool CanDoubleJump { get; private set; }

        [Header("Weapon System")]
        [Tooltip("플레이어가 사용할 수 있는 모든 무기 데이터 목록입니다.")]
        public List<WeaponData> availableWeapons;

        [Tooltip("현재 장착하고 있는 무기의 데이터입니다. (런타임에 자동 할당)")]
        public WeaponData CurrentWeaponData { get; private set; }
        private int _currentWeaponIndex = 0;

        private bool _isStunImmune = false; // 현재 경직 면역 상태인지 여부
        //[Header("Combat")]
        //[Tooltip("현재 장착하고 있는 무기의 종류입니다.")]
        //public WeaponType CurrentWeapon { get; private set; }

        /// <summary>
        /// 스크립트 인스턴스가 로드될 때 호출됩니다.
        /// </summary>
        private void Awake()
        {
            // 게임 시작 시 현재 체력을 최대 체력으로 초기화합니다.
            currentHp = maxHp;
            CurrentAst = maxAst;

            // 게임 시작 시 첫 번째 무기로 초기화
            if (availableWeapons != null && availableWeapons.Count > 0)
            {
                EquipWeapon(0);
            }

            CurrentGuardGauge = maxGuardGauge;
            if (_stateMachine == null) _stateMachine = GetComponent<PlayerStateMachine>();
            if (_motor == null) _motor = GetComponent<PlayerMotor>();
        }

        private void Update()
        {
            // 가드 상태가 아닐 때 가드 게이지를 회복합니다.
            if (!(_stateMachine.CurrentState is PlayerGuardState))
            {
                RegenerateGuardGauge();
            }
        }
        /// <summary>
        /// 현재 무기를 지정된 타입으로 변경합니다.
        /// </summary>
        /// <param name="newWeapon">새롭게 장착할 무기 타입</param>
        public void ChangeNextWeapon()
        {
            if (availableWeapons == null || availableWeapons.Count == 0) return;

            // 다음 무기 인덱스를 계산하고, 목록을 순환하도록 합니다.
            int nextIndex = (_currentWeaponIndex + 1) % availableWeapons.Count;
            EquipWeapon(nextIndex);
        }
        
        /// <summary>
        /// 지정된 타입의 무기로 교체합니다.
        /// </summary>
        /// <param name="targetType">교체할 무기의 타입</param>
        public void ChangeWeapon(WeaponType targetType)
        {
            if (availableWeapons == null || availableWeapons.Count == 0) return;

            // 사용 가능한 무기 목록에서 해당 타입을 가진 첫 번째 무기를 찾습니다.
            for (int i = 0; i < availableWeapons.Count; i++)
            {
                if (availableWeapons[i].weaponType == targetType)
                {
                    EquipWeapon(i);
                    return; // 무기를 찾았으면 종료
                }
            }

            Debug.LogWarning($"{targetType} 타입의 무기를 찾을 수 없습니다.");
        }

        /// <summary>
        /// 지정된 인덱스의 무기를 장착합니다.
        /// </summary>
        private void EquipWeapon(int index)
        {
            _currentWeaponIndex = index;
            CurrentWeaponData = availableWeapons[_currentWeaponIndex];
            Debug.Log($"{CurrentWeaponData.weaponType}으로 무기 교체!");

            // TODO: 무기 모델을 바꾸거나, HUD UI를 업데이트하는 로직을 여기서 호출합니다.
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

        /// <summary>
        /// 지정된 양의 데미지를 받고, 공격 타입에 따라 추가 처리를 합니다.
        /// </summary>
        //넉백타입이 제거되지 않은 코드
        //public void TakeDamage(float damage, MonsterSkillData.AttackType attackType, MonsterSkillData.KnockbackType knockbackType)
        public void TakeDamage(float damage, Transform attacker, MonsterSkillData.AttackType attackType)
        {
            // --- 코드 블럭 단위로 제공 (수정된 부분) ---
            // 현재 플레이어의 상태를 확인합니다.
            var currentState = _stateMachine.CurrentState;

            // 1. 회피 상태일 때
            if (currentState is PlayerDodgeState)
            {
                // Yellow 타입 공격이 아니면 데미지만 받고 경직은 무시합니다.
                if (attackType != MonsterSkillData.AttackType.Yellow)
                {
                    currentHp -= damage;
                    Debug.Log($"[회피 중 피격] 플레이어가 {damage}의 피해를 입었습니다!");
                    // TODO: 데미지 UI 표시
                    if (currentHp <= 0) { /* TODO: 사망 처리 */ }
                    return; // 경직 로직을 실행하지 않고 종료
                }
                // Yellow 타입 공격이면 회피에 실패하므로 아래의 기본 피격 로직을 따릅니다.
            }

            // 2. 가드 상태일 때
            if (currentState is PlayerGuardState)
            {
                // Blue 타입 공격이 아니면 가드를 시도합니다.
                if (attackType != MonsterSkillData.AttackType.Blue)
                {
                    float guardCost = damage * 0.5f;
                    if (CurrentGuardGauge >= guardCost)
                    {
                        // 가드 성공
                        CurrentGuardGauge -= guardCost;
                        float reducedDamage = damage * (1 - (_data.guardDamageReduction / 100f));
                        currentHp -= reducedDamage;

                        // TODO: PlayerMotor에 가드 넉백 메서드 추가
                        _motor.ApplyKnockback(attacker.position, _data.guardSuccessKnockbackForce);

                        Debug.Log($"[가드 성공] {reducedDamage}의 감소된 피해를 입고, 가드 게이지 {guardCost} 소모. 현재 가드 게이지: {CurrentGuardGauge}");
                        // TODO: 가드 성공 이펙트(VFX, SFX) 재생
                        if (currentHp <= 0) { /* TODO: 사망 처리 */ }
                        return; // 가드에 성공했으므로 경직 로직을 실행하지 않음
                    }
                }
                // Blue 타입 공격이거나, 가드 게이지가 부족하면 가드에 실패하므로 아래의 기본 피격 로직을 따릅니다.
                Debug.Log($"[가드 실패] {attackType} 공격 또는 가드 게이지 부족!");
            }

            // 3. 경직 면역 상태가 아닐 때 (기본 피격, 회피/가드 실패)
            if (!_isStunImmune)
            {
                currentHp -= damage;
                Debug.Log($"플레이어가 {damage}의 피해를 입었습니다! ({attackType} 공격)");

                if (currentHp <= 0)
                {
                    // TODO: 플레이어 사망 처리 로직
                }
                else
                {
                    // HitState로 강제 전환
                    _motor.ApplyKnockback(attacker.position, _data.hitKnockbackForce);
                    _stateMachine.ForceChangeState(GetComponent<Player>().HitState);
                }
            }
            else
            {
                // 경직 면역 상태에서는 데미지만 받습니다.
                currentHp -= damage;
                Debug.Log($"[경직 면역] 플레이어가 {damage}의 피해를 입었습니다!");
                if (currentHp <= 0) { /* TODO: 사망 처리 */ }
            }
        }

        /// <summary>
        /// 경직 면역 상태를 일정 시간 동안 활성화합니다.
        /// </summary>
        public void StartStunImmunity()
        {
            StartCoroutine(StunImmunityCoroutine());
        }

        private IEnumerator StunImmunityCoroutine()
        {
            _isStunImmune = true;
            yield return new WaitForSeconds(_data.stunImmunityDuration);
            _isStunImmune = false;
        }

        /// <summary>
        /// 가드 게이지를 초당 일정량 회복시킵니다.
        /// </summary>
        private void RegenerateGuardGauge()
        {
            // TODO: PlayerSO에 guardGaugeRegenRate 변수 추가 후 연결
            if (CurrentGuardGauge < maxGuardGauge)
            {
                CurrentGuardGauge += _data.guardGaugeRegenRate * Time.deltaTime;
                CurrentGuardGauge = Mathf.Min(CurrentGuardGauge, maxGuardGauge);
            }
        }
    }
}