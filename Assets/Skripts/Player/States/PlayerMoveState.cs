// Assets/Scripts/Player/States/Grounded/PlayerMoveState.cs
using Player.Animation;
using Player.Data;
using UnityEngine;

namespace Player.States
{
    public class PlayerMoveState : PlayerGroundedState
    {
        private readonly PlayerSO _data;

        public PlayerMoveState(Player player, PlayerStateMachine stateMachine, PlayerInput input, PlayerMotor motor, PlayerSO data, CharacterStats stats, PlayerAnimationController animController)
            : base(player, stateMachine, input, motor, stats, animController)
        {
            _data = data; // 참조 할당
        }

        public override void Enter()
        {
            base.Enter();
        }

        public override void Update()
        {
            base.Update();

            if (_input.MoveInput.sqrMagnitude < 0.01f)
            {
                _stateMachine.ChangeState(_player.IdleState);
                return;
            }

            // --- 1. 이동 명령 ---
            Vector3 cameraForward = Camera.main.transform.forward;
            Vector3 cameraRight = Camera.main.transform.right;
            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();
            Vector3 moveDirection = (cameraForward * _input.MoveInput.y + cameraRight * _input.MoveInput.x).normalized;

            float targetSpeed = _input.IsRunning ? _data.runSpeed : _data.walkSpeed;

            // Motor의 Move 메서드 호출
            _motor.Move(moveDirection * targetSpeed);

            // --- 2. 속도 기반 애니메이션 ---
            // Rigidbody의 실제 월드 속도를 가져옵니다.
            Vector3 worldVelocity = _motor.Velocity;
            // 월드 속도를 플레이어의 로컬 방향 기준으로 변환합니다.
            Vector3 localVelocity = _player.transform.InverseTransformDirection(worldVelocity);
            // 최대 속도를 기준으로 현재 속도를 -1 ~ 1 사이로 정규화합니다.
            float normalizedZ = localVelocity.z / _data.runSpeed;
            float normalizedX = localVelocity.x / _data.runSpeed;

            // 최종 계산된 정규화 값을 애니메이터에 전달합니다.
            // 1. 목표 크기(Magnitude)를 정합니다. (애니메이터 설정에 맞게 걷기: 0.5, 달리기: 1)
            float targetMagnitude = _input.IsRunning ? 1f : 0.5f;

            // 2. 현재 입력 방향 벡터에 목표 크기를 곱합니다.
            Vector2 animVector = _input.MoveInput * targetMagnitude;

            // 3. 최종 계산된 값을 애니메이터에 전달합니다.
            _animController.SetMove(animVector.x, animVector.y);
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}