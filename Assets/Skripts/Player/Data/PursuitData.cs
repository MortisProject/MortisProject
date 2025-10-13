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

        [Tooltip("목표 얼마만큼의 거리까지 접근했을 때 멈출지 결정합니다.")]
        public float stoppingDistance = 2.5f;

        [Header("Finisher Attack")]
        [Tooltip("추격 후 목표에 도달했을 때 실행할 마무리 일격 효과입니다.")]
        public AttackEffect finisherEffect;

        // TODO: 추격 시작/종료 시의 VFX, SFX 등을 여기에 추가할 수 있습니다.
    }
}