// Assets/Scripts/Player/States/Airborne/PlayerAirborneState.cs
using Player.Animation;
using Player.Data;
using UnityEngine;

namespace Player.States
{
    public abstract class PlayerAirborneState : IState
    {
        protected readonly Player _player;
        protected readonly PlayerStateMachine _stateMachine;
        protected readonly PlayerInput _input;
        protected readonly PlayerMotor _motor;
        protected readonly PlayerSO _data;
        protected readonly PlayerAnimationController _animController;

        public PlayerAirborneState(Player player, PlayerStateMachine stateMachine, PlayerInput input, PlayerMotor motor, PlayerSO data, PlayerAnimationController animController)
        {
            _player = player;
            _stateMachine = stateMachine;
            _input = input;
            _motor = motor;
            _data = data;
            _animController = animController;
        }

        public virtual void Enter()
        {
        }

        public virtual void Update()
        {
            // 더블 점프 입력을 최우선으로 확인합니다.
            if (_input.IsJumpPressed && _player.Stats.CanDoubleJump)
            {
                // 1. 더블 점프 기회를 사용 처리합니다.
                _player.Stats.UseDoubleJump();
                _animController.PlayJump();

                // 2. (핵심) 현재의 수평 속도를 0으로 만들어 관성을 탈출합니다.
                _motor.Stop();
                _motor.ResetVerticalVelocity();

                // 3. 설정된 더블 점프 힘으로 새로 점프합니다.
                _motor.Jump(_data.doubleJumpForce);
                DoubleJumpDash();

                // TODO: 더블 점프 전용 애니메이션 트리거를 재생합니다.
                // _animController.PlayDoubleJump();
                return; // 더블 점프 후 다른 로직을 실행하지 않도록 합니다.
            }

            // 조준 입력을 확인합니다.
            if (_input.IsWireAiming)
            {
                _stateMachine.ChangeState(_player.WireAimState);
                return;
            }
            // 공중제어
            HandleAirControl();
        }

        public virtual void Exit()
        {
        }

        /// <summary>
        /// 공중에 떠 있는 동안 캐릭터를 수평으로 움직일 수 있게 합니다.
        /// </summary>
        private void HandleAirControl()
        {
            if (_input.MoveInput.sqrMagnitude < 0.01f) return;

            Vector3 cameraForward = Camera.main.transform.forward;
            Vector3 cameraRight = Camera.main.transform.right;
            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();
            Vector3 moveDirection = (cameraForward * _input.MoveInput.y + cameraRight * _input.MoveInput.x).normalized;

            // 공중 제어 시에는 걷는 속도를 사용 (나중에 별도 변수로 분리 가능)
            _motor.AirMove(moveDirection, _data.airControlForce);
        }       
        
        private void DoubleJumpDash()
        {
            Vector3 dashDirection = Camera.main.transform.forward;
            dashDirection.y = 0;
            _motor.GetComponent<Rigidbody>().AddForce(dashDirection.normalized * _data.doubleJumpDashForce, ForceMode.Impulse);
        }
    }
}