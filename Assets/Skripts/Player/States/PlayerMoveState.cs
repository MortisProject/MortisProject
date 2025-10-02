// Assets/Scripts/Player/States/Grounded/PlayerMoveState.cs
using UnityEngine;
using Player.Animation;

namespace Player.States
{
    public class PlayerMoveState : PlayerGroundedState
    {
        private readonly CharacterStats _stats;

        public PlayerMoveState(Player player, PlayerStateMachine stateMachine, PlayerInput input, PlayerMotor motor, CharacterStats stats, PlayerAnimationController animController)
            : base(player, stateMachine, input, motor, animController)
        {
            _stats = stats;
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
            if (_input.MoveInput == Vector2.zero)
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

            float targetSpeed = _input.IsRunning ? _stats.runSpeed : _stats.walkSpeed;
            _motor.Move(moveDirection * targetSpeed);

            Vector3 localMove = _player.transform.InverseTransformDirection(moveDirection);
            float runMultiplier = _input.IsRunning ? 2f : 1f;
            _animController.SetMove(localMove.x * runMultiplier, localMove.z * runMultiplier); // �ִϸ��̼� ��Ʈ�ѷ� ���

        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}