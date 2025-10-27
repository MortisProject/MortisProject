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

        [Header("발사 위치 설정")]
        [Tooltip("사용할 발사 위치의 인덱스입니다. 0: 오른손, 1: 왼손")]
        public int muzzleIndex = 0;
        // TODO: 여러 총구에서 발사할 경우를 대비해 muzzleIndex를 추가할 수 있습니다.

        public override void Execute(Player performer, PlayerAnimationEvents hitboxProvider, IState sourceState)
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
            if (muzzleIndex < 0 || muzzleIndex >= hitboxProvider.muzzles.Length)
            {
                Debug.LogWarning($"Muzzle Index ({muzzleIndex})가 유효하지 않습니다.");
                return;
            }
            Transform muzzle = hitboxProvider.muzzles[muzzleIndex];

            // 발사 방향을 현재 상태(State)에서 가져옵니다.
            Vector3 fireDirection;
            if (sourceState is PlayerAttackState attackState)
            {
                fireDirection = attackState.AimDirection;
            }
            else if (sourceState is PlayerBurstSkillState burstState)
            {
                fireDirection = burstState.GetAimDirection(); // BurstState에서 AimDirection을 가져옴
            }
            else
            {
                // 예외 처리: 조준 방향을 알 수 없으면 카메라 정면을 사용
                fireDirection = Camera.main.transform.forward;
            }

            projectileObject.transform.position = muzzle.position;
            projectileObject.transform.rotation = Quaternion.LookRotation(fireDirection);

            // 3. 발사체를 초기화하고 발사합니다.
            if (projectileObject.TryGetComponent<Projectile>(out Projectile projectile))
            {
                // 최종 데미지를 여기서 계산합니다.
                float finalDamage = performer.Stats.attackValue * (damageMultiplier / 100f);

                // 계산된 최종 데미지를 Projectile에 직접 전달합니다.
                projectile.Initialize(performer.StateMachine, performer.transform, projectilePoolTag, fireDirection, finalDamage, knockbackForce, projectileData, isKnockback); 
                projectileObject.SetActive(true);
            }
        }
    }
}