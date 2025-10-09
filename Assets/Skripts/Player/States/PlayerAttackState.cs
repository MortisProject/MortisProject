// Assets/Scripts/Player/States/PlayerAttackState.cs
using Player.Animation;
using Player.Data;
using UnityEngine;

namespace Player.States
{
    /// <summary>
    /// 모든 공격 상태의 기반이 되는 추상 클래스입니다.
    /// 콤보 시스템, 입력 버퍼, 상태 전환 등 공통 로직을 처리합니다.
    /// </summary>
    public abstract class PlayerAttackState : IState
    {
        // --- 참조 ---
        protected readonly Player _player;
        protected readonly PlayerStateMachine _stateMachine;
        protected readonly PlayerInput _input;
        protected readonly CharacterStats _stats;
        protected readonly PlayerAnimationController _animController;

        // --- 콤보 관리 ---
        protected int _comboIndex = 0; // 현재 몇 번째 콤보인지
        protected AttackType _currentAttackType = AttackType.None; // 현재 공격 타입이 무엇인지
        private float _comboTimer = 0f; // 콤보 유예 시간을 재는 타이머

        // --- 입력 버퍼 ---
        private AttackType _bufferedAttack = AttackType.None; // '우선순위 입력 버퍼'
        private bool _isInputWindowOpen = false; // 다음 콤보 입력을 받을 수 있는 '입력 유예 창'
        private bool _isAttackDelay = false; // 후딜레이 상태인지 확인

        public PlayerAttackState(Player player, PlayerStateMachine stateMachine, PlayerInput input, CharacterStats stats, PlayerAnimationController animController)
        {
            _player = player;
            _stateMachine = stateMachine;
            _input = input;
            _stats = stats;
            _animController = animController;
        }

        public virtual void Enter()
        {
            _comboIndex = 0; // 공격 상태에 처음 진입하면 콤보를 1타부터 시작
            _comboTimer = 0f;
            _isInputWindowOpen = false;
            _bufferedAttack = AttackType.None;

            // 첫 공격 실행
            TriggerAttack(AttackType.WeakAttack);
        }

        public virtual void Update()
        {
            // 후딜레이 상태이고, 입력 창이 열려있을 때만 입력을 받음
            if (_isAttackDelay && _isInputWindowOpen)
            {
                HandleAttackInput();
            }
        }

        public virtual void Exit()
        {
            _animController.SetComboStack(0);
            // 상태를 나갈 때 모든 변수 초기화
            _comboIndex = 0;
            _isInputWindowOpen = false;
            _bufferedAttack = AttackType.None;
        }

        /// <summary>
        /// '우선순위 입력 버퍼' 로직을 처리합니다. 강공격이 약공격을 덮어씁니다.
        /// </summary>
        private void HandleAttackInput()
        {
            if (_input.IsStrongAttackPressed)
            {
                _bufferedAttack = AttackType.StrongAttack; // 강공격 입력을 버퍼에 저장
            }
            else if (_input.IsWeakAttackPressed)
            {
                _bufferedAttack = AttackType.WeakAttack; // 약공격 입력을 버퍼에 저장

            }
        }

        /// <summary>
        /// 콤보 단계와 공격 타입에 맞는 애니메이션 트리거를 활성화합니다.
        /// </summary>
        protected void TriggerAttack(AttackType attackType)
        {
            _isInputWindowOpen = false;
            _isAttackDelay = false;
            _comboIndex++;

            _animController.SetComboStack(_comboIndex);

            // 콤보의 첫 공격인지, 이어지는 공격인지에 따라 다른 트리거를 사용합니다.
            if (_comboIndex == 1)
            {
                // 1타: 무기별 시작 트리거를 활성화합니다.
                if (_stats.CurrentWeapon == WeaponType.Whip)
                {
                    _animController.StartWhipAttack();
                }
                else if (_stats.CurrentWeapon == WeaponType.RayGun)
                {
                    _animController.StartRaygunAttack();
                }
            }
            else
            {
                // 2타 이상: 약/강 공격 타입에 맞는 트리거를 활성화합니다.
                if (attackType == AttackType.WeakAttack)
                {
                    _animController.PlayWeakAttack();
                }
                else if (attackType == AttackType.StrongAttack)
                {
                    _animController.PlayStrongAttack();
                }
            }
        }

        // --- 애니메이션 이벤트에서 호출될 메서드들 ---

        /// <summary>
        /// (애니메이션 이벤트) 다음 입력을 받을 수 있는 '입력 유예 창'을 엽니다.
        /// </summary>
        public void OpenInputWindow()
        {
            _isInputWindowOpen = true;
        }

        /// <summary>
        /// 후딜레이 상태로 전환하고, 예약된 다음 공격을 실행합니다.
        /// </summary>
        public void StartAttackDelay()
        {
            _isAttackDelay = true;
            _isInputWindowOpen = false; // 입력 저장 종료

            if (_bufferedAttack != AttackType.None)
            {
                // 버퍼에 예약된 공격이 있으면, 즉시 다음 콤보 실행
                TriggerAttack(_bufferedAttack);
                _bufferedAttack = AttackType.None;
            }
        }

        /// <summary>
        /// 콤보를 완전히 종료하고 Idle 상태로 돌아갑니다.
        /// </summary>
        public void EndAttackDelay()
        {
            // 만약 StartAttackDelay가 호출되었지만 다음 공격으로 이어지지 않은 경우에만 Idle로 전환
            if (_isAttackDelay)
            {
                _stateMachine.ChangeState(_player.IdleState);
            }
        }
    }
}