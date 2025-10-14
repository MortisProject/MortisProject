// Assets/Skripts/Player/Data/PursuitData.cs
using UnityEngine;

namespace Player.Data
{
    /// <summary>
    /// '추격 및 강타' 액션의 모든 파라미터를 정의하는 ScriptableObject입니다.
    /// </summary>
    [CreateAssetMenu(fileName = "NewPursuitData", menuName = "Data/Pursuit Data")]
    public class PursuitData : ScriptableObject
    {
        [Header("Pursuit Settings")]
        [Tooltip("목표를 향해 돌진하는 속도입니다.")]
        public float pursuitSpeed = 40f;

        [Tooltip("목표로부터 수평으로 얼마나 떨어진 지점 상공으로 도약할지 결정합니다.")]
        public float horizontalOffset = 3f;

        [Tooltip("지상으로부터 수직으로 얼마나 높은 지점까지 도약할지 결정합니다.")]
        public float verticalOffset = 5f;

        [Header("Descending Phase")]
        [Tooltip("최고점에서 낙하할 때 적용할 중력 배율입니다. 높을수록 빠르게 떨어집니다. (기본 중력 = 1)")]
        [Range(1f, 10f)]
        public float gravityMultiplier = 2.5f;

        [Header("Finisher Attack")]
        [Tooltip("추격 후 목표에 도달했을 때 실행할 마무리 일격 효과입니다.")]
        public AttackEffect finisherEffect;

        // TODO: 추격 시작/종료 시의 VFX, SFX 등을 여기에 추가할 수 있습니다.
    }
}