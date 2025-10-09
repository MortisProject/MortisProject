// Assets/Scripts/Player/Data/SkillData.cs
using UnityEngine;

namespace Player.Data
{
    /// <summary>
    /// 개별 스킬의 모든 데이터를 담는 ScriptableObject입니다.
    /// 이 에셋을 통해 기획자는 각 스킬의 밸런스를 쉽게 조절할 수 있습니다.
    /// </summary>
    [CreateAssetMenu(fileName = "NewSkillData", menuName = "Data/Skill Data")]
    public class SkillData : ScriptableObject
    {
        [Header("Combat Data")]
        [Tooltip("스킬의 데미지 배율입니다. (예: 120% -> 120)")]
        public int damageMultiplier = 100;

        [Tooltip("스킬 사용 시 생성되는 버스트 게이지 양입니다.")]
        public int burstGeneration = 5;
    }
}