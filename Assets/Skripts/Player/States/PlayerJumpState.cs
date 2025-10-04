// Assets/Scripts/Player/States/Airborne/PlayerJumpState.cs
using Player.Animation;
using Player.Data;
using UnityEngine;

namespace Player.States
{
    public class PlayerJumpState : IState
    {
        // 필요한 컴포넌트 참조 변수들
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
            // Motor에 점프 명령
            _motor.Jump(_data.jumpHeight);
            _animController.PlayJump();
            _jumpStartTime = Time.time;

            // TODO: 점프 도움닫기 애니메이션 출력 대기시간
        }


        public void Update()
        {
            float verticalVelocity = _motor.VerticalVelocity;

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

        public void Exit()
        {
        }
    }
}