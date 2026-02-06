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
        [Tooltip("약공격 콤보에 사용할 스킬 데이터 배열입니다.")]
        public SkillData[] weakAttackSkills;

        [Tooltip("강공격 콤보에 사용할 스킬 데이터 배열입니다.")]
        public SkillData[] strongAttackSkills;

        [Tooltip("변환 공격에 사용할 데이터 배열입니다. (콤보 순서에 맞게 등록)")]
        public SwapAttackData[] swapAttacks;

        [Header("Burst Skill")]
        [Tooltip("이 무기의 버스트 스킬 데이터입니다.")]
        public SkillData burstSkill;
    }
}