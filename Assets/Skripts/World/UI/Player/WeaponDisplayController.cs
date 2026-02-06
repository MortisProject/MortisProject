// Assets/Skripts/Player/UI/WeaponDisplayController.cs
using Player.Data;
using System.Collections.Generic; // List를 사용하기 위해 추가
using UnityEngine;
using UnityEngine.UI;

namespace Player.UI
{
    /// <summary>
    /// 무기 아이콘 교체 애니메이션, 버스트 게이지 표시를 전담하는 '전문가' 스크립트입니다.
    /// PlayerHUDManager로부터 명령을 받습니다.
    /// </summary>
    public class WeaponDisplayController : MonoBehaviour
    {
        // --- 헬퍼 클래스 정의 ---
        // (하나의 무기 UI 세트를 묶어서 관리하기 위함)
        [System.Serializable]
        public class WeaponIconSlot
        {
            [Tooltip("이 슬롯이 담당할 무기 타입입니다. (예: Whip)")]
            public WeaponType weaponType; // WeaponData의 Enum과 일치

            [Tooltip("이 무기 슬롯의 Animator입니다. (Weapon_A 또는 Weapon_B의 Animator)")]
            public Animator slotAnimator; // 'BecomeMain', 'BecomeStandby', 'IsBurstReady' 파라미터 필요

            [Tooltip("버스트 게이지를 표시할 Filled 이미지입니다.")]
            public Image bustGaugeBackground; // 0% ~ 100% 채워질 이미지

            // 이 슬롯의 게이지가 부드럽게 차오르도록 내부적으로 값을 저장
            [HideInInspector] public float targetBustFill;
        }
        // --- 헬퍼 클래스 정의 끝 ---


        [Header("시각 효과 설정")]
        [Tooltip("버스트 게이지가 채워지는 속도입니다.")]
        [SerializeField] private float _bustFillSpeed = 2f;

        [Header("슬롯 참조")]
        [Tooltip("관리할 모든 무기 아이콘 슬롯을 등록합니다.")]
        [SerializeField] private List<WeaponIconSlot> _weaponSlots;

        // 현재 어떤 무기가 '메인'인지 기억
        private WeaponType _currentMainWeaponType;


        private void Update()
        {
            // --- 버스트 게이지 부드럽게 채우기 (Lerp) ---
            // 모든 슬롯을 순회하며 targetFill 값으로 부드럽게 변경
            foreach (var slot in _weaponSlots)
            {
                if (slot.bustGaugeBackground.fillAmount != slot.targetBustFill)
                {
                    slot.bustGaugeBackground.fillAmount = Mathf.Lerp(
                        slot.bustGaugeBackground.fillAmount,
                        slot.targetBustFill,
                        Time.deltaTime * _bustFillSpeed
                    );
                }
            }
        }

        /// <summary>
        /// (PlayerHUDManager가 호출) 버스트 게이지 표시를 업데이트합니다.
        /// </summary>
        public void UpdateBustGauge(float current, float max)
        {
            float targetFill = Mathf.Clamp01(current / max);

            // 모든 슬롯을 순회
            foreach (var slot in _weaponSlots)
            {
                // 현재 '메인' 무기 슬롯만 게이지를 채웁니다.
                if (slot.weaponType == _currentMainWeaponType)
                {
                    slot.targetBustFill = targetFill;

                    // 100%가 되면 "버스트 ON" 애니메이션(보라색 이펙트)을 켭니다.
                    slot.slotAnimator.SetBool("IsBurstReady", targetFill >= 1f);
                }
                else // 보조 무기 슬롯
                {
                    slot.targetBustFill = 0f; // 보조 무기는 항상 0%
                    slot.slotAnimator.SetBool("IsBurstReady", false);
                }
            }
        }

        /// <summary>
        /// (PlayerHUDManager가 호출) 무기 교체 애니메이션을 실행합니다.
        /// </summary>
        public void UpdateWeaponSwap(WeaponData mainWeapon, WeaponData standbyWeapon)
        {
            if (mainWeapon == null || standbyWeapon == null) return;

            // 현재 메인 무기가 무엇인지 기억
            _currentMainWeaponType = mainWeapon.weaponType;

            // 모든 슬롯을 순회
            foreach (var slot in _weaponSlots)
            {
                // 메인 무기가 된 슬롯
                if (slot.weaponType == mainWeapon.weaponType)
                {
                    // "Standby" 트리거가 혹시 켜져 있다면 강제로 끈다.
                    slot.slotAnimator.ResetTrigger("BecomeStandby");
                    // "Main" 트리거를 켠다.
                    slot.slotAnimator.SetTrigger("BecomeMain");
                }
                // 보조 무기가 된 슬롯
                else if (slot.weaponType == standbyWeapon.weaponType)
                {
                    // "Main" 트리거가 혹시 켜져 있다면 강제로 끈다.
                    slot.slotAnimator.ResetTrigger("BecomeMain");
                    // "Standby" 트리거를 켠다.
                    slot.slotAnimator.SetTrigger("BecomeStandby");

                    // 보조 무기로 전환될 땐, 게이지와 이펙트를 즉시 끕니다.
                    slot.targetBustFill = 0f;
                    slot.bustGaugeBackground.fillAmount = 0f;
                    slot.slotAnimator.SetBool("IsBurstReady", false);
                }
            }
        }
    }
}