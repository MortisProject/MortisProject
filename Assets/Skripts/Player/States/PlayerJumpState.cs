// Assets/Scripts/Player/States/Airborne/PlayerJumpState.cs
using Player.Animation;
using Player.Data;
using UnityEngine;

namespace Player.States
{
    public class PlayerJumpState : PlayerAirborneState
    {
        private float _jumpStartTime;

        // �����ڿ��� �θ� Ŭ������ �ʿ��� ��� ������ �����մϴ�.
        public PlayerJumpState(Player player, PlayerStateMachine stateMachine, PlayerInput input, PlayerMotor motor, PlayerSO data, PlayerAnimationController animController)
            : base(player, stateMachine, input, motor, data, animController)
        {
        }

        public override void Enter()
        {
            base.Enter(); // �θ��� Enter ȣ�� (����� �������)

            _motor.Jump(_data.jumpHeight);
            _animController.PlayJump();
            _jumpStartTime = Time.time;
        }


        public override void Update()
        {
            base.Update();

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

        public override void Exit()
        {
        }
    }
}