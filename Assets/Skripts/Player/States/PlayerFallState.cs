// Assets/Scripts/Player/States/Airborne/PlayerFallState.cs
using Player.Animation;
using UnityEngine;

namespace Player.States
{
    public class PlayerFallState : IState
    {
        private readonly Player _player;
        private readonly PlayerStateMachine _stateMachine;
        private readonly PlayerMotor _motor;
        private readonly PlayerAnimationController _animController;

        // TODO: 공중에서 캐릭터를 약간씩 움직일 수 있는 '공중 제어' 기능을 추가하면 조작감이 향상됩니다.

        public PlayerFallState(Player player, PlayerStateMachine stateMachine, PlayerMotor motor, PlayerAnimationController animController)
        {
            _player = player;
            _stateMachine = stateMachine;
            _motor = motor;
            _animController = animController;
        }

        public void Enter()
        {
            // 추락 애니메이션을 재생하도록 애니메이터 파라미터를 설정합니다.
            // TODO: PlayerAnimationController에 PlayFall() 같은 메서드를 추가하면 좋습니다.
            // _animController.PlayFall(true);
        }

        public void Update()
        {
            // 매 프레임마다 착지했는지 확인합니다.
            if (_stateMachine.IsGrounded)
            {       
                // 착지 신호를 애니메이션 컨트롤러에 보냅니다.
                _animController.PlayLand();
                // 그 후, Idle 상태로 전환합니다.
                _stateMachine.ChangeState(_player.IdleState);
            }
        }

        public void Exit()
        {

        }
    }
}