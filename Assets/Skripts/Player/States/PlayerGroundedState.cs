// Assets/Scripts/Player/States/Grounded/PlayerGroundedState.cs
using UnityEngine;
using Player.Animation;

namespace Player.States
{
    // MonoBehaviour를 상속받지 않는 일반 C# 클래스입니다.
    public abstract class PlayerGroundedState : IState
    {
        protected readonly Player _player; // Player 메인 클래스 참조 추가
        protected readonly PlayerStateMachine _stateMachine;
        protected readonly PlayerInput _input;
        protected readonly PlayerMotor _motor;
        protected readonly PlayerAnimationController _animController;

        private float _fallGraceTimer; // 유예 시간 타이머 변수
        private const float FallGracePeriod = 0.2f; // 유예 시간을 상수

        public PlayerGroundedState(Player player, PlayerStateMachine stateMachine, PlayerInput input, PlayerMotor motor, PlayerAnimationController animController)
        {
            _player = player;
            _stateMachine = stateMachine;
            _input = input;
            _motor = motor; // 참조 할당
            _animController = animController;
        }

        public virtual void Enter()
        {
            // grounded 상태에 진입할 때 공통적으로 할 로직 (예: 중력 값 변경 등)
            _fallGraceTimer = FallGracePeriod;
        }

        public virtual void Update()
        {
            // 점프 입력만 확인합니다.
            if (_input.IsJumpPressed && _stateMachine.IsGrounded)
            {
                _stateMachine.ChangeState(_player.JumpState);
                return;
            }

            if (!_stateMachine.IsGrounded)
            {
                // 땅에 없다면 유예 시간 감소
                _fallGraceTimer -= Time.deltaTime;
                if (_fallGraceTimer <= 0f)
                {
                    // 유예 시간이 다 되면 추락 상태로 전환
                    _stateMachine.ChangeState(_player.FallState);
                }
            }
            else
            {
                // 땅에 있다면 유예 시간을 계속 초기화
                _fallGraceTimer = FallGracePeriod;
            }
        }

        public virtual void Exit()
        {
            // grounded 상태에서 빠져나갈 때 공통적으로 할 로직
        }
    }
}