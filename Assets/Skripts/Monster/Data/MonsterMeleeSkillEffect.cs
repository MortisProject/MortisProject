// Assets/Scripts/Monster/Data/MonsterMeleeSkillEffect.cs
using UnityEngine;

namespace Monster.Data
{
    [CreateAssetMenu(fileName = "New Melee Skill", menuName = "Monster/Skills/Melee Skill Effect")]
    public class MonsterMeleeSkillEffect : MonsterSkillData
    {
        [Header("근접 공격 설정")]
        [Tooltip("활성화할 히트박스의 인덱스입니다. (MonsterHitboxProvider에 설정된 번호)")]
        public int hitboxIndex;

        [Tooltip("히트박스가 활성화될 시간(초)입니다.")]
        public float duration = 0.3f;

        public override void Execute(Monster performer, Animation.MonsterAnimationEvents eventSource)
        {
            eventSource.ActivateHitbox(hitboxIndex, this, duration);
        }
    }
}