// Assets/Scripts/Player/States/PlayerBurstSkillState.cs
using Player.Animation;
using Player.Data;
using UnityEngine;

namespace Player.States
{
    /// <summary>
    /// 버스트 스킬을 사용하는 상태입니다.
    /// 스킬 시전 중에는 경직 면역이며, 애니메이션이 끝나면 자동으로 종료됩니다.
    /// </summary>
    public class PlayerBurstSkillState : IState
    {
        private readonly Player _player;
        private readonly PlayerStateMachine _stateMachine;
        private readonly CharacterStats _stats;
        private readonly PlayerAnimationController _animController;

        // 스킬 시전 시점의 조준 방향 (투사체 스킬을 위함)
        private Vector3 _aimDirection;

        public PlayerBurstSkillState(Player player, PlayerStateMachine stateMachine, CharacterStats stats, PlayerAnimationController animController)
        {
            _player = player;
            _stateMachine = stateMachine;
            _stats = stats;
            _animController = animController;
        }

        public void Enter()
        {
            Debug.Log("버스트 스킬 상태 진입!");

            // 1. 버스트 게이지를 전부 소모합니다.
            _stats.ConsumeBust(_stats.maxBurst);

            // 2. 현재 카메라 방향을 조준 방향으로 저장합니다. (투사체 등에 사용)
            _aimDirection = Camera.main.transform.forward;

            // 3. 스킬 시전 중 경직에 걸리지 않도록 무적(경직 면역) 상태로 만듭니다.
            _player.Stats.StartStunImmunity();
            // TODO: 기획에 따라 완전 무적이 필요하다면 CharacterStats에 별도 플래그(_isInvincible) 추가 필요

            // 4. 무기 모델을 활성화합니다.
            _stats.ActivateCurrentWeaponModel();

            // 5. (핵심) 현재 무기 타입에 맞는 버스트 스킬 애니메이션을 재생합니다.
            switch (_stats.CurrentWeaponData.weaponType)
            {
                case WeaponType.Whip:
                    _animController.PlayWhipBurst();
                    break;
                case WeaponType.RayGun:
                    _animController.PlayRaygunBurst();
                    break;
                    // TODO: 다른 무기 타입이 추가되면 여기에 case를 추가합니다.
            }
        }

        public void Update()
        {
            // 이 상태는 애니메이션 이벤트에 의해 제어되므로 Update에서는 할 일이 없습니다.
        }

        public void Exit()
        {
            // 스킬이 끝나면 무기 모델을 다시 비활성화합니다.
            _stats.DeactivateAllWeaponModels();
        }

        /// <summary>
        /// (애니메이션 이벤트에서 호출될) 애니메이션이 끝났을 때 Idle 상태로 전환합니다.
        /// </summary>
        public void OnAnimationFinished()
        {
            // 버스트 스킬은 땅에서만 발동되므로, 끝나면 항상 IdleState로 돌아갑니다.
            _stateMachine.ChangeState(_player.IdleState);
        }

        /// <summary>
        /// (PlayerAnimationEvents가 참조할) 이 상태의 조준 방향을 반환합니다.
        /// </summary>
        public Vector3 GetAimDirection()
        {
            return _aimDirection;
        }
    }
}