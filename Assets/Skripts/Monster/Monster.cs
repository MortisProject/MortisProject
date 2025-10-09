// Assets/Scripts/World/Monster.cs
using UnityEngine;

namespace World
{
    /// <summary>
    /// 테스트용 몬스터 스크립트입니다. 체력을 가지고 있으며, 피해를 받을 수 있습니다.
    /// </summary>
    public class Monster : MonoBehaviour
    {
        [Header("Stats")]
        [Tooltip("몬스터의 현재 체력입니다.")]
        [SerializeField] private float health = 100f;

        /// <summary>
        /// 지정된 양의 데미지를 받아 체력을 감소시킵니다.
        /// </summary>
        /// <param name="damage">입을 데미지의 양</param>
        public void TakeDamage(float damage)
        {
            health -= damage;
            Debug.Log($"{gameObject.name}이(가) {damage}의 피해를 입었습니다! 현재 체력: {health}");

            if (health <= 0)
            {
                Die();
            }
        }

        /// <summary>
        /// 몬스터가 죽었을 때의 처리를 담당합니다.
        /// </summary>
        private void Die()
        {
            Debug.Log($"{gameObject.name}이(가) 처치되었습니다.");
            // TODO: 몬스터 사망 시 파괴, 아이템 드랍, 점수 증가 등의 로직을 여기에 추가합니다.
            Destroy(gameObject);
        }
    }
}