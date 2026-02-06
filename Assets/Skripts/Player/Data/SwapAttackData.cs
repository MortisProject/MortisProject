// Assets/Skripts/Player/Data/SwapAttackData.cs
using UnityEngine;

namespace Player.Data
{
    /// <summary>
    /// 변환 공격의 모든 단계를 정의하는 ScriptableObject
    /// 이 에셋 하나로 'A무기 공격 -> B무기로 교체 -> B무기 공격'의 전체 흐름을 관리
    /// </summary>
    [CreateAssetMenu(fileName = "NewSwapAttackData", menuName = "Data/Swap Attack Data")]
    public class SwapAttackData : ScriptableObject
    {
        [Header("Pre-Swap Phase")]
        [Tooltip("무기가 교체되기 전에 실행될 공격 효과들입니다.")]
        public AttackEffect[] preSwapEffects;

        [Header("Swap Phase")]
        [Tooltip("교체할 목표 무기의 타입입니다.")]
        public WeaponType targetWeaponType;

        [Header("Post-Swap Phase")]
        [Tooltip("무기가 교체된 후에 실행될 공격 효과들입니다.")]
        public AttackEffect[] postSwapEffects;
    }
}