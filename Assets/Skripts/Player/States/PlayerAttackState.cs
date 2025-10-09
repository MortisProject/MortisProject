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
            _currentAttackType = _input.IsStrongAttackPressed ? AttackType.StrongAttack : AttackType.WeakAttack;
            TriggerAttack();
        }

        public virtual void Update()
        {
            // 콤보 유예 시간(타이머) 처리
            if (_comboTimer > 0)
            {
                _comboTimer -= Time.deltaTime;
                if (_comboTimer <= 0)
                {
                    // 유예 시간이 다 되면 콤보를 종료하고 Idle 상태로 돌아감
                    _stateMachine.ChangeState(_player.IdleState);
                    return;
                }
            }

            // '입력 유예 창'이 열려있을 때만 입력을 받음
            if (_isInputWindowOpen)
            {
                HandleAttackInput();
            }
        }

        public virtual void Exit()
        {
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
                _bufferedAttack = AttackType.StrongAttack; // 강공격은 언제나 버퍼에 저장
            }
            else if (_input.IsWeakAttackPressed)
            {
                if (_bufferedAttack == AttackType.None) // 버퍼가 비어있을 때만 약공격 저장
                {
                    _bufferedAttack = AttackType.WeakAttack;
                }
            }
        }

        /// <summary>
        /// 다음 콤보 공격을 실행하는 공통 메서드.
        /// </summary>
        protected void TriggerAttack()
        {
            _isInputWindowOpen = false; // 다음 공격이 시작되면 입력 창을 닫음
            _comboIndex++; // 콤보 카운트 증가

            // TODO: 현재 콤보와 무기에 맞는 SkillData를 찾아 애니메이션 재생
            // PlayAnimationForCurrentCombo();
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
        /// (애니메이션 이벤트) 현재 공격 모션이 끝나는 시점에 호출됩니다.
        /// </summary>
        public void OnAttackMotionEnd()
        {
            _isInputWindowOpen = false; // 입력 창을 닫음

            if (_bufferedAttack != AttackType.None)
            {
                // 다음 공격 타입 설정
                _currentAttackType = _bufferedAttack; 
                // 버퍼에 예약된 공격이 있으면 다음 콤보 실행
                AttackType nextAttack = _bufferedAttack;
                // 버퍼 비우기
                _bufferedAttack = AttackType.None; 

                // 여기서 다음 공격 타입을 전달하며 TriggerAttack() 호출
                TriggerAttack();
            }
            else
            {
                // 예약된 공격이 없으면 콤보 유예 시간 타이머 시작
                // TODO: 현재 스킬의 comboGraceTime을 가져와야 함
                _comboTimer = 0.6f; // 임시 값
            }
        }
    }
}