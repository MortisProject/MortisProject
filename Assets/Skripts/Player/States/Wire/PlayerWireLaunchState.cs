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

        private GameObject _hookObject;

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
            // WireHook 프리팹을 생성하고 목표를 설정합니다.
            _hookObject = Object.Instantiate(_data.wireHookPrefab, _player.WireOrigin.position, Quaternion.identity);
            _hookObject.GetComponent<WireHook>().target = _stateMachine.WireTarget;

            // WireRenderer를 '출렁임' 모드로 활성화하고, 날아가는 훅을 따라가도록 합니다.
            _player.WireRenderer.Activate(_player.WireOrigin, _stateMachine.WireTarget, _hookObject.transform);


            // 애니메이션 컨트롤러에 발사 신호를 전달합니다.


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
            // 훅이 목표 지점에 거의 도달했는지 매 프레임 확인합니다.
            if (_hookObject != null && Vector3.Distance(_hookObject.transform.position, _stateMachine.WireTarget.position) < 1f)
            {
                // 도달했다면 MoveState로 전환합니다.
                _stateMachine.ChangeState(_player.WireMoveState);
            }
        }

        public void Exit()
        {
            if (_hookObject != null)
            {
                Object.Destroy(_hookObject);
            }
        }
    }
}