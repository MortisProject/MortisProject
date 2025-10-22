// Assets/Scripts/Player/Core/CharacterStats.cs
using Player.States;
using Player.Data;
using System.Collections.Generic;
using UnityEngine;
using Monster.Data;
using System.Collections;
using Player.Animation;
using World.Manager;
using System;

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
        [SerializeField] private PlayerAnimationController _animController;

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
        public float maxBurst = 10f;
        [Tooltip("현재 아스트(Bust) 양입니다.")]
        public float CurrentBurst;

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

        [Tooltip("실제 무기 모델 GameObject 리스트입니다. availableWeapons 리스트와 순서 및 개수가 반드시 일치해야 합니다.")]
        public List<GameObject> weaponObjects;

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
            DeactivateAllWeaponModels();

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

            if (availableWeapons.Count > 1)
            {
                int nextIndex = (_currentWeaponIndex + 1) % availableWeapons.Count;
                WeaponData standbyWeapon = availableWeapons[nextIndex];

                OnWeaponChanged?.Invoke(CurrentWeaponData, standbyWeapon);
            }
            else
            {
                OnWeaponChanged?.Invoke(CurrentWeaponData, null); // 보조 무기가 없음
            }
        }

        /// <summary>
        /// 현재 장착된 무기 모델을 활성화하고, 나머지는 비활성화합니다.
        /// </summary>
        public void ActivateCurrentWeaponModel()
        {
            // 리스트 개수가 맞는지 안전하게 확인합니다.
            if (weaponObjects == null || weaponObjects.Count != availableWeapons.Count)
            {
                Debug.LogError("WeaponObjects와 AvailableWeapons 리스트의 개수가 일치하지 않습니다!");
                return;
            }

            for (int i = 0; i < weaponObjects.Count; i++)
            {
                // 현재 무기 인덱스와 일치하는 모델만 활성화합니다.
                if (weaponObjects[i] != null)
                {
                    weaponObjects[i].SetActive(i == _currentWeaponIndex);
                }
            }
        }

        /// <summary>
        /// 모든 무기 모델을 비활성화합니다.
        /// </summary>
        public void DeactivateAllWeaponModels()
        {
            if (weaponObjects == null) return;

            foreach (var weaponObj in weaponObjects)
            {
                if (weaponObj != null)
                {
                    weaponObj.SetActive(false);
                }
            }
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
                OnAstChanged?.Invoke(CurrentAst, maxAst);
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
            OnAstChanged?.Invoke(CurrentAst, maxAst);
        }

        /// <summary>
        /// 지정된 양의 버스트 게이지를 소모합니다. 성공 시 true, 실패 시 false를 반환합니다.
        /// </summary>
        public bool ConsumeBust(float amount)
        {
            if (CurrentBurst >= amount)
            {
                CurrentBurst -= amount;
                OnBustChanged?.Invoke(CurrentBurst, maxBurst); // UI 업데이트 이벤트 호출
                return true;
            }
            return false;
        }

        /// <summary>
        /// 지정된 양의 버스트 게이지를 획득합니다.
        /// </summary>
        public void AddBust(float amount)
        {
            CurrentBurst = Mathf.Min(CurrentBurst + amount, maxBurst);
            OnBustChanged?.Invoke(CurrentBurst, maxBurst); // UI 업데이트 이벤트 호출
        }

        /// <summary>
        /// 지정된 양의 데미지를 받고, 공격 타입에 따라 추가 처리를 합니다.
        /// </summary>
        //넉백타입이 제거되지 않은 코드
        //public void TakeDamage(float damage, MonsterSkillData.AttackType attackType, MonsterSkillData.KnockbackType knockbackType)
        public void TakeDamage(float damage, Transform attacker, MonsterSkillData.AttackType attackType)
        {
            // 현재 플레이어의 상태를 확인합니다.
            var currentState = _stateMachine.CurrentState;

            // 1. 회피 상태일 때
            if (currentState is PlayerDodgeState dodgeState)
            {
                // 회피 판정 시간 내에 피격되었는지 확인합니다.
                if (dodgeState.TimeSinceEntered <= _data.perfectDodgeWindow)
                {
                    // Yellow 타입 공격은 퍼펙트 회피를 무시하고 그대로 피격됩니다.
                    if (attackType == MonsterSkillData.AttackType.Yellow)
                    {
                        Debug.Log("퍼펙트 회피 실패! (Yellow 타입 공격)");
                        // 여기서 특별한 처리를 하지 않으면, 코드는 아래의 일반 피격 로직으로 넘어갑니다.
                    }
                    else // Yellow 타입이 아니면 퍼펙트 회피 성공!
                    {
                        Debug.Log("퍼펙트 회피 성공!");

                        _animController.PlayDodgePerfect();
                        _motor.Jump(2);
                        // 불릿타임을 발동시킵니다.
                        BulletTimeManager.Instance.StartBulletTime(_data.perfectDodgeBulletTimeDuration);

                        // TODO: 여기에 퍼펙트 회피 성공 시의 시각/청각 효과(VFX, SFX) 재생 로직을 추가할 수 있습니다.

                        // 데미지를 받지 않고, 상태도 변하지 않도록 여기서 메서드를 즉시 종료합니다.
                        return;
                    }
                }
                // 일반 회피 중 피격 로직 (Yellow가 아닐 때)
                if (attackType != MonsterSkillData.AttackType.Yellow)
                {
                    currentHp -= damage;
                    OnHpChanged?.Invoke(currentHp, maxHp);
                    Debug.Log($"[회피 중 피격] 플레이어가 {damage}의 피해를 입었습니다!");
                    if (currentHp <= 0) { /* TODO: 사망 처리 */ }
                    return; // 경직 없이 데미지만 받고 종료
                }
            }

            // 2. 가드 상태일 때
            if (currentState is PlayerGuardState guardState)
            {
                if (guardState.TimeSinceEntered <= _data.perfectGuardWindow)
                {
                    // Blue 타입 공격을 받으면 퍼펙트 가드에 실패하고 즉시 피격됩니다.
                    if (attackType == MonsterSkillData.AttackType.Blue)
                    {
                        Debug.Log("퍼펙트 가드 실패! (Blue 타입 공격)");
                        // 아래의 일반 피격 로직으로 넘어갑니다.
                    }
                    else // Blue 타입이 아니면 퍼펙트 가드 성공!
                    {
                        Debug.Log("퍼펙트 가드 성공!");

                        // 불릿타임을 발동시킵니다.
                        BulletTimeManager.Instance.StartBulletTime(_data.perfectGuardBulletTimeDuration);

                        // TODO: 여기에 퍼펙트 가드 성공 시의 시각/청각 효과(VFX, SFX) 재생 로직을 추가할 수 있습니다.
                        // (예: 화면에 스파크 효과, "Clang!" 사운드 등)

                        // 데미지, 게이지 소모, 넉백 없이 즉시 메서드를 종료합니다.
                        return;
                    }
                }
                    // Blue 타입 공격이 아니면 가드를 시도합니다.
                if (attackType != MonsterSkillData.AttackType.Blue)
                {
                    float guardCost = damage * 0.5f;
                    if (CurrentGuardGauge >= guardCost)
                    {
                        // 가드 성공
                        _animController.PlayGuardHit();
                        CurrentGuardGauge -= guardCost;
                        float reducedDamage = damage * (1 - (_data.guardDamageReduction / 100f));
                        currentHp -= reducedDamage;
                        OnHpChanged?.Invoke(currentHp, maxHp);

                        // TODO: PlayerMotor에 가드 넉백 메서드 추가
                        _motor.ApplyKnockback(attacker.position, _data.guardSuccessKnockbackForce);

                        Debug.Log($"[가드 성공] {reducedDamage}의 감소된 피해를 입고, 가드 게이지 {guardCost} 소모. 현재 가드 게이지: {CurrentGuardGauge}");
                        // TODO: 가드 성공 이펙트(VFX, SFX) 재생
                        if (currentHp <= 0) { /* TODO: 사망 처리 */ }
                        return; // 가드에 성공했으므로 경직 로직을 실행하지 않음
                    }
                }
                // Blue 타입 공격이거나, 가드 게이지가 부족하면 가드 브레이크 상태로 전환
                Debug.Log($"[가드 브레이크!] {attackType} 공격 또는 가드 게이지 부족!");
                // 데미지를 받고, 넉백을 적용한 후, GuardBreakState로 전환합니다.
                currentHp -= damage;
                OnHpChanged?.Invoke(currentHp, maxHp);
                _animController.PlayGuardBreak();
                _motor.ApplyKnockback(attacker.position, _data.hitKnockbackForce);
                _stateMachine.ForceChangeState(GetComponent<Player>().GuardBreakState);
                return;
            }

            // 3. 경직 면역 상태가 아닐 때 (기본 피격, 회피/가드 실패)
            if (!_isStunImmune)
            {
                currentHp -= damage;
                OnHpChanged?.Invoke(currentHp, maxHp);
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
                OnHpChanged?.Invoke(currentHp, maxHp);
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

        /// <summary>
        /// 버스트 게이지가 변경될 때 UI 등에 알리기 위한 이벤트입니다. (현재값, 최대값)
        /// </summary>
        public event Action<float, float> OnBustChanged;
        /// <summary>
        /// 체력이 변경될 때 UI 등에 알리기 위한 이벤트입니다. (현재값, 최대값)
        /// </summary>
        public event Action<float, float> OnHpChanged;

        /// <summary>
        /// 아스트가 변경될 때 UI 등에 알리기 위한 이벤트입니다. (현재값, 최대값)
        /// </summary>
        public event Action<float, float> OnAstChanged;

        /// <summary>
        /// 무기가 교체될 때 UI 등에 알리기 위한 이벤트입니다. (새 주무기 데이터, 새 보조무기 데이터)
        /// </summary>
        public event Action<WeaponData, WeaponData> OnWeaponChanged;
    }
}