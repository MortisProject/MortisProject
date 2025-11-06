// Assets/Skripts/World/Event/SequenceManager.cs
using UnityEngine;
using System.Collections.Generic; // List를 사용하기 위해 필수
using World.Event; // 1단계에서 만든 EventStep에 접근하기 위해 필수

namespace World.Event
{
    /// <summary>
    /// 여러 이벤트 단계(EventStep)를 순차적으로 실행하는 총괄 매니저입니다.
    /// 이 컴포넌트에 실행할 EventStep들을 리스트로 등록합니다.
    /// </summary>
    public class SequenceManager : MonoBehaviour
    {

        [Header("이벤트 순서")]
        [Tooltip("이 시퀀스 매니저가 실행할 이벤트 단계(EventStep)들입니다. 리스트의 순서대로 실행됩니다.")]
        [SerializeField]
        private List<EventStep> eventSteps;
        
        [Header("Runtime Status (Read-Only)")]
        [Tooltip("현재 활성화된 단계의 태그(이름)입니다.")]
        [SerializeField]
        private string currentStepName = "None";

        [Tooltip("현재 활성화된 단계의 오브젝트입니다.")]
        [SerializeField]
        private EventStep currentActiveStep;

        // 현재 몇 번째 단계를 실행 중인지 추적하는 인덱스
        private int currentStepIndex = -1;

        /// <summary>
        /// 컴포넌트가 활성화될 때, 리스트에 등록된 모든 EventStep의 완료 이벤트를 구독합니다.
        /// </summary>
        private void Awake()
        {
            eventSteps = new List<EventStep>();
            foreach (Transform child in transform)
            {
                // 자식이 활성화되어 있고, EventStep 컴포넌트를 가지고 있다면
                if (child.gameObject.activeSelf && child.TryGetComponent<EventStep>(out var step))
                {
                    eventSteps.Add(step);
                }
            }

            // 모든 자식 단계(EventStep)의 OnStepCompleted 이벤트를 구독합니다.
            foreach (var step in eventSteps)
            {
                step.OnStepCompleted.AddListener(OnNextStep);
            }
        }

        /// <summary>
        /// (외부에서 호출) 이 이벤트 시퀀스를 처음부터 시작합니다.
        /// (예: 플레이어가 특정 트리거에 진입했을 때 이 함수를 호출)
        /// </summary>
        [ContextMenu("DEBUG: 이벤트 시퀀스 시작")] // 인스펙터에서 우클릭 메뉴로 테스트 가능
        public void StartSequence()
        {
            Debug.Log($"[SequenceManager] 시퀀스 시작: {gameObject.name} (총 {eventSteps.Count} 단계)", this);

            // 모든 단계를 비활성화 상태로 초기화 (재시작 대비)
            foreach (var step in eventSteps)
            {
                step.Deactivate();
            }

            currentStepIndex = -1;
            currentStepName = "Initializing...";
            OnNextStep(); // 첫 번째 단계를 실행
        }

        /// <summary>
        /// (내부 함수) 자식 EventStep이 완료 신호를 보낼 때마다 호출됩니다.
        /// </summary>
        private void OnNextStep()
        {
            // 이전 단계(있었다면)를 비활성화(Deactivate)시킵니다.
            if (currentActiveStep != null)
            {
                currentActiveStep.Deactivate();
            }

            // 다음 단계로 인덱스를 증가시킵니다.
            currentStepIndex++;

            // 아직 실행할 다음 단계가 리스트에 남아있는지 확인합니다.
            if (currentStepIndex < eventSteps.Count)
            {
                currentActiveStep = eventSteps[currentStepIndex];

                // EventStep이 null이 아닌지 다시 확인 (안전 코드)
                if (currentActiveStep != null)
                {
                    currentStepName = currentActiveStep.eventTag;
                    Debug.Log($"[SequenceManager] {currentStepIndex + 1}번째 단계 시작 -> {currentStepName}");

                    // Begin() 대신 Activate()를 호출하여 상태를 변경
                    currentActiveStep.Activate();
                }
                else
                {
                    // GetComponentsInChildren에서 null이 올 수 없지만, 안전을 위해
                    Debug.LogWarning($"[SequenceManager] {currentStepIndex + 1}번째 단계가 비어있습니다(null). 이 단계를 건너뜁니다.");
                    currentStepName = "Skipped (Null)";
                    OnNextStep(); // 즉시 다음 단계 시도
                }
            }
            else
            {
                // 리스트의 모든 단계를 완료했습니다.
                Debug.Log($"[SequenceManager] 시퀀스 모든 단계 완료: {gameObject.name}");
                currentStepName = "Sequence Completed";
                currentActiveStep = null;
            }
        }
    }
}