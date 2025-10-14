// Assets/Skripts/Player/Data/ProjectileData.cs
using UnityEngine;

namespace Player.Data
{
    /// <summary>
    /// 발사체의 모든 데이터를 정의하는 ScriptableObject입니다.
    /// </summary>
    [CreateAssetMenu(fileName = "NewProjectileData", menuName = "Data/Projectile Data")]
    public class ProjectileData : ScriptableObject
    {
        [Header("Projectile Stats")]
        [Tooltip("발사체의 속도입니다.")]
        public float projectileSpeed = 50f;

        [Tooltip("발사체의 관통 여부입니다.")]
        public bool isPenetration = false;

        [Tooltip("발사체의 최대 수명(활성화 시간)입니다.")]
        public float projectileLifeTime = 3f;

        [Header("Effects")]
        [Tooltip("몬스터나 지형에 부딪혔을 때 생성될 시각 효과(VFX) 프리팹입니다.")]
        public GameObject impactVFXPrefab;

        // TODO: 충돌 시 사운드(SFX) 등의 데이터를 여기에 추가할 수 있습니다.
    }
}