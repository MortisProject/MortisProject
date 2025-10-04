// Assets/Scripts/Player/States/Grounded/PlayerGroundedState.cs
using UnityEngine;
using Player.Animation;

namespace Player.States
{
    // MonoBehaviour�� ��ӹ��� �ʴ� �Ϲ� C# Ŭ�����Դϴ�.
    public abstract class PlayerGroundedState : IState
    {
        protected readonly Player _player; // Player ���� Ŭ���� ���� �߰�
        protected readonly PlayerStateMachine _stateMachine;
        protected readonly PlayerInput _input;
        protected readonly PlayerMotor _motor;
        protected readonly PlayerAnimationController _animController;

        private float _fallGraceTimer; // ���� �ð� Ÿ�̸� ����

        public PlayerGroundedState(Player player, PlayerStateMachine stateMachine, PlayerInput input, PlayerMotor motor, PlayerAnimationController animController)
        {
            _player = player;
            _stateMachine = stateMachine;
            _input = input;
            _motor = motor; // ���� �Ҵ�
            _animController = animController;
        }

        public virtual void Enter()
        {
            // grounded ���¿� ������ �� ���������� �� ���� (��: �߷� �� ���� ��)
        }

        public virtual void Update()
        {
            // ���� �Է¸� Ȯ���մϴ�.
            if (_input.IsJumpPressed && _stateMachine.IsGrounded)
            {
                _stateMachine.ChangeState(_player.JumpState);
                return;
            }
        }

        public virtual void Exit()
        {
            // grounded ���¿��� �������� �� ���������� �� ����
        }
    }
}