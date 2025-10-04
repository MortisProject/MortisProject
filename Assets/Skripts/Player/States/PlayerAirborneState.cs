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
            // 공중제어
            HandleAirControl();
        }

        /// <summary>
        /// 공중에 떠 있는 동안 캐릭터를 수평으로 움직일 수 있게 합니다.
        /// </summary>
        private void HandleAirControl()
        {
            Vector3 cameraForward = Camera.main.transform.forward;
            Vector3 cameraRight = Camera.main.transform.right;
            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();
            Vector3 moveDirection = (cameraForward * _input.MoveInput.y + cameraRight * _input.MoveInput.x).normalized;

            // 공중 제어 시에는 걷는 속도를 사용 (나중에 별도 변수로 분리 가능)
            _motor.Move(moveDirection * _data.walkSpeed);
        }

        public virtual void Exit()
        {
        }
    }
}