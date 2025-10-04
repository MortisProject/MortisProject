// Assets/Scripts/Player/States/Grounded/PlayerIdleState.cs
using UnityEngine;
using Player.Animation;

namespace Player.States
{
    public class PlayerIdleState : PlayerGroundedState
    {
        public PlayerIdleState(Player player, PlayerStateMachine stateMachine, PlayerInput input, PlayerMotor motor, PlayerAnimationController animController)
            : base(player, stateMachine, input, motor, animController)
        {
        }

        public override void Enter()
        {
            base.Enter(); // 부모 클래스의 Enter 로직 실행

            // Idle 상태에 진입했으므로, 이동 애니메이션 파라미터를 0으로 설정
            //_animController.SetMove(0f, 0f);
        }

        public override void Update()
        {
            base.Update(); // 부모 클래스의 Update 로직(점프 등) 실행

            _animController.SetMove(0f, 0f);
            // 이동 입력이 있는지 확인
            if (_input.MoveInput.sqrMagnitude > 0.01f)
            {
                // 이동 입력이 있다면, Move 상태로 전환
                _stateMachine.ChangeState(_player.MoveState);
                return; // 상태 전환이 일어났으므로 더 이상 Idle 상태의 로직을 실행하지 않음
            }
        }

        public override void Exit()
        {
            base.Exit(); // 부모 클래스의 Exit 로직 실행
        }
    }
}