// Assets/Scripts/Monster/Combat/MonsterHitbox.cs
using Player;
using Monster.Data;
using System.Collections.Generic;
using UnityEngine;

namespace Monster.Combat
{
    [RequireComponent(typeof(Collider))]
    public class MonsterHitbox : MonoBehaviour
    {
        private Collider _collider;
        private float _lifeTimer;
        private List<Collider> _hitTargets = new List<Collider>();

        // 스킬 정보를 저장할 변수
        private MonsterSkillData _skillData;
        private Monster _performer;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
        }

        private void Update()
        {
            if (_lifeTimer <= 0f) return;

            _lifeTimer -= Time.deltaTime;

            if (_lifeTimer <= 0f)
            {
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 지정된 스킬 데이터와 지속 시간으로 히트박스를 활성화합니다.
        /// </summary>
        public void Activate(MonsterSkillData skill, Monster performer, float duration)
        {
            _skillData = skill;
            _performer = performer;
            _lifeTimer = duration;
            _hitTargets.Clear();
            gameObject.SetActive(true);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && !_hitTargets.Contains(other))
            {
                if (other.TryGetComponent<CharacterStats>(out var playerStats))
                {
                    // 최종 데미지 계산
                    float finalDamage = _performer.Data.attackValue * (_skillData.damageMultiplier / 100f);

                    // 플레이어에게 데미지와 스킬 정보를 전달
                    playerStats.TakeDamage(finalDamage, _skillData.attackType, _skillData.knockbackType);
                    _hitTargets.Add(other); // 중복 피격 방지
                }
            }
        }
    }
}