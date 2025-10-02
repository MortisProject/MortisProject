// Assets/Scripts/Player/States/Airborne/PlayerJumpState.cs
using UnityEngine;
using Player.Animation;

namespace Player.States
{
    public class PlayerJumpState : IState
    {
        // �ʿ��� ������Ʈ ���� ������
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
            // Motor�� ���� ���
            _motor.Jump(_stats.jumpHeight);
            _animController.PlayJump();
        }


        public void Update()
        {
            // ���� ������ PlayerStateMachine�� �ϹǷ�, �� ���� �о���⸸ �ϸ� �˴ϴ�.
            if (_stateMachine.IsGrounded)
            {
                _stateMachine.ChangeState(_player.IdleState);
            }
        }

        public void Exit()
        {
            // ���� ���¿��� ���� �� �ʿ��� ���� ���� (��: ���� �ִϸ��̼� Ʈ���� ���� ��)
        }
    }
}