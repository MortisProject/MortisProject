// Assets/Skripts/Player/States/PlayerHitState.cs
using Player.Animation;
using Player.Data;
using UnityEngine;

namespace Player.States
{
    /// <summary>
    /// 몬스터의 공격에 피격당했을 때의 상태입니다.
    /// 짧은 경직과 함께 뒤로 밀려나며, 일정 시간 동안 다른 행동이 불가능합니다.
    /// </summary>
    public class PlayerHitState : IState
    {
        private readonly Player _player;
        private readonly PlayerStateMachine _stateMachine;
        private readonly PlayerMotor _motor;
        private readonly PlayerSO _data;
        private readonly PlayerAnimationController _animController;

        private float _stunTimer; // 경직 시간을 계산할 타이머

        public PlayerHitState(Player player, PlayerStateMachine stateMachine, PlayerMotor motor, PlayerSO data, PlayerAnimationController animController)
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
            // TODO: PlayerSO에 hitStunDuration 변수 추가 후 연결
            _stunTimer = _data.hitStunDuration;

            // TODO: PlayerMotor에 피격 넉백 관련 메서드 추가 후 연결
            //_motor.ApplyHitKnockback(_data.hitKnockbackForce);

            // TODO: PlayerAnimationController에 피격 애니메이션 재생 메서드 추가
            _animController.PlayHit();
        }

        /// <summary>
        /// 매 프레임 호출됩니다.
        /// </summary>
        public void Update()
        {
            // 경직 타이머를 감소시킵니다.
            _stunTimer -= Time.deltaTime;

            // 경직 시간이 끝나면 Idle 상태로 전환합니다.
            if (_stunTimer <= 0f)
            {
                _stateMachine.ChangeState(_player.IdleState);
            }
        }

        /// <summary>
        /// 상태를 빠져나갈 때 호출됩니다.
        /// </summary>
        public void Exit()
        {
            // CharacterStats에 경직 면역 코루틴 시작을 요청합니다.
            _player.Stats.StartStunImmunity();
        }
    }
}