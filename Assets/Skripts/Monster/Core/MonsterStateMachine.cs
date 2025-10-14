// Assets/Scripts/Monster/Core/MonsterStateMachine.cs
using Monster.States;
using UnityEngine;

namespace Monster
{
    public class MonsterStateMachine : MonoBehaviour
    {
        // 현재 활성화된 상태
        public IMonsterState CurrentState { get; private set; }
        private Monster _monster;

        private void Awake()
        {
            _monster = GetComponent<Monster>();
        }

        private void Update()
        {
            // 자신의 Update 루프에서 현재 상태의 Update를 호출합니다.
            CurrentState?.Update();
        }

        public void Initialize(IMonsterState startingState)
        {
            CurrentState = startingState;
            CurrentState.Enter();
        }

        public void ChangeState(IMonsterState newState)
        {
            CurrentState?.Exit();
            CurrentState = newState;
            CurrentState.Enter();
        }
    }
}