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
            // WireHook 프리팹을 생성합니다.
            GameObject hookObject = Object.Instantiate(_data.wireHookPrefab, _player.WireOrigin.position, Quaternion.identity);

            // 생성된 훅의 목표 지점을 설정합니다.
            hookObject.GetComponent<WireHook>().target = _stateMachine.WireTarget;

            // WireRenderer를 활성화하고, 시작점/끝점/그리고 '날아가는 훅'의 transform을 전달합니다.
            _player.WireRenderer.Activate(_player.WireOrigin, _stateMachine.WireTarget, hookObject.transform);

            // 애니메이션 컨트롤러에 발사 신호를 전달합니다.


            // 땅에 있다면 와이어 던지는 애니 실행
            if(_stateMachine.IsGrounded)
            {
                _animController.PlayWireLaunch();
            }
            // 와이어 애니 프레임에 이벤트로 와이어 발사.
            // 와이어가 이어졌는 신호를 받으면 뒤로 점프 애니
            _animController.PlayWireStartJump();
            // 모터에서 뒤로 점프 호출
            _motor.Jump(_data.wireLaunchBackwardImpulse);
            _motor.Move(_player.transform.forward * _data.wireLaunchBackwardImpulse);

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