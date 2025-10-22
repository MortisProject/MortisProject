// Assets/Skripts/Player/Data/AttackEffect.cs
using Player.Animation;
using Player.Combat;
using Player.States;
using UnityEngine;

namespace Player.Data
{
    /// <summary>
    /// 모든 공격 효과의 기반이 되는 추상 ScriptableObject입니다.
    /// 각 효과는 Execute 메서드를 통해 자신만의 로직을 실행합니다.
    /// </summary>
    public abstract class AttackEffect : ScriptableObject
    {
        [Header("공통 데이터")]
        [Tooltip("스킬의 데미지 배율입니다. (예: 120% -> 120)")]
        public int damageMultiplier = 100;

        [Tooltip("이 공격이 몬스터에게 가하는 넉백의 힘입니다. 0이면 넉백이 발생하지 않습니다.")]
        [Range(0f, 50f)]
        public float knockbackForce = 5f;
        // TODO: 여기에 모든 공격 효과가 공통으로 사용할 데이터(사운드, 이펙트 등)를 추가할 수 있습니다.

        /// <summary>
        /// 이 공격 효과의 실제 로직을 실행합니다.
        /// </summary>
        /// <param name="performer">공격을 실행하는 주체 (Player)</param>
        /// <param name="hitboxProvider">히트박스/총구 등 위치 정보를 제공하는 컴포넌트</param>
        /// <param name="attackState">현재 공격 상태</param>
        public abstract void Execute(Player performer, PlayerAnimationEvents hitboxProvider, IState sourceState);
    }
}