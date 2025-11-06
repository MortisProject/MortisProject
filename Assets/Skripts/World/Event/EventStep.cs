// Assets/Skripts/World/Event/EventStep.cs
using UnityEngine;
using UnityEngine.Events;

namespace World.Event
{
    /// <summary>
    /// 이벤트 단계의 '설계도'(추상 클래스).
    /// </summary>
    public abstract class EventStep : MonoBehaviour
    {
        [Header("이벤트 단계 정보")]
        [Tooltip("이 단계의 이름입니다. (예: 1-1. 창고 진입)")]
        public string eventTag = "New Event Step";

        [Tooltip("이 단계가 무엇을 하는지 설명합니다. (기획자용)")]
        [TextArea(2, 5)]
        public string description;

        [Header("이벤트 단계 출력")]
        [Tooltip("이 단계가 성공적으로 완료되었을 때 호출할 이벤트입니다.")]
        public UnityEvent OnStepCompleted; // (옵저버 패턴의 'Subject' 역할)

        [Header("디버그 시각화")]
        [SerializeField] private Color gizmoColor = new Color(1, 0.92f, 0.016f, 0.3f);

        /// <summary>
        /// (읽기 전용) 현재 이 단계가 활성화된 상태인지 나타냅니다.
        /// </summary>
        public bool IsActive { get; private set; }

        /// <summary>
        /// [수정] (SequenceManager가 호출) 이 단계를 '활성화' 상태로 만듭니다.
        /// </summary>
        public void Activate()
        {
            if (IsActive) return;
            IsActive = true;
            Debug.Log($"[EventStep] 활성화: ({eventTag}) - {gameObject.name}", this);
            Begin(); // 실제 자식 클래스의 로직 실행
        }

        /// <summary>
        /// [수정] (SequenceManager가 호출) 이 단계를 '비활성화' 상태로 만듭니다.
        /// </summary>
        public void Deactivate()
        {
            IsActive = false;
        }

        /// <summary>
        /// [수정] 자식 클래스가 구현해야 할 실제 로직 (protected로 변경)
        /// </summary>
        protected abstract void Begin();

        /// <summary>
        /// (자식 클래스에서 호출용) 이 단계의 임무 완수를 알립니다.
        /// </summary>
        protected void NotifyCompletion()
        {
            Debug.Log($"[EventStep] 완료: ({eventTag}) - {gameObject.name}", this);
            // [수정] 완료 시 스스로 비활성화하지 않고, 매니저가 Deactivate()를 호출하도록 기다림.
            // Deactivate(); // -> SequenceManager가 OnNextStep에서 처리
            OnStepCompleted?.Invoke();
        }

#if UNITY_EDITOR
        // 디버그용 시각화 기즈모
        private void OnDrawGizmos()
        {
            Color activeColor = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 1f);
            Gizmos.color = IsActive ? activeColor : gizmoColor;

            if (IsActive)
            {
                Gizmos.DrawSphere(transform.position, 1f);
                UnityEditor.Handles.Label(transform.position + Vector3.up * 1.5f, $"▶ {eventTag} (ACTIVE)");
            }
            else
            {
                Gizmos.DrawWireSphere(transform.position, 1f);
                UnityEditor.Handles.Label(transform.position + Vector3.up * 1.5f, $"■ {eventTag}");
            }

            if (transform.parent != null && transform.parent.TryGetComponent<SequenceManager>(out var manager))
            {
                Gizmos.color = gizmoColor;
                Gizmos.DrawLine(transform.position, manager.transform.position);
            }
        }
#endif
    }
}