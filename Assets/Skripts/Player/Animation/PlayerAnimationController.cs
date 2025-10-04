// Assets/Scripts/Player/Animation/PlayerAnimationController.cs
using UnityEngine;

namespace Player.Animation
{
    /// <summary>
    /// Animator 컴포넌트를 직접 제어하는 중앙 컨트롤러.
    /// 모든 상태(State)는 이 스크립트를 통해 애니메이션 재생을 요청합니다.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class PlayerAnimationController : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("애니메이션 파라미터가 목표 값으로 변하는 데 걸리는 시간입니다.")]
        [SerializeField] private float _smoothTime = 0.1f;

        // Animator 파라미터 이름을 미리 해시값으로 변환하여 성능 향상
        private readonly int _moveXHash = Animator.StringToHash("MoveX");
        private readonly int _moveYHash = Animator.StringToHash("MoveY");
        private readonly int _jumpHash = Animator.StringToHash("Jump");
        private readonly int _landHash = Animator.StringToHash("Land");

        // TODO: 추후 공격, 닷지 등의 애니메이션 해시값을 여기에 추가합니다.
        // private readonly int _attackTriggerHash = Animator.StringToHash("Attack");

        private Animator _animator;

        // 상태(State)가 설정한 목표 이동 값
        private Vector2 _targetMove;

        // SmoothDamp를 위한 현재 속도 값 (내부적으로 사용됨)
        private Vector2 _currentMoveVelocity;

        private void Awake()
        {
            // Animator 컴포넌트를 캐싱
            _animator = GetComponent<Animator>();
        }

        /// <summary>
        /// 매 프레임 호출되어 현재 애니메이션 값을 목표 값으로 부드럽게 이동시킵니다.
        /// </summary>
        private void Update()
        {
            // 현재 애니메이터의 MoveX, MoveY 값을 가져옵니다.
            float currentX = _animator.GetFloat(_moveXHash);
            float currentY = _animator.GetFloat(_moveYHash);

            
            // Mathf.SmoothDamp를 사용하여 현재 값을 목표 값으로 부드럽게 보간합니다.
            float smoothedX = Mathf.SmoothDamp(currentX, _targetMove.x, ref _currentMoveVelocity.x, _smoothTime);
            float smoothedY = Mathf.SmoothDamp(currentY, _targetMove.y, ref _currentMoveVelocity.y, _smoothTime);

            // 보간중 0에 수렴하면 0으로 고정
            if (Mathf.Abs(smoothedX) < 0.01f) smoothedX = 0f;
            if (Mathf.Abs(smoothedY) < 0.01f) smoothedY = 0f;

            // 최종 계산된 부드러운 값을 애니메이터에 직접 설정합니다. (dampTime 없이)
            _animator.SetFloat(_moveXHash, smoothedX);
            _animator.SetFloat(_moveYHash, smoothedY);
        }

        /// <summary>
        /// 이동 애니메이션의 블렌드 트리 값을 설정합니다.
        /// </summary>
        /// <param name="x">좌우 이동 값</param>
        /// <param name="y">앞뒤 이동 값</param>
        public void SetMove(float x, float y)
        {
            _targetMove.x = x;
            _targetMove.y = y;
        }

        /// <summary>
        /// 점프 애니메이션을 재생합니다.
        /// </summary>
        public void PlayJump()
        {
            _animator.SetTrigger(_jumpHash);
        }

        /// <summary>
        /// 착지 애니메이션을 재생합니다.
        /// </summary>
        public void PlayLand()
        {
            _animator.SetTrigger(_landHash);
        }
    }
}