// Assets/Skripts/Player/Data/ProjectileAttackEffect.cs
using Player.Animation;
using Player.Combat;
using Player.States;
using UnityEngine;
using World;

namespace Player.Data
{
    /// <summary>
    /// 투사체 발사를 정의하는 AttackEffect입니다.
    /// </summary>
    [CreateAssetMenu(fileName = "NewProjectileAttackEffect", menuName = "Data/Attack Effect/Projectile Attack")]
    public class ProjectileAttackEffect : AttackEffect
    {
        [Header("투사체 설정")]
        [Tooltip("발사할 투사체의 데이터입니다.")]
        public ProjectileData projectileData;

        [Tooltip("ProjectilePoolManager에서 사용할 투사체의 태그입니다.")]
        public string projectilePoolTag = "RaygunProjectile";

        // TODO: 여러 총구에서 발사할 경우를 대비해 muzzleIndex를 추가할 수 있습니다.

        public override void Execute(Player performer, PlayerAnimationEvents hitboxProvider, PlayerAttackState attackState)
        {
            if (projectileData == null)
            {
                Debug.LogWarning("ProjectileData가 설정되지 않았습니다.");
                return;
            }

            // 1. 발사체 풀에서 발사체를 가져옵니다.
            GameObject projectileObject = ProjectilePoolManager.Instance.GetFromPool(projectilePoolTag);
            if (projectileObject == null) return;

            // 2. 발사 위치와 방향을 설정합니다.
            Transform muzzle = performer.WireOrigin; // TODO: 실제 총구 Transform 참조로 변경 필요
            Vector3 fireDirection = attackState.AimDirection;

            projectileObject.transform.position = muzzle.position;
            projectileObject.transform.rotation = Quaternion.LookRotation(fireDirection);

            // 3. 발사체를 초기화하고 발사합니다.
            if (projectileObject.TryGetComponent<Projectile>(out Projectile projectile))
            {
                // Projectile의 Initialize 메서드는 기본 공격력만 받도록 수정될 예정입니다.
                projectile.Initialize(projectilePoolTag, fireDirection, performer.Stats.attackValue, projectileData);
                projectileObject.SetActive(true);
            }
        }
    }
}