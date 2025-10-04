// Assets/Scripts/Player/States/Airborne/PlayerJumpState.cs
using Player.Animation;
using Player.Data;
using UnityEngine;

namespace Player.States
{
    public class PlayerJumpState : PlayerAirborneState
    {
        private float _jumpStartTime;

        // 생성자에서 부모 클래스에 필요한 모든 참조를 전달합니다.
        public PlayerJumpState(Player player, PlayerStateMachine stateMachine, PlayerInput input, PlayerMotor motor, PlayerSO data, PlayerAnimationController animController)
            : base(player, stateMachine, input, motor, data, animController)
        {
        }

        public override void Enter()
        {
            base.Enter(); // 부모의 Enter 호출 (현재는 비어있음)

            _motor.Jump(_data.jumpHeight);
            _animController.PlayJump();
            _jumpStartTime = Time.time;
        }


        public override void Update()
        {
            base.Update();

            // 점프 시작 후 0.5초의 유예 시간이 지나기 전까지는 하강 판정을 하지 않습니다.
            if (Time.time < _jumpStartTime + 0.5f)
            {
                return;
            }

            // 유예 시간이 지난 후, 수직 속도가 음수가 되면(하강 시작) FallState로 전환합니다.
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