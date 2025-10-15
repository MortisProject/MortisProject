// Assets/Scripts/Monster/Combat/MonsterProjectile.cs
using Player;
using Monster.Data;
using Monster.Manager;
using UnityEngine;

namespace Monster.Combat
{
    [RequireComponent(typeof(Rigidbody), typeof(Collider))]
    public class MonsterProjectile : MonoBehaviour
    {
        private Rigidbody _rigidbody;
        private MonsterSkillData _skillData;
        private Monster _performer;
        private float _lifeTimer;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            GetComponent<Collider>().isTrigger = true;
            _rigidbody.useGravity = false;
        }

        private void Update()
        {
            _lifeTimer -= Time.deltaTime;
            if (_lifeTimer <= 0f)
            {
                // 수명이 다하면 스스로 풀에 반납
                ReturnToPool();
            }
        }

        /// <summary>
        /// 투사체를 초기화하고 발사합니다.
        /// </summary>
        public void Initialize(MonsterSkillData skill, Monster performer, Vector3 direction)
        {
            _skillData = skill;
            _performer = performer;

            // Projectile 스킬 데이터로 형변환하여 속도와 수명을 가져옵니다.
            if (skill is MonsterProjectileSkillEffect projectileSkill)
            {
                _lifeTimer = projectileSkill.projectileLifetime;
                _rigidbody.linearVelocity = direction * projectileSkill.projectileSpeed;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // 플레이어와 충돌했는지 확인합니다.
            if (other.CompareTag("Player"))
            {
                if (other.TryGetComponent<CharacterStats>(out var playerStats))
                {
                    float finalDamage = _performer.Data.attackValue * (_skillData.damageMultiplier / 100f);
                    playerStats.TakeDamage(finalDamage, _skillData.attackType);
                    //넉백타입이 제거되지 않은 코드
                    //playerStats.TakeDamage(finalDamage, _skillData.attackType, _skillData.knockbackType);
                }

                // 충돌 시 즉시 풀에 반납
                ReturnToPool();
            }
            // TODO: 벽이나 다른 장애물에 부딪혔을 때의 처리
        }

        private void ReturnToPool()
        {
            // MonsterPoolManager를 통해 자신을 반납합니다.
            string tag = (_skillData as MonsterProjectileSkillEffect)?.projectileTag;
            if (!string.IsNullOrEmpty(tag))
            {
                MonsterPoolManager.Instance.ReturnToPool(tag, gameObject);
            }
            else
            {
                // 태그가 없다면 그냥 비활성화
                gameObject.SetActive(false);
            }
        }
    }
}