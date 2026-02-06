// Assets/Skripts/Player/Combat/Projectile.cs
using Player.Data;
using UnityEngine;
using World;

namespace Player.Combat
{
    /// <summary>
    /// 발사체의 이동, 충돌 처리, 소멸 로직을 담당
    /// 이 스크립트는 발사체 프리팹에 부착되어야함
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(SphereCollider))]
    public class Projectile : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("디버깅용: 현재 발사체에 적용된 데이터")]
        [SerializeField] private ProjectileData _data;

        private PlayerStateMachine _stateMachine;
        private Rigidbody _rigidbody;
        private float _finalDamage;
        private float _knockbackForce;
        private float _lifeTimeTimer;
        private string _poolTag;
        private Transform _attacker;
        private bool _isKnockback; 

        // 한 번의 활성화 동안 이미 공격한 대상을 저장하여 중복 피격을 방지 (관통탄을 위함)
        private System.Collections.Generic.List<Collider> _hitTargets = new System.Collections.Generic.List<Collider>();

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            GetComponent<SphereCollider>().isTrigger = true; // 충돌 감지를 위해 Trigger로 설정
            _rigidbody.useGravity = false; // 중력의 영향을 받지 않도록 설정
        }

        private void Update()
        {
            _lifeTimeTimer -= Time.deltaTime;
            if (_lifeTimeTimer <= 0f)
            {
                // 수명이 다하면 풀에 반납
                ProjectilePoolManager.Instance.ReturnToPool(_poolTag, gameObject);
            }
        }

        /// <summary>
        /// 발사체가 활성화될 때 외부에서 호출하여 초기 설정을 수행
        /// </summary>
        /// <param name="initialDirection">발사될 방향</param>
        /// <param name="baseDamage">플레이어의 기본 공격력</param>
        /// <param name="data">발사체의 모든 속성을 담은 ScriptableObject</param>
        public void Initialize(PlayerStateMachine stateMachine, Transform attacker, string poolTag, Vector3 initialDirection, float finalDamage, float knockbackForce, ProjectileData data, bool isKnockback)
        {
            _stateMachine = stateMachine;
            _attacker = attacker;
            _poolTag = poolTag;
            _finalDamage = finalDamage;
            _knockbackForce = knockbackForce;
            _data = data;
            _lifeTimeTimer = _data.projectileLifeTime;
            _isKnockback = isKnockback;
            _hitTargets.Clear();

            _rigidbody.linearVelocity = initialDirection.normalized * _data.projectileSpeed;
        }

        private void OnTriggerEnter(Collider other)
        {
            // 이미 공격한 대상은 무시합니다.
            if (_hitTargets.Contains(other)) return;

            // 'Monster' 태그를 가진 대상과 충돌했는지 확인
            if (other.CompareTag("Monster"))
            {
                if (other.TryGetComponent<Monster.Monster>(out var monster))
                {
                    // 최종 데미지 계산 (기본 데미지 * 발사체 데미지 배율)
                    Vector3 knockbackDirection = (other.transform.position - _attacker.position).normalized;
                    monster.TakeDamage(_finalDamage, knockbackDirection, _knockbackForce, _isKnockback);
                    _hitTargets.Add(other);

                    HandleImpact(other.ClosestPoint(transform.position));
                }
            }
            else
            {
                HandleImpact(other.ClosestPoint(transform.position));
            }
        }

        /// <summary>
        /// 충돌 시 공통 로직을 처리 (이펙트 생성, 오브젝트 비활성화 등)
        /// </summary>
        private void HandleImpact(Vector3 impactPoint)
        {
            if (_data.impactVFXPrefab != null)
            {
                var effect = Instantiate(_data.impactVFXPrefab, impactPoint, Quaternion.identity);
            }

            if (!_data.isPenetration)
            {
                // 관통탄이 아니면 풀에 반납
                ProjectilePoolManager.Instance.ReturnToPool(_poolTag, gameObject);
            }
        }
    }
}