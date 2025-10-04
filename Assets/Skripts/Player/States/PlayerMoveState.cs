// Assets/Scripts/Player/States/Grounded/PlayerMoveState.cs
using Player.Animation;
using Player.Data;
using UnityEngine;

namespace Player.States
{
    public class PlayerMoveState : PlayerGroundedState
    {
        private readonly PlayerSO _data;

        public PlayerMoveState(Player player, PlayerStateMachine stateMachine, PlayerInput input, PlayerMotor motor, PlayerSO data, PlayerAnimationController animController)
            : base(player, stateMachine, input, motor, animController)
        {
            _data = data; // ���� �Ҵ�
        }

        public override void Enter()
        {
            base.Enter();
        }

        public override void Update()
        {
            // �θ� Ŭ������ Update�� ȣ���Ͽ� ���� ���� ���� ��ȯ ������ ���� üũ�մϴ�.
            base.Update();

            // �̵� �Է��� ���ٸ�, Idle ���·� ��ȯ�մϴ�.
            if (_input.MoveInput.sqrMagnitude < 0.01f)
            {
                _stateMachine.ChangeState(_player.IdleState);
                return;
            }

            Vector3 cameraForward = Camera.main.transform.forward;
            Vector3 cameraRight = Camera.main.transform.right;
            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();
            Vector3 moveDirection = (cameraForward * _input.MoveInput.y + cameraRight * _input.MoveInput.x).normalized;

            float targetSpeed = _input.IsRunning ? _data.runSpeed : _data.walkSpeed;
            _motor.Move(moveDirection * targetSpeed);

            float runMultiplier = _input.IsRunning ? 1f : 0.5f; // �ȱ�� 0.5, �޸���� 1�� ũ�⸦ ������ ����
            _animController.SetMove(_input.MoveInput.x * runMultiplier, _input.MoveInput.y * runMultiplier);
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}