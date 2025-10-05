// Assets/Scripts/Player/States/Wire/PlayerWireLaunchState.cs
using Player.Animation;
using Player.Data;
using UnityEngine;

namespace Player.States
{
    public class PlayerWireLaunchState : IState
    {
        private readonly Player _player;
        private readonly PlayerStateMachine _stateMachine;
        private readonly PlayerMotor _motor;
        private readonly PlayerSO _data;
        private readonly PlayerAnimationController _animController;

        private float _launchStartTime; // 상태에 진입한 시간

        public PlayerWireLaunchState(Player player, PlayerStateMachine stateMachine, PlayerMotor motor, PlayerSO data, PlayerAnimationController animController)
        {
            _player = player;
            _stateMachine = stateMachine;
            _motor = motor;
            _data = data;
            _animController = animController;
        }

        public void Enter()
        {
            _launchStartTime = Time.time;

            // 뒤로 살짝 점프하는 움직임 실행
            _motor.Jump(_data.wireLaunchBackwardImpulse);
            _motor.Move(_player.transform.forward * _data.wireLaunchBackwardImpulse);

            // 애니메이션 컨트롤러에 발사 신호 전달
            // 와이어 발사애니 (유지)
            // 와이어 애니 프레임에 이벤트로 와이어 발사.
            // 와이어가 이어졌는 신호를 받으면 뒤로 점프 애니
            _animController.PlayWireStartJump();
            // 와이어 이동 시작 알림
            _animController.SetWireMove(true);


            // TODO: 와이어가 날아가는 시각 효과(VFX)를 여기서 생성합니다.
        }

        public void Update()
        {
            // [와이어 대기시간]이 지나면 MoveState로 전환
            if (Time.time >= _launchStartTime + _data.wireLaunchDelay)
            {
                _stateMachine.ChangeState(_player.WireMoveState);
            }
        }

        public void Exit()
        {
        }
    }
}