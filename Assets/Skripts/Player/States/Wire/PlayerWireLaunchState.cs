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

            // TODO: '뒤로 살짝 점프'하는 로직 추가
            _motor.Jump(3f);
            _motor.Move(-_player.transform.forward * 2f);


            // TODO: 와이어 발사 애니메이션 및 VFX 재생
            // _animController.PlayWireLaunch();
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