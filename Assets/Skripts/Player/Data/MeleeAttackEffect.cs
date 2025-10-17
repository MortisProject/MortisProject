// Assets/Skripts/Player/Data/MeleeAttackEffect.cs
using Player.Animation;
using Player.Combat;
using Player.States;
using System.Linq;
using UnityEngine;

namespace Player.Data
{
    /// <summary>
    /// 근접 공격(히트박스 활성화)을 정의하는 AttackEffect입니다.
    /// </summary>
    [CreateAssetMenu(fileName = "NewMeleeAttackEffect", menuName = "Data/Attack Effect/Melee Attack")]
    public class MeleeAttackEffect : AttackEffect
    {
        [Header("근접 공격 설정")]
        [Tooltip("사용할 히트박스 그룹의 태그입니다. PlayerAnimationEvents에 설정된 태그와 일치해야 합니다.")]
        public string groupTag = "Weak";

        [Tooltip("선택된 그룹 내에서 활성화할 히트박스의 인덱스입니다.")]
        public int hitboxIndex;

        [Tooltip("히트박스가 활성화될 시간(초)입니다.")]
        public float duration = 0.2f;

        public override void Execute(Player performer, PlayerAnimationEvents hitboxProvider, PlayerAttackState attackState)
        {
            // 1. 최종 데미지를 계산합니다.
            float baseDamage = performer.Stats.attackValue;
            float finalDamage = baseDamage * (damageMultiplier / 100f);

            // 2. 현재 무기 타입에 맞는 히트박스 배열을 가져옵니다.
            PlayerAnimationEvents.HitboxGroup[] sourceGroups = null;
            switch (performer.Stats.CurrentWeaponData.weaponType)
            {
                case WeaponType.Whip:
                    sourceGroups = hitboxProvider.whipHitboxGroups;
                    break;
                    // TODO: 다른 근접 무기 타입이 추가되면 여기에 case를 추가합니다.
            }
            if (sourceGroups == null) return;

            // 3. 올바른 히트박스를 찾아 활성화합니다.
            var targetGroup = sourceGroups.FirstOrDefault(g => g.tag == groupTag);

            if (targetGroup != null && hitboxIndex < targetGroup.hitboxes.Length)
            {
                Hitbox targetHitbox = targetGroup.hitboxes[hitboxIndex];
                targetHitbox.Activate(finalDamage, knockbackForce, duration, performer.transform);
            }
            else
            {
                Debug.LogWarning($"MeleeAttackEffect: 태그 '{groupTag}'를 가진 그룹에서 히트박스(인덱스 {hitboxIndex})를 찾을 수 없습니다.");
            }
        }
    }
}