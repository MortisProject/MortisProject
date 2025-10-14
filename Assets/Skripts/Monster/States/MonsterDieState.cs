// Assets/Scripts/Monster/States/MonsterDieState.cs
using System.Collections;
using UnityEngine;

namespace Monster.States
{
    public class MonsterDieState : IMonsterState
    {
        private readonly Monster _monster;
        private const float DestroyDelay = 3f;

        public MonsterDieState(Monster monster)
        {
            _monster = monster;
        }

        public void Enter()
        {
            Debug.Log("사망 상태 시작.");

            // 사망 애니메이션을 재생합니다.
            _monster.AnimController.PlayDie();

            // 더 이상 충돌하거나 움직이지 않도록 관련 컴포넌트를 비활성화합니다.
            _monster.GetComponent<Collider>().enabled = false;
            _monster.Agent.enabled = false;

            // 일정 시간 후 오브젝트를 비활성화하는 코루틴을 시작
            _monster.StartCoroutine(DeactivateAfterDelay());
        }
        private IEnumerator DeactivateAfterDelay()
        {
            yield return new WaitForSeconds(DestroyDelay);

            // 이제 몬스터는 스스로를 파괴하거나 풀에 반납하지 않고,
            _monster.gameObject.SetActive(false);
            Debug.Log($"{_monster.name} 비활성화.");
        }

        public void Update()
        {
            // 죽었으므로 아무것도 하지 않습니다.
        }

        public void Exit()
        {
            // 이 상태는 FSM의 마지막이므로 Exit이 호출될 일이 거의 없습니다.
        }
    }
}