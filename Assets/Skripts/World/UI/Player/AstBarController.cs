// Assets/Skripts/Player/UI/AstBarController.cs
using UnityEngine;
using UnityEngine.UI; // Image 컴포넌트를 사용하기 위해 필수

namespace Player.UI
{
    /// <summary>
    /// Ast 바의 시각적 표현(채우기)을 전담하는 전문가 스크립트입니다.
    /// PlayerHUDManager로부터 명령을 받습니다.
    /// </summary>
    [RequireComponent(typeof(Image))] // 이 스크립트는 Image 컴포넌트가 필수입니다.
    public class AstBarController : MonoBehaviour
    {
        [Header("시각 효과 설정")]
        [Tooltip("Ast가 변경될 때 바(Bar)가 채워지는 속도입니다.")]
        [SerializeField] private float _fillSpeed = 3f;

        private Image _astBarImage; // 실제 Ast를 표시할 Filled 이미지
        private float _targetFill;  // 목표로 하는 fillAmount 값 (0.0 ~ 1.0)

        private void Awake()
        {
            // 자신의 Image 컴포넌트를 가져옵니다.
            _astBarImage = GetComponent<Image>();
            if (_astBarImage.type != Image.Type.Filled)
            {
                Debug.LogWarning("AstBarController가 붙어있는 Image가 'Filled' 타입이 아닙니다!", this);
            }

            // CharacterStats에서 Ast는 maxAst로 초기화됩니다.
            _targetFill = 1f;
            _astBarImage.fillAmount = 1f;
        }

        private void Update()
        {
            // 현재 fillAmount 값을 목표(_targetFill) 값까지 부드럽게 이동시킵니다.
            if (_astBarImage.fillAmount != _targetFill)
            {
                _astBarImage.fillAmount = Mathf.Lerp(_astBarImage.fillAmount, _targetFill, Time.deltaTime * _fillSpeed);
            }
        }

        /// <summary>
        /// (PlayerHUDManager가 호출할) Ast 표시를 업데이트합니다.
        /// </summary>
        /// <param name="current">현재 Ast</param>
        /// <param name="max">최대 Ast</param>
        public void UpdateDisplay(float current, float max)
        {
            // 목표 fillAmount 값을 0과 1 사이로 정규화하여 설정합니다.
            _targetFill = Mathf.Clamp01(current / max);
        }
    }
}