// Assets/Scripts/Player/States/Airborne/PlayerJumpState.cs
using Player.Animation;
using Player.Data;
using UnityEngine;

namespace Player.States
{
    public class PlayerJumpState : IState
    {
        // �ʿ��� ������Ʈ ���� ������
        private readonly Player _player;
        private readonly PlayerStateMachine _stateMachine;
        private readonly PlayerMotor _motor;
        private readonly PlayerSO _data;
        private readonly PlayerAnimationController _animController;
       
        private float _jumpStartTime;

        public PlayerJumpState(Player player, 
            PlayerStateMachine stateMachine, 
            PlayerMotor motor, PlayerSO data, 
            PlayerAnimationController animController)
        {
            _player = player;
            _stateMachine = stateMachine;
            _motor = motor;
            _data = data;
            _animController = animController;
        }

        public void Enter()
        {
            // Motor�� ���� ���
            _motor.Jump(_data.jumpHeight);
            _animController.PlayJump();
            _jumpStartTime = Time.time;

            // TODO: ���� ����ݱ� �ִϸ��̼� ��� ���ð�
        }


        public void Update()
        {
            float verticalVelocity = _motor.VerticalVelocity;

            // ���� ���� �� 0.5���� ���� �ð��� ������ �������� �ϰ� ������ ���� �ʽ��ϴ�.
            if (Time.time < _jumpStartTime + 0.5f)
            {
                return;
            }

            // ���� �ð��� ���� ��, ���� �ӵ��� ������ �Ǹ�(�ϰ� ����) FallState�� ��ȯ�մϴ�.
            if (_motor.VerticalVelocity < 0f)
            {
                _stateMachine.ChangeState(_player.FallState);
            }

        }

        public void Exit()
        {
        }
    }
}