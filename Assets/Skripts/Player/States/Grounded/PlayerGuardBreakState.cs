// Assets/Skripts/Player/States/PlayerGuardBreakState.cs
using Player.Animation;
using UnityEngine;

namespace Player.States
{
    /// <summary>
    /// 가드 게이지 부족으로 가드가 파괴되었을 때의 상태입니다.
    /// 전용 애니메이션을 재생하고, 애니메이션이 끝나면 Idle 상태로 전환됩니다.
    /// </summary>
    public class PlayerGuardBreakState : IState
    {
        private readonly Player _player;
        private readonly PlayerStateMachine _stateMachine;
        private readonly PlayerAnimationController _animController;

        public PlayerGuardBreakState(Player player, PlayerStateMachine stateMachine, PlayerAnimationController animController)
        {
            _player = player;
            _stateMachine = stateMachine;
            _animController = animController;
        }

        public void Enter()
        {
        }

        public void Update()
        {
            // 이 상태는 애니메이션 이벤트에 의해 제어되므로 Update에서는 아무것도 하지 않습니다.
        }

        public void Exit()
        {
            // 특별한 종료 로직은 없습니다.
        }

        /// <summary>
        /// (애니메이션 이벤트에서 호출) 애니메이션이 끝났을 때 Idle 상태로 전환합니다.
        /// </summary>
        public void OnAnimationFinished()
        {
            _stateMachine.ChangeState(_player.IdleState);
        }
    }
}