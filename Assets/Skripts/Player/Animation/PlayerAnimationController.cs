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

        // Animator 파라미터 이름을 미리 해시값으로 변환
        // move
        private readonly int _moveXHash = Animator.StringToHash("MoveX");
        private readonly int _moveYHash = Animator.StringToHash("MoveY");
        // jump
        private readonly int _jumpHash = Animator.StringToHash("Jump");
        // wire
        private readonly int _isGroundedHash = Animator.StringToHash("IsGrounded");
        private readonly int _wireStartJumpHash = Animator.StringToHash("WireStartJump");
        private readonly int _isWrieMoveHash = Animator.StringToHash("isWireMove");
        // attack
        private readonly int _comboStackHash = Animator.StringToHash("ComboStack");
        private readonly int _whipAttackStartHash = Animator.StringToHash("Whip_Attack_Start");
        private readonly int _raygunAttackStartHash = Animator.StringToHash("Raygun_Attack_Start");
        private readonly int _weakAttackHash = Animator.StringToHash("Weak_Attack");
        private readonly int _strongAttackHash = Animator.StringToHash("Strong_Attack");
        private readonly int _whipBurstHash = Animator.StringToHash("Whip_Burst");
        private readonly int _raygunBurstHash = Animator.StringToHash("Raygun_Burst");
        private readonly int _whipSwapHash = Animator.StringToHash("Whip_Swap");
        private readonly int _raygunSwapHash = Animator.StringToHash("Raygun_Swap");
        private readonly int _noInputHash = Animator.StringToHash("No_Input");
        // defensive
        private readonly int _hitHash = Animator.StringToHash("Hit");
        private readonly int _isGuardingHash = Animator.StringToHash("IsGuarding");
        private readonly int _dodgeHash = Animator.StringToHash("Dodge");
        private readonly int _dodgePerfectHash = Animator.StringToHash("Dodge_Perfect");
        private readonly int _guardHitHash = Animator.StringToHash("Guard_Hit");
        private readonly int _guardBreakHash = Animator.StringToHash("Guard_Break");
        private readonly int _guardPerfectHash = Animator.StringToHash("Guard_Parfect");

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

        public void SetMove(float x, float y){ _targetMove.x = x; _targetMove.y = y; }
        public void PlayJump() => _animator.SetTrigger(_jumpHash);
        public void SetGrounded(bool isGrounded) => _animator.SetBool(_isGroundedHash, isGrounded);
        public void SetWireMove(bool isWireMove) => _animator.SetBool(_isWrieMoveHash, isWireMove);
        public void PlayWireStartJump() => _animator.SetTrigger(_wireStartJumpHash);
        public void SetComboStack(int stack) => _animator.SetInteger(_comboStackHash, stack);
        public void StartWhipAttack() => _animator.SetTrigger(_whipAttackStartHash);
        public void StartRaygunAttack() => _animator.SetTrigger(_raygunAttackStartHash);
        public void PlayWeakAttack() => _animator.SetTrigger(_weakAttackHash);
        public void PlayStrongAttack() => _animator.SetTrigger(_strongAttackHash); 
        public void PlayWhipBurst() => _animator.SetTrigger(_whipBurstHash);
        public void PlayRaygunBurst() => _animator.SetTrigger(_raygunBurstHash);
        public void PlayWhipSwapAttack() => _animator.SetTrigger(_whipSwapHash);
        public void PlayRaygunSwapAttack() => _animator.SetTrigger(_raygunSwapHash);
        public void NoInput() => _animator.SetTrigger(_noInputHash);
        public void PlayHit() => _animator.SetTrigger(_hitHash);
        public void SetGuarding(bool isGuarding) => _animator.SetBool(_isGuardingHash, isGuarding);
        public void PlayDodge() => _animator.SetTrigger(_dodgeHash);
        public void PlayDodgePerfect() => _animator.SetTrigger(_dodgePerfectHash);
        public void PlayGuardHit() => _animator.SetTrigger(_guardHitHash);
        public void PlayGuardBreak() => _animator.SetTrigger(_guardBreakHash);
        public void PlayGuradPerfect() => _animator.SetTrigger(_guardPerfectHash);

        /// <summary>
        /// 이동 관련 애니메이터 파라미터와 내부 변수들을 즉시 0으로 초기화합니다.
        /// SmoothDamp를 무시하고 값을 강제로 리셋합니다.
        /// </summary>
        public void ResetMoveParameters()
        {
            // 1. 목표 이동 값을 0으로 설정합니다.
            _targetMove = Vector2.zero;

            // 2. SmoothDamp가 사용하는 현재 속도 값도 0으로 초기화합니다.
            _currentMoveVelocity = Vector2.zero;

            // 3. 애니메이터의 파라미터 값을 즉시 0으로 강제 설정합니다.
            _animator.SetFloat(_moveXHash, 0f);
            _animator.SetFloat(_moveYHash, 0f);
        }
    }
}