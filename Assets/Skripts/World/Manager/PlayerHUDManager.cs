// Assets/Skripts/Player/UI/PlayerHUDManager.cs
using Player.Data;
using UnityEngine;

namespace Player.UI
{
    /// <summary>
    /// 플레이어의 CharacterStats를 구독하고,
    /// 하위의 UI 컨트롤러(전문가)들에게 데이터 변경을 알리는 중앙 관리자입니다.
    /// </summary>
    public class PlayerHUDManager : MonoBehaviour
    {
        [Header("플레이어 참조")]
        [Tooltip("플레이어의 CharacterStats 컴포넌트를 할당해야 합니다.")]
        [SerializeField] private CharacterStats _playerStats;

        [Header("UI 전문가 참조")]
        [Tooltip("HP 바를 제어하는 전문가 스크립트입니다.")]
        [SerializeField] private HPBarController _hpBar;

        [Tooltip("Ast 바를 제어하는 전문가 스크립트입니다.")]
        [SerializeField] private AstBarController _astBar;

        [Tooltip("무기 및 버스트 게이지 UI를 제어하는 전문가 스크립트입니다.")]
        [SerializeField] private WeaponDisplayController _weaponDisplay;


        /// <summary>
        /// 컴포넌트가 활성화될 때 CharacterStats의 이벤트를 구독합니다.
        /// </summary>
        private void OnEnable()
        {
            if (_playerStats == null)
            {
                Debug.LogError("PlayerHUDManager에 _playerStats가 할당되지 않았습니다!", this);
                return;
            }

            // 1단계에서 만든 이벤트들을 구독합니다.
            _playerStats.OnHpChanged += HandleHpChanged;
            _playerStats.OnAstChanged += HandleAstChanged;
            _playerStats.OnBustChanged += HandleBustChanged;
            _playerStats.OnWeaponChanged += HandleWeaponChanged;

            // --- 초기값 설정 ---
            // HUD가 켜질 때 현재 스탯으로 즉시 초기화합니다.
            HandleHpChanged(_playerStats.currentHp, _playerStats.maxHp);
            HandleAstChanged(_playerStats.CurrentAst, _playerStats.maxAst);
            HandleBustChanged(_playerStats.CurrentBurst, _playerStats.maxBurst);

            // 무기 초기화 (보조 무기가 없을 수도 있음을 고려)
            WeaponData standbyWeapon = null;
            if (_playerStats.availableWeapons.Count > 1)
            {
                int nextIndex = (0 + 1) % _playerStats.availableWeapons.Count; // 0번이 기본 무기라고 가정
                standbyWeapon = _playerStats.availableWeapons[nextIndex];
            }
            HandleWeaponChanged(_playerStats.CurrentWeaponData, standbyWeapon);
        }

        /// <summary>
        /// 컴포넌트가 비활성화될 때 이벤트를 구독 해제합니다.
        /// </summary>
        private void OnDisable()
        {
            if (_playerStats == null) return;

            // 구독을 해제하여 메모리 누수를 방지합니다.
            _playerStats.OnHpChanged -= HandleHpChanged;
            _playerStats.OnAstChanged -= HandleAstChanged;
            _playerStats.OnBustChanged -= HandleBustChanged;
            _playerStats.OnWeaponChanged -= HandleWeaponChanged;
        }

        // --- 이벤트 핸들러 메서드 ---

        private void HandleHpChanged(float current, float max)
        {
            _hpBar.UpdateDisplay(current, max);
        }

        private void HandleAstChanged(float current, float max)
        {
            _astBar.UpdateDisplay(current, max);
        }

        private void HandleBustChanged(float current, float max)
        {
            _weaponDisplay.UpdateBustGauge(current, max);
        }

        private void HandleWeaponChanged(WeaponData mainWeapon, WeaponData standbyWeapon)
        {
            _weaponDisplay.UpdateWeaponSwap(mainWeapon, standbyWeapon);
        }
    }
}