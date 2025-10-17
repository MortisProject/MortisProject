// Assets/Skripts/Player/States/PlayerDodgeState.cs
using Player.Animation;
using Player.Data;
using UnityEngine;

namespace Player.States
{
    /// <summary>
    /// 몬스터의 공격을 회피하는 상태입니다.
    /// 짧은 시간 동안 뒤로 빠르게 이동하며, 특정 공격 유형에 대해 무적 판정을 가집니다.
    /// </summary>
    public class PlayerDodgeState : IState
    {
        private readonly Player _player;
        private readonly PlayerStateMachine _stateMachine;
        private readonly PlayerMotor _motor;
        private readonly PlayerSO _data;
        private readonly PlayerAnimationController _animController;

        public float TimeSinceEntered { get; private set; }

        public PlayerDodgeState(Player player, PlayerStateMachine stateMachine, PlayerMotor motor, PlayerSO data, PlayerAnimationController animController)
        {
            _player = player;
            _stateMachine = stateMachine;
            _motor = motor;
            _data = data;
            _animController = animController;
        }

        /// <summary>
        /// 상태에 진입할 때 호출됩니다.
        /// </summary>
        public void Enter()
        {
            TimeSinceEntered = 0f;
            _animController.PlayDodge();
        }

        /// <summary>
        /// 매 프레임 호출됩니다.
        /// </summary>
        public void Update()
        {
            TimeSinceEntered += Time.deltaTime;
        }

        /// <summary>
        /// 상태를 빠져나갈 때 호출됩니다.
        /// </summary>
        public void Exit()
        {
            _player.Motor.EndDodgeMovement();
        }

        /// <summary>
        /// (PlayerAnimationEvents에서 호출) 애니메이션이 끝났을 때 상태를 전환합니다.
        /// </summary>
        public void FinishDodge()
        {
            _stateMachine.ChangeState(_player.IdleState);
        }
    }
}