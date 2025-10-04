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
            _data = data; // 참조 할당
        }

        public override void Enter()
        {
            base.Enter();
        }

        public override void Update()
        {
            // 부모 클래스의 Update를 호출하여 점프 같은 공통 전환 로직을 먼저 체크합니다.
            base.Update();

            // 이동 입력이 없다면, Idle 상태로 전환합니다.
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

            float runMultiplier = _input.IsRunning ? 1f : 0.5f; // 걷기는 0.5, 달리기는 1의 크기를 갖도록 설정
            _animController.SetMove(_input.MoveInput.x * runMultiplier, _input.MoveInput.y * runMultiplier);
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}