// Assets/Skripts/World/Event/Steps/EventStepCollapseFloor.cs
using UnityEngine;
using World.Event;     // EventStep을 상속받기 위해 필수
using World.Manager; // CameraManager를 사용하기 위해 필수

namespace World.Event
{
    /// <summary>
    /// 바닥 붕괴 등 시각적 연출과 카메라 쉐이크를 실행하는 이벤트 단계입니다.
    /// </summary>
    public class EventStepCollapseFloor : EventStep
    {
        [Header("연출 설정")]
        [Tooltip("바닥 붕괴 애니메이션 등을 재생할 Animator")]
        [SerializeField] private Animator visualAnimator;

        [Tooltip("Animator에서 발동시킬 트리거 이름")]
        [SerializeField] private string animationTrigger = "Collapse";

        [Header("카메라 연출")]
        [Tooltip("이 연출이 시작될 때 카메라 지진 효과(지속성 쉐이크)를 발동시킵니다.")]
        [SerializeField] private bool startCameraShake = true;

        // TODO: 나중에 지진을 멈추는 'EventStepStopShake'를
        // SequenceManager의 다음 단계로 추가할 수 있습니다.

        /// <summary>
        /// SequenceManager가 이 단계를 활성화할 때 호출됩니다.
        /// </summary>
        public override void Begin()
        {
            Debug.Log("[EventStep] 바닥 붕괴 연출을 시작합니다...", this);

            // 1. 애니메이터가 연결되어 있으면 트리거를 발동시킵니다.
            if (visualAnimator != null)
            {
                visualAnimator.SetTrigger(animationTrigger);
            }

            // 2. 카메라 쉐이크 옵션이 켜져 있으면 CameraManager에 지진 시작을 요청합니다.
            if (startCameraShake)
            {
                // (이전에 만든 CameraManager의 지속성 쉐이크 함수 호출)
                CameraManager.Instance.StartContinuousShake();
            }

            // 3. 이 연출은 기다릴 필요 없이 즉시 다음 단계로 넘어가도록 설정합니다.
            // (만약 연출이 끝날 때까지 기다려야 한다면, 애니메이션 이벤트에서 NotifyCompletion()을 호출하도록 수정)
            NotifyCompletion();
        }
    }
}