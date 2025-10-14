// Assets/Scripts/Player/States/PlayerGroundedAttackState.cs
using Player.Animation;
using Player.Data;

namespace Player.States
{
    public class PlayerGroundedAttackState : PlayerAttackState
    {
        // PlayerGroundedAttackState는 PlayerAttackState의 모든 기능을 상속받습니다.
        public PlayerGroundedAttackState(Player player, PlayerStateMachine stateMachine, PlayerInput input, PlayerMotor motor, CharacterStats stats, PlayerAnimationController animController)
            : base(player, stateMachine, input, motor, stats, animController)
        {
        }

        public override void Enter()
        {
            base.Enter(); // 부모 클래스(PlayerAttackState)의 Enter 로직 실행
        }

        public override void Update()
        {
            base.Update(); // 부모 클래스의 Update 로직 실행

            // TODO: 여기에 지상 공격만의 특별한 로직이 있다면 추가할 수 있습니다.
            // (예: 공격 중 이동 입력 시 약간의 전진)
        }

        public override void Exit()
        {
            base.Exit(); // 부모 클래스의 Exit 로직 실행
        }
    }
}