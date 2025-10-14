// Assets/Scripts/Monster/Data/MonsterProjectileSkillEffect.cs
using UnityEngine;

namespace Monster.Data
{
    [CreateAssetMenu(fileName = "New Projectile Skill", menuName = "Monster/Skills/Projectile Skill Effect")]
    public class MonsterProjectileSkillEffect : MonsterSkillData
    {
        [Header("원거리 공격 설정")]
        public string projectileTag;

        // --- 코드 블럭 단위로 제공 (새로 추가된 부분) ---
        [Tooltip("투사체의 속도입니다.")]
        public float projectileSpeed = 20f;
        [Tooltip("투사체의 최대 수명(초)입니다.")]
        public float projectileLifetime = 5f;

        public override void Execute(Monster performer, Animation.MonsterAnimationEvents eventSource)
        {
            // FireProjectile에 스킬 정보(this)를 함께 넘겨줍니다.
            eventSource.FireProjectile(this, projectileTag);
        }
    }
}