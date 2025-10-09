// Assets/Scripts/Player/Combat/Hitbox.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using World; // Monster 스크립트를 사용하기 위해 추가

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

        // 한 번의 활성화 동안 이미 공격한 대상을 저장하여 중복 피격을 방지합니다.
        private List<Collider> _hitTargets = new List<Collider>();

        private void Awake()
        {
            _collider = GetComponent<Collider>();
            _collider.isTrigger = true; // 코드로 IsTrigger를 확실하게 보장
            gameObject.SetActive(false); // 게임 시작 시에는 항상 비활성화 상태로
        }

        /// <summary>
        /// 지정된 데미지와 지속 시간으로 히트박스를 활성화합니다.
        /// </summary>
        /// <param name="damage">적용할 데미지</param>
        /// <param name="duration">활성화될 시간 (초)</param>
        public void Activate(float damage, float duration)
        {
            _damage = damage;
            _hitTargets.Clear(); // 새로운 공격이므로 이전에 맞춘 타겟 목록 초기화
            gameObject.SetActive(true);

            // 지정된 시간이 지나면 자동으로 비활성화하는 코루틴 시작
            StartCoroutine(DeactivateAfter(duration));
        }

        private IEnumerator DeactivateAfter(float duration)
        {
            yield return new WaitForSeconds(duration);
            gameObject.SetActive(false);
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
                if (other.TryGetComponent<Monster>(out Monster monster))
                {
                    monster.TakeDamage(_damage);
                    _hitTargets.Add(other); // 공격한 대상으로 추가하여 중복 피격 방지
                }
            }
        }
    }
}