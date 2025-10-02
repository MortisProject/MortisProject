// Assets/Scripts/Player/States/Airborne/PlayerJumpState.cs
using UnityEngine;
using Player.Animation;

namespace Player.States
{
    public class PlayerJumpState : IState
    {
        // 필요한 컴포넌트 참조 변수들
        private readonly Player _player;
        private readonly PlayerStateMachine _stateMachine;
        private readonly PlayerMotor _motor;
        private readonly CharacterStats _stats;
        private readonly PlayerAnimationController _animController;
        
        public PlayerJumpState(Player player, PlayerStateMachine stateMachine, PlayerMotor motor, CharacterStats stats, PlayerAnimationController animController)
        {
            _player = player;
            _stateMachine = stateMachine;
            _motor = motor;
            _stats = stats;
            _animController = animController;
        }

        public void Enter()
        {
            // Motor에 점프 명령
            _motor.Jump(_stats.jumpHeight);
            _animController.PlayJump();
        }


        public void Update()
        {
            // 착지 감지는 PlayerStateMachine이 하므로, 그 값을 읽어오기만 하면 됩니다.
            if (_stateMachine.IsGrounded)
            {
                _stateMachine.ChangeState(_player.IdleState);
            }
        }

        public void Exit()
        {
            // 점프 상태에서 나갈 때 필요한 정리 로직 (예: 점프 애니메이션 트리거 리셋 등)
        }
    }
}