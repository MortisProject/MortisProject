// Assets/Scripts/Monster/Data/MonsterSkillData.cs
using UnityEngine;

namespace Monster.Data
{
    /// <summary>
    /// 모든 몬스터 스킬 데이터의 기반이 되는 추상 ScriptableObject입니다.
    /// </summary>
    public abstract class MonsterSkillData : ScriptableObject
    {
        [Header("스킬 공통 설정")]
        [Tooltip("일반(흰색), 회피(파란색), 가드(노란색) 공격 타입을 설정합니다.")]
        public AttackType attackType = AttackType.Normal;

        [Tooltip("몬스터의 기본 공격력에 곱해질 데미지 배율입니다. (예: 150 -> 150%)")]
        [Range(0, 500)]
        public float damageMultiplier = 100f;

        //[Tooltip("피격 시 플레이어에게 적용될 넉백의 종류입니다.")]
        //public KnockbackType knockbackType = KnockbackType.Hit;

        /// <summary>
        /// 이 스킬의 실제 로직을 실행합니다.
        /// </summary>
        /// <param name="performer">스킬을 사용하는 몬스터</param>
        /// <param name="eventSource">애니메이션 이벤트를 수신한 컴포넌트</param>
        public abstract void Execute(Monster performer, Animation.MonsterAnimationEvents eventSource);

        // --- 열거형 정의 ---
        public enum AttackType { Normal, Blue, Yellow }
        
        //히트와 넉백은 기획서상 나누지 않았으나
        //나중에 필요성이 있으면 사용할 수 있도록 주석 처리함
        //public enum KnockbackType { Hit, Knockback }
    }
}