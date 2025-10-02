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

        // TODO: 점프 상태로 전환하기 위해 점프 상태의 인스턴스가 필요합니다.

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
        }

        public virtual void Update()
        {
            // 땅 위에 있을 때 공통적으로 체크할 전환 조건들
            if (_input.IsJumpPressed)
            {
                _stateMachine.ChangeState(_player.JumpState); // 점프 상태로 전환
            }
        }

        public virtual void Exit()
        {
            // grounded 상태에서 빠져나갈 때 공통적으로 할 로직
        }
    }
}