// Assets/Skripts/World/Event/EventStep.cs
using UnityEngine;
using UnityEngine.Events;

namespace World.Event
{
    /// <summary>
    /// 모든 이벤트 단계(자식 매니저)가 상속받아야 할 추상 클래스입니다.
    /// 이벤트 시퀀스의 기본 단위를 정의합니다.
    /// </summary>
    public abstract class EventStep : MonoBehaviour
    {
        [Header("이벤트 단계 출력")]
        [Tooltip("이 단계가 성공적으로 완료되었을 때 호출할 이벤트입니다. (SequenceManager가 자동으로 구독)")]
        public UnityEvent OnStepCompleted;

        /// <summary>
        /// SequenceManager가 이 단계를 시작하라고 명령할 때 호출됩니다.
        /// 이 단계의 실제 로직(예: 트리거 활성화, 몬스터 스폰 감시 시작)을 여기에 구현합니다.
        /// </summary>
        public abstract void Begin();

        /// <summary>
        /// (자식 클래스에서 호출용)
        /// 이 단계가 임무를 완수했을 때 호출하여, SequenceManager에게 완료되었음을 알립니다.
        /// </summary>
        protected void NotifyCompletion()
        {
            Debug.Log($"[EventStep] 완료: {gameObject.name}", this);
            OnStepCompleted?.Invoke();
        }
    }
}