// Assets/Scripts/Player/Data/SkillData.cs
using System.Collections.Generic;
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
        [Header("Skill Effects")]
        [Tooltip("이 스킬이 실행될 때 발동할 공격 효과 목록입니다.")]
        [SerializeReference]
        public List<AttackEffect> effects = new List<AttackEffect>();

        [Tooltip("스킬 사용 시 생성되는 버스트 게이지 양입니다.")]
        public int burstGeneration = 5;

        // TODO: 스킬 사용 시 소모되는 자원(Ast) 등의 데이터를 추가할 수 있습니다.
    }
}