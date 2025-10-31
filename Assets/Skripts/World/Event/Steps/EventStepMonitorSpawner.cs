// Assets/Skripts/World/Event/Steps/EventStepMonitorSpawner.cs
using UnityEngine;
using World.Event; // EventStep을 상속받기 위해 필수
using Monster;     // MonsterSpawner에 접근하기 위해 필수

namespace World.Event
{
    /// <summary>
    /// 지정된 MonsterSpawner가 생성한 몬스터가 모두 처치될 때까지 감시하는 이벤트 단계입니다.
    /// </summary>
    public class EventStepMonitorSpawner : EventStep
    {
        [Header("감시 대상")]
        [Tooltip("이 단계가 활성화할 몬스터 스포너입니다.")]
        [SerializeField] private MonsterSpawner spawner;

        // 처치해야 할 몬스터의 남은 수
        private int monstersToDefeat;

        /// <summary>
        /// SequenceManager가 이 단계를 활성화할 때 호출됩니다.
        /// </summary>
        public override void Begin()
        {
            if (spawner == null)
            {
                Debug.LogWarning($"[EventStep] '{gameObject.name}'에 스포너가 할당되지 않았습니다. 이 단계를 즉시 완료 처리합니다.");
                NotifyCompletion();
                return;
            }

            // 1. 스포너에게 총 몇 마리를 스폰할 것인지 물어봅니다.
            monstersToDefeat = spawner.GetSpawnPointCount();

            if (monstersToDefeat <= 0)
            {
                Debug.LogWarning($"[EventStep] '{spawner.name}'에 스폰 포인트가 없습니다. 이 단계를 즉시 완료 처리합니다.");
                NotifyCompletion();
                return;
            }

            Debug.Log($"[EventStep] 몬스터 {monstersToDefeat}마리 처치 감시를 시작합니다.", this);

            // 2. 스포너에게 "나(this)를 감시자로 등록"하며 몬스터 스폰을 활성화합니다.
            spawner.ActivateSpawner(this);
        }

        /// <summary>
        /// (Monster.cs가 호출할 공개 함수) 몬스터가 죽을 때마다 이 함수가 호출됩니다.
        /// </summary>
        public void OnMonsterDied()
        {
            monstersToDefeat--;
            Debug.Log($"[EventStep] 몬스터 처치! 남은 몬스터: {monstersToDefeat}", this);

            // 3. 남은 몬스터가 0이 되면 임무 완수!
            if (monstersToDefeat <= 0)
            {
                NotifyCompletion();
            }
        }
    }
}