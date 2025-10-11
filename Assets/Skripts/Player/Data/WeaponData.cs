// Assets/Skripts/Player/Data/WeaponData.cs
using UnityEngine;

namespace Player.Data
{
    /// <summary>
    /// 무기 하나의 모든 데이터를 통합 관리하는 ScriptableObject입니다.
    /// </summary>
    [CreateAssetMenu(fileName = "NewWeaponData", menuName = "Data/Weapon Data")]
    public class WeaponData : ScriptableObject
    {
        [Header("Weapon Info")]
        [Tooltip("무기의 종류를 구분하기 위한 Enum 값입니다.")]
        public WeaponType weaponType;

        [Header("Combat Settings")]
        [Tooltip("근접 공격(채찍 등)의 콤보별 스킬 데이터 배열입니다.")]
        public SkillData[] weakAttackSkills;

        // [삭제할 곳] public ProjectileData projectileData;

        // TODO: 강공격 스킬, 무기 모델 프리팹, 전용 애니메이션 오버라이드 등의 데이터를 여기에 추가할 수 있습니다.
    }
}