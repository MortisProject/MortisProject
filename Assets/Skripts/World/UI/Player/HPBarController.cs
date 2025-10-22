// Assets/Skripts/Player/UI/HPBarController.cs
using UnityEngine;
using UnityEngine.UI; // Image 컴포넌트를 사용하기 위해 필수

namespace Player.UI
{
    /// <summary>
    /// HP 바의 시각적 표현(채우기, 이펙트)을 전담하는 전문가 스크립트입니다.
    /// PlayerHUDManager로부터 명령을 받습니다.
    /// </summary>
    [RequireComponent(typeof(Image))] // 이 스크립트는 Image 컴포넌트가 필수입니다.
    public class HPBarController : MonoBehaviour
    {
        [Header("시각 효과 설정")]
        [Tooltip("HP가 변경될 때 바(Bar)가 채워지는 속도입니다.")]
        [SerializeField] private float _fillSpeed = 3f;

        private Image _hpBarImage; // 실제 HP를 표시할 Filled 이미지
        private float _targetFill; // 목표로 하는 fillAmount 값 (0.0 ~ 1.0)

        private void Awake()
        {
            // 자신의 Image 컴포넌트를 가져옵니다.
            _hpBarImage = GetComponent<Image>();
            if (_hpBarImage.type != Image.Type.Filled)
            {
                Debug.LogWarning("HPBarController가 붙어있는 Image가 'Filled' 타입이 아닙니다!", this);
            }

            // 게임 시작 시 100%로 설정
            _targetFill = 1f;
            _hpBarImage.fillAmount = 1f;
        }

        private void Update()
        {
            // 현재 fillAmount 값을 목표(_targetFill) 값까지 부드럽게 이동시킵니다.
            // 이것이 "변경 효과"의 핵심입니다.
            if (_hpBarImage.fillAmount != _targetFill)
            {
                _hpBarImage.fillAmount = Mathf.Lerp(_hpBarImage.fillAmount, _targetFill, Time.deltaTime * _fillSpeed);
            }
        }

        /// <summary>
        /// (PlayerHUDManager가 호출할) HP 표시를 업데이트합니다.
        /// </summary>
        /// <param name="current">현재 HP</param>
        /// <param name="max">최대 HP</param>
        public void UpdateDisplay(float current, float max)
        {
            // 목표 fillAmount 값을 0과 1 사이로 정규화하여 설정합니다.
            _targetFill = Mathf.Clamp01(current / max);

            // TODO: 여기에 피격 시 이펙트(Animator 트리거, 파티클 재생 등) 로직을 추가하면
            // PlayerHUDManager는 이 사실을 전혀 몰라도 됩니다.
        }
    }
}