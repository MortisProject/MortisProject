// Assets/Skripts/World/Event/Steps/EventStepTrigger.cs
using UnityEngine;
using World.Event; // EventStep을 상속받기 위해 필수

namespace World.Event
{
    /// <summary>
    /// 플레이어가 특정 영역에 진입하는 것을 감지하는 이벤트 단계입니다.
    /// 기존 SpawnTrigger와 유사하나, 시퀀스 매니저와 연동됩니다.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class EventStepTrigger : EventStep
    {
        [Header("트리거 설정")]
        [Tooltip("감지할 대상의 태그입니다.")]
        [SerializeField] private string targetTag = "Player";

        [Tooltip("한 번만 발동할지 여부입니다.")]
        [SerializeField] private bool triggerOnce = true;

        private Collider _collider;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
            // 이 컴포넌트는 반드시 트리거로 작동해야 합니다.
            _collider.isTrigger = true;
        }

        /// <summary>
        /// SequenceManager가 이 단계를 활성화할 때 호출됩니다.
        /// </summary>
        public override void Begin()
        {
            // 콜라이더를 활성화하여 플레이어의 진입을 감지할 준비를 합니다.
            _collider.enabled = true;
            Debug.Log($"[EventStep] '{targetTag}'의 진입을 대기합니다...", this);
        }

        /// <summary>
        /// 트리거 영역에 누군가 들어왔을 때 호출됩니다.
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(targetTag))
            {
                Debug.Log($"[EventStep] '{targetTag}' 진입 감지!", this);

                // 임무 완수! 총괄 매니저에게 알립니다.
                NotifyCompletion();

                // 한 번만 발동하는 경우, 콜라이더를 비활성화하여 중복 신호를 막습니다.
                if (triggerOnce)
                {
                    _collider.enabled = false;
                }
            }
        }
    }
}