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
        protected readonly PlayerMotor _motor;
        protected readonly CharacterStats _stats;
        protected readonly PlayerAnimationController _animController;

        // --- 콤보 관리 ---
        protected int _comboIndex = 0; // 현재 몇 번째 콤보인지
        protected AttackType _currentAttackType = AttackType.None; // 현재 공격 타입이 무엇인지

        // --- 입력 버퍼 ---
        private AttackType _bufferedAttack = AttackType.None; // '우선순위 입력 버퍼'
        private bool _isInputWindowOpen = false; // 다음 콤보 입력을 받을 수 있는 '입력 유예 창'
        private bool _isAttackDelay = false; // 후딜레이 상태인지 확인

        // 공격 상태에 진입한 순간의 조준 방향을 저장합니다.
        public Vector3 AimDirection { get; private set; }

        // 현재 실행 중인 변환 공격 데이터
        private SwapAttackData _currentSwapAttackData;
        
        public PlayerAttackState(Player player, PlayerStateMachine stateMachine, PlayerInput input, PlayerMotor motor, CharacterStats stats, PlayerAnimationController animController)
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
            // 공격 상태 진입 시점의 카메라 정면 방향을 저장합니다.
            AimDirection = Camera.main.transform.forward;

            // 이동값 초기화
            _motor.Stop();

            _comboIndex = 0; // 공격 상태에 처음 진입하면 콤보를 1타부터 시작
            _isInputWindowOpen = false;
            _bufferedAttack = AttackType.None;
            _currentSwapAttackData = null;

            // 첫 공격 실행
            TriggerAttack(AttackType.WeakAttack);
        }

        public virtual void Update()
        {
            if (_comboIndex >= 4)
            {
                // 최대 콤보에 도달하면 더 이상 입력을 받지 않음
                return;
            }

            // 1. '입력 유예 창' (InputStart)이 열려있을 때 입력을 버퍼에 저장합니다.
            if (_isInputWindowOpen)
            {
                AimDirection = Camera.main.transform.forward;
                HandleAttackInput();
            }
            // 2. '후딜레이' (AttackDelay) 구간에 있을 때 새로운 입력을 감지합니다.
            else if (_isAttackDelay)
            {
                // 이 구간에서 새로운 입력이 들어왔는지 확인합니다.
                HandleAttackInput();

                // 버퍼에 새로운 공격이 예약되었다면 (즉, 방금 입력이 들어왔다면)
                if (_bufferedAttack != AttackType.None)
                {
                    // 즉시 다음 공격으로 전환합니다.
                    AimDirection = Camera.main.transform.forward;
                    TriggerAttack(_bufferedAttack);
                    _bufferedAttack = AttackType.None; // 버퍼를 비워 중복 실행을 방지합니다.
                }
            }
        }

        public virtual void Exit()
        {
            // 애니메이터 파라미터 잔여값 초기화
            _animController.ResetMoveParameters();
            _animController.SetComboStack(0);
            // 상태를 나갈 때 모든 변수 초기화
            _comboIndex = 0;
            _isInputWindowOpen = false;
            _isAttackDelay = false;
            _bufferedAttack = AttackType.None;
        }

        /// <summary>
        /// 새로운 입력이 들어오면 입력 버퍼에 덮어씌웁니다. 
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
            else if (_comboIndex >= 1 && _input.IsSwapNextWeaponPressed)
            {
                // 변환 공격이 가능한지 데이터 확인
                int swapIndex = _comboIndex;
                if (swapIndex < _stats.CurrentWeaponData.swapAttacks.Length && _stats.CurrentWeaponData.swapAttacks[swapIndex] != null)
                {
                    _bufferedAttack = AttackType.SwapAttack;
                    return; // 변환 공격이 입력되면 다른 입력은 무시
                }
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
            _currentAttackType = attackType;
            _currentSwapAttackData = null;

            _animController.SetComboStack(_comboIndex);

            // 콤보의 첫 공격인지, 이어지는 공격인지에 따라 다른 트리거를 사용합니다.
            if (_comboIndex == 1)
            {
                // 1타: 현재 장착된 무기의 WeaponType에 따라 다른 시작 트리거를 활성화합니다.
                // _stats.CurrentWeaponData.weaponType으로 확인합니다.
                switch (_stats.CurrentWeaponData.weaponType)
                {
                    case WeaponType.Whip:
                        _animController.StartWhipAttack();
                        break;
                    case WeaponType.RayGun:
                        _animController.StartRaygunAttack();
                        break;
                        // TODO: 나중에 다른 무기가 추가되면 여기에 case를 추가합니다.
                        // case WeaponType.Dagger:
                        //     _animController.StartDaggerAttack();
                        //     break;
                }
            }
            else
            {
                // 2타 이상: 약/강 공격 타입에 맞는 트리거를 활성화합니다.
                if (attackType == AttackType.WeakAttack) // 약공격
                {
                    _animController.PlayWeakAttack();
                    Debug.Log("PlayWeakAttack");
                }
                else if (attackType == AttackType.StrongAttack) // 강공격
                {
                    _animController.PlayStrongAttack();
                    Debug.Log("PlayStrongAttack");
                }
                else if (attackType == AttackType.SwapAttack)
                {
                    // 1. 실행할 변환 공격 데이터를 WeaponData에서 찾아 저장
                    int swapIndex = _comboIndex - 1;
                    _currentSwapAttackData = _stats.CurrentWeaponData.swapAttacks[swapIndex];

                    // 2. 변환 애니메이션 재생
                    switch (_stats.CurrentWeaponData.weaponType)
                    {
                        case WeaponType.Whip: _animController.PlayWhipSwapAttack(); break;
                        case WeaponType.RayGun: _animController.PlayRaygunSwapAttack(); break;
                    }

                    // 2. 실제 무기 데이터를 교체
                    _stats.ChangeNextWeapon();
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
                _animController.NoInput();
                _stateMachine.ChangeState(_player.IdleState);
            }
        }

        /// <summary>
        /// (PlayerAnimationEvents에서 호출) 현재 콤보에 맞는 모든 공격 효과를 실행합니다.
        /// </summary>
        public void ExecuteAttackEffects()
        {
            int currentSkillIndex = _comboIndex - 1;
            if (currentSkillIndex < 0) return;

            // 현재 공격 타입에 따라 사용할 스킬 배열을 선택
            SkillData[] skillArray = null;
            switch (_currentAttackType)
            {
                case AttackType.WeakAttack:
                    skillArray = _stats.CurrentWeaponData.weakAttackSkills;
                    break;
                case AttackType.StrongAttack:
                    skillArray = _stats.CurrentWeaponData.strongAttackSkills;
                    break;
            }

            // 선택된 배열에서 현재 인덱스의 스킬을 가져옴
            if (skillArray != null && currentSkillIndex < skillArray.Length)
            {
                SkillData currentSkill = skillArray[currentSkillIndex];
                if (currentSkill == null || currentSkill.effects == null) return;

                var hitboxProvider = _player.GetComponentInChildren<PlayerAnimationEvents>();
                if (hitboxProvider == null) return;

                // 해당 스킬의 모든 효과를 실행
                foreach (var effect in currentSkill.effects)
                {
                    if (effect != null)
                    {
                        effect.Execute(_player, hitboxProvider, this);
                    }
                }
            }
        }

        /// <summary>
        /// (AnimationEvent) 변환 공격의 Pre-Swap 효과들을 실행합니다.
        /// </summary>
        public void ExecutePreSwapEffects()
        {
            if (_currentSwapAttackData == null) return;
            ExecuteEffects(_currentSwapAttackData.preSwapEffects);
        }

        /// <summary>
        /// (AnimationEvent) 실제 무기를 교체합니다.
        /// </summary>
        public void PerformSwap()
        {
            if (_currentSwapAttackData == null) return;
            _stats.ChangeWeapon(_currentSwapAttackData.targetWeaponType);
        }

        /// <summary>
        /// (AnimationEvent) 변환 공격의 Post-Swap 효과들을 실행합니다.
        /// </summary>
        public void ExecutePostSwapEffects()
        {
            if (_currentSwapAttackData == null) return;
            ExecuteEffects(_currentSwapAttackData.postSwapEffects);
        }

        /// <summary>
        /// AttackEffect 배열을 받아 실행하는 공통 헬퍼 메서드
        /// </summary>
        private void ExecuteEffects(AttackEffect[] effects)
        {
            if (effects == null) return;

            var hitboxProvider = _player.GetComponentInChildren<PlayerAnimationEvents>();
            if (hitboxProvider == null) return;

            foreach (var effect in effects)
            {
                if (effect != null)
                {
                    effect.Execute(_player, hitboxProvider, this);
                }
            }
        }
    }
}