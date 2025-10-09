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
        [Header("Skill Identifier")]
        [Tooltip("이 스킬의 고유 이름입니다. (예: Whip_Weak_01)")]
        public string skillName;

        [Tooltip("이 스킬이 속한 무기 타입입니다.")]
        public WeaponType weaponType;

        [Header("Combat Data")]
        [Tooltip("스킬의 데미지 배율입니다. (예: 120% -> 120)")]
        public int skillDamagePercentage = 100;

        [Tooltip("스킬 사용 시 생성되는 버스트 게이지 양입니다.")]
        public int burstGeneration = 5;

        [Header("Timing Data")]
        [Tooltip("스킬의 전체 모션이 재생되는 시간입니다. (단위: 초)")]
        public float motionTime = 0.5f;

        [Tooltip("콤보 유예 시간입니다. 이 시간 안에 다음 입력을 해야 콤보가 이어집니다.")]
        public float comboGraceTime = 0.6f;

        [Header("Animation")]
        [Tooltip("이 스킬에 해당하는 애니메이션 클립 이름 또는 해시값입니다. (참고용)")]
        public string animationName;

        // TODO: 기획서의 '스킬 상태 변화'에 따라 경직, 넉백 등의 효과를 추가할 수 있습니다.
        // public float stiffDuration = 0.2f;
    }
}