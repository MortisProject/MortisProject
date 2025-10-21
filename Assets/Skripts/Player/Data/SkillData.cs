// Assets/Scripts/Player/Data/SkillData.cs
using UnityEngine;

namespace Player.Data
{
    /// <summary>
    /// 개별 스킬(콤보의 한 단계)을 정의하는 ScriptableObject입니다.
    /// 하나의 스킬은 여러 개의 AttackEffect를 가질 수 있습니다.
    /// </summary>
    [CreateAssetMenu(fileName = "NewSkillData", menuName = "Data/Skill Data")]
    public class SkillData : ScriptableObject
    {
        // --- 코드 블럭 단위로 제공 ---
        [Header("Skill Effects")]
        [Tooltip("이 스킬이 실행될 때 발동할 공격 효과 목록입니다.")]
        public AttackEffect[] effects;
        // --- 여기까지 추가 ---

        [Header("Combat Data")]
        // [삭제할 곳] public int damageMultiplier = 100;
        [Tooltip("스킬 사용 시 생성되는 버스트 게이지 양입니다.")]
        public int burstGeneration = 5;

        // TODO: 스킬 사용 시 소모되는 자원(Ast) 등의 데이터를 추가할 수 있습니다.
    }
}