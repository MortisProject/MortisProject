// Assets/Scripts/Player/States/Grounded/PlayerGroundedState.cs
using UnityEngine;
using Player.Animation;

namespace Player.States
{
    // MonoBehaviour를 상속받지 않는 일반 C# 클래스입니다.
    public abstract class PlayerGroundedState : IState
    {
        protected readonly Player _player; // Player 메인 클래스 참조 추가
        protected readonly PlayerStateMachine _stateMachine;
        protected readonly PlayerInput _input;
        protected readonly PlayerMotor _motor;
        protected readonly CharacterStats _stats;
        protected readonly PlayerAnimationController _animController;

        private float _fallGraceTimer; // 유예 시간 타이머 변수
        private const float FallGracePeriod = 0.2f; // 유예 시간을 상수

        public PlayerGroundedState(Player player, PlayerStateMachine stateMachine, PlayerInput input, PlayerMotor motor, CharacterStats stats, PlayerAnimationController animController)
        {
            _player = player;
            _stateMachine = stateMachine;
            _input = input;
            _motor = motor;
            _stats = stats;
            _animController = animController;
        }

        public virtual void Enter()
        {
            // grounded 상태에 진입할 때 공통적으로 할 로직 (예: 중력 값 변경 등)
            _fallGraceTimer = FallGracePeriod;
        }

        public virtual void Update()
        {
            // 버스트
            if (_input.IsBurstSkillPressed && _stats.CurrentBurst >= _stats.maxBurst)
            {
                _stateMachine.ChangeState(_player.BurstSkillState);
                return;
            }
            // 회피 방어 입력이 먼저
            // 회피
            if (_input.IsGuarding)
            {
                _stateMachine.ChangeState(_player.GuardState);
                return;
            }
            // 방어
            if (_input.IsDodgePressed)
            {
                _stateMachine.ChangeState(_player.DodgeState);
                return;
            }
            // 공격은 좌클릭(약공격)으로 진입
            if (_input.IsWeakAttackPressed)
            {
                // 공격 입력이 들어오면, 지상 공격 상태로 즉시 전환합니다.
                _stateMachine.ChangeState(_player.GroundedAttackState);
                return; // 상태가 전환되었으므로 아래 로직은 실행하지 않음
            }

            // --- 새로 추가된 무기 교체 로직 ---
            if (_input.IsSwapNextWeaponPressed || _input.IsSwapPrevWeaponPressed)
            {
                // 아스트 1 소모를 시도하고, 성공했을 때만 무기 교체
                if (_stats.ConsumeAst(1))
                {
                    if (_input.IsSwapNextWeaponPressed)
                    {
                        _stats.ChangeNextWeapon();
                    }
                    else
                    {
                        // _stats.SwapToPrevWeapon();
                    }
                    // TODO: 무기 교체 시 HUD 애니메이션 재생 등 시각적 연출 호출
                }
                else
                {
                    Debug.Log("아스트가 부족하여 무기를 교체할 수 없습니다.");
                    // TODO: 아스트 부족 시 사운드 효과 등 피드백 연출
                }
                return; // 무기 교체 시도는 다른 행동(점프 등)보다 우선하므로 여기서 종료
            }

            // 조준 입력을 점프보다 먼저 확인합니다. (조준 중 점프 방지) 
            if (_input.IsWireAiming)
            {
                _stateMachine.ChangeState(_player.WireAimState);
                return;
            }

            // 점프 입력만 확인합니다.
            if (_input.IsJumpPressed && _stateMachine.IsGrounded)
            {
                _stateMachine.ChangeState(_player.JumpState);
                return;
            }

            if (!_stateMachine.IsGrounded)
            {
                // 땅에 없다면 유예 시간 감소
                _fallGraceTimer -= Time.deltaTime;
                if (_fallGraceTimer <= 0f)
                {
                    // 유예 시간이 다 되면 추락 상태로 전환
                    _stateMachine.ChangeState(_player.FallState);
                }
            }
            else
            {
                // 땅에 있다면 유예 시간을 계속 초기화
                _fallGraceTimer = FallGracePeriod;
            }
        }

        public virtual void Exit()
        {
            // grounded 상태에서 빠져나갈 때 공통적으로 할 로직
        }
    }
}