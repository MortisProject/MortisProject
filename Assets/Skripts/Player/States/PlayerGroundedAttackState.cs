// Assets/Scripts/Player/States/PlayerGroundedAttackState.cs
using Player.Animation;
using Player.Data;

namespace Player.States
{
    public class PlayerGroundedAttackState : PlayerAttackState
    {
        // PlayerGroundedAttackState는 PlayerAttackState의 모든 기능을 상속받습니다.
        public PlayerGroundedAttackState(Player player, PlayerStateMachine stateMachine, PlayerInput input, CharacterStats stats, PlayerAnimationController animController)
            : base(player, stateMachine, input, stats, animController)
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

        /// <summary>
        /// (애니메이션 이벤트에서 호출됨) 현재 콤보에 맞는 히트박스를 활성화합니다.
        /// </summary>
        public void ActivateHitbox()
        {
            if (_comboIndex <= 0) return;

            SkillData skillData = null;
            Combat.Hitbox hitbox = null;
            int currentComboStep = _comboIndex - 1; // 배열 인덱스는 0부터 시작하므로

            // 현재 무기와 공격 타입에 따라 올바른 SkillData와 Hitbox를 가져옴
            if (_stats.CurrentWeapon == WeaponType.Whip)
            {
                if (_currentAttackType == AttackType.WeakAttack)
                {
                    skillData = _player.whipWeakAttackCombo[currentComboStep];
                    hitbox = _player.whipWeakAttackHitboxes[currentComboStep];
                }
                else // StrongAttack
                {
                    // skillData = _player.whipStrongAttackCombo[currentComboStep];
                    // hitbox = _player.whipStrongAttackHitboxes[currentComboStep];
                }
            }
            // else if (_stats.CurrentWeapon == WeaponType.RayGun) { ... }

            // 유효한 SkillData와 Hitbox가 있다면 활성화
            if (skillData != null && hitbox != null)
            {
                // TODO: 플레이어의 기본 공격력과 스킬의 데미지 배율을 조합하여 최종 데미지를 계산해야 함
                // 여기서는 테스트를 위해 스킬 배율을 그대로 데미지로 사용
                float damage = skillData.skillDamagePercentage;
                float duration = 0.2f; // 히트박스 지속시간 (임시)

                hitbox.Activate(damage, duration);
            }
        }
    }
}