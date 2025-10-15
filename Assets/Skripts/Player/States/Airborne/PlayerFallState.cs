// Assets/Scripts/Player/States/Airborne/PlayerFallState.cs
using Player.Animation;
using Player.Data; // 참조를 위해 추가
using UnityEngine;

namespace Player.States
{
    // IState 대신 PlayerAirborneState를 상속받습니다.
    public class PlayerFallState : PlayerAirborneState
    {
        // 생성자에서 부모 클래스에 필요한 모든 참조를 전달합니다.
        public PlayerFallState(Player player, PlayerStateMachine stateMachine, PlayerInput input, PlayerMotor motor, PlayerSO data, PlayerAnimationController animController)
            : base(player, stateMachine, input, motor, data, animController)
        {
        }

        public override void Enter()
        {
            base.Enter();
        }

        public override void Update()
        {
            base.Update();

            if (_stateMachine.IsGrounded)
            {
                _stateMachine.ChangeState(_player.IdleState);
                return;
            }
        }
    }
}