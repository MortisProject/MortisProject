// Assets/Skripts/Player/States/PlayerGuardState.cs
using Player.Animation;
using UnityEngine;

namespace Player.States
{
    /// <summary>
    /// 몬스터의 공격을 방어하는 상태입니다.
    /// 키를 누르고 있는 동안 유지되며, 특정 공격 유형에 따라 다른 결과를 가집니다.
    /// </summary>
    public class PlayerGuardState : IState
    {
        private readonly Player _player;
        private readonly PlayerStateMachine _stateMachine;
        private readonly PlayerInput _input;
        private readonly PlayerAnimationController _animController;

        public PlayerGuardState(Player player, PlayerStateMachine stateMachine, PlayerInput input, PlayerAnimationController animController)
        {
            _player = player;
            _stateMachine = stateMachine;
            _input = input;
            _animController = animController;
        }

        /// <summary>
        /// 상태에 진입할 때 호출됩니다.
        /// </summary>
        public void Enter()
        {
            // TODO: PlayerAnimationController에 가드 시작/유지 애니메이션 관련 메서드 추가
            _animController.SetGuarding(true);
        }

        /// <summary>
        /// 매 프레임 호출됩니다.
        /// </summary>
        public void Update()
        {
            // 가드 키에서 손을 떼면 Idle 상태로 돌아갑니다.
            if (!_input.IsGuarding)
            {
                _stateMachine.ChangeState(_player.IdleState);
                return;
            }
        }

        /// <summary>
        /// 상태를 빠져나갈 때 호출됩니다.
        /// </summary>
        public void Exit()
        {
            // TODO: PlayerAnimationController에 가드 종료 애니메이션 관련 메서드 추가
            _animController.SetGuarding(false);
        }
    }
}