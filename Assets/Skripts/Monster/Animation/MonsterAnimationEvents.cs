// Assets/Scripts/Monster/Animation/MonsterAnimationEvents.cs
using Monster.Combat;
using Monster.Data;
using Monster.Manager;
using UnityEngine;

namespace Monster.Animation
{
    /// <summary>
    /// 몬스터 애니메이션 클립의 이벤트를 수신하고 Monster.cs로 전달하는 역할을 합니다.
    /// </summary>
    public class MonsterAnimationEvents : MonoBehaviour
    {
        [Header("스킬 데이터")]
        [Tooltip("이 몬스터가 사용할 스킬 에셋들을 여기에 등록합니다. (0번 = 일반공격, 1번 = 특수공격 등)")]
        [SerializeField] private MonsterSkillData[] _skills;

        [Header("공격 판정 참조")]
        [Tooltip("몬스터의 근접 공격 판정을 위한 히트박스들입니다.")]
        [SerializeField] private MonsterHitbox[] _hitboxes;

        [Tooltip("투사체가 발사될 위치(Muzzle)입니다.")]
        [SerializeField] private Transform _muzzle;

        private Monster _monster;

        private void Awake()
        {
            // 부모 오브젝트에서 Monster 컴포넌트를 찾아 할당합니다.
            _monster = GetComponentInParent<Monster>();
        }

        /// <summary>
        /// (애니메이션 이벤트에서 호출) 지정된 인덱스의 스킬을 실행합니다.
        /// </summary>
        public void ExecuteSkill(int skillIndex)
        {
            if (skillIndex < 0 || skillIndex >= _skills.Length)
            {
                Debug.Log($"{skillIndex} 번째 인덱스에 히트박스가 없습니다.");
                return;
            }
            _skills[skillIndex]?.Execute(_monster, this);
        }


        /// <summary>
        /// 지정된 인덱스의 히트박스를 활성화합니다.
        /// </summary>
        public void ActivateHitbox(int index, MonsterSkillData skill, float duration)
        {
            if (index < 0 || index >= _hitboxes.Length) return;
            _hitboxes[index].Activate(skill, _monster, duration);
        }

        /// <summary>
        /// 투사체를 발사합니다.
        /// </summary>
        public void FireProjectile(MonsterSkillData skill, string tag)
        {
            GameObject projectileObj = MonsterPoolManager.Instance.GetFromPool(tag);
            if (projectileObj == null) return;

            projectileObj.transform.position = _muzzle.position;

            // 몬스터가 플레이어를 바라보는 방향으로 발사
            Vector3 direction = (_monster.target.position - _muzzle.position).normalized;

            if (projectileObj.TryGetComponent<MonsterProjectile>(out var projectile))
            {
                projectile.Initialize(skill, _monster, direction);
                projectileObj.SetActive(true);
            }
        }

        /// <summary>
        /// (애니메이션 이벤트) 공격 애니메이션이 끝났음을 알립니다.
        /// </summary>
        public void OnAttackFinished()
        {
            _monster.OnAttackFinished();
        }
    }
}