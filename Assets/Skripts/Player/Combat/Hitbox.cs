// Assets/Scripts/Player/Combat/Hitbox.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player.Combat
{
    /// <summary>
    /// 재사용 가능한 피격 판정 스크립트입니다.
    /// 정해진 시간 동안 활성화되어 'Monster' 태그를 가진 대상을 공격합니다.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class Hitbox : MonoBehaviour
    {
        private Collider _collider;
        private float _damage;
        private float _knockbackForce;
        private float _lifeTimer; // 남은 활성화 시간을 체크할 타이머
        private List<Collider> _hitTargets = new List<Collider>();
        private Transform _attacker; // 공격자(플레이어)의 Transform

        private void Awake()
        {
            _collider = GetComponent<Collider>();
        }

        /// <summary>
        /// 매 프레임 호출되어 활성화 시간을 체크합니다.
        /// </summary>
        private void Update()
        {
            // 타이머가 0 이하면 로직을 실행하지 않음 (비활성화 상태)
            if (_lifeTimer <= 0f)
            {
                return;
            }

            // 매 프레임 타이머 감소
            _lifeTimer -= Time.deltaTime;

            // 타이머가 다 되면 스스로 비활성화
            if (_lifeTimer <= 0f)
            {
                gameObject.SetActive(false);
            }
        }
        /// <summary>
        /// 지정된 데미지와 지속 시간으로 히트박스를 활성화합니다.
        /// </summary>
        public void Activate(float damage, float knockbackForce, float duration, Transform attacker)
        {
            _damage = damage;
            _knockbackForce = knockbackForce;
            _lifeTimer = duration;
            _attacker = attacker;
            _hitTargets.Clear();
            gameObject.SetActive(true);
        }

        /// <summary>
        /// 다른 콜라이더가 이 히트박스의 트리거 영역에 들어왔을 때 호출됩니다.
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            // 'Monster' 태그를 가졌는지, 그리고 이전에 공격한 대상이 아닌지 확인
            if (other.CompareTag("Monster") && !_hitTargets.Contains(other))
            {
                // Monster 스크립트를 가져와서 데미지를 입힘
                if (other.TryGetComponent<Monster.Monster>(out var monster))
                {
                    Vector3 knockbackDirection = (other.transform.position - _attacker.position).normalized;
                    monster.TakeDamage(_damage, knockbackDirection, _knockbackForce);
                    _hitTargets.Add(other);
                }
            }
        }
    }
}