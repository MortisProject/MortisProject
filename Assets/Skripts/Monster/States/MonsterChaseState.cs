// Assets/Scripts/Monster/States/MonsterChaseState.cs
using UnityEngine;

namespace Monster.States
{
    public class MonsterChaseState : IMonsterState
    {
        private readonly Monster _monster;

        public MonsterChaseState(Monster monster)
        {
            _monster = monster;
        }

        public void Enter()
        {
            Debug.Log("추격 상태 시작.");

            // NavMeshAgent의 이동을 시작하고, 달리기 애니메이션을 켭니다.
            _monster.Agent.isStopped = false;
            _monster.AnimController.SetWalking(false);
        }

        public void Update()
        {
            // 추격 중에 타겟이 사라졌다면(죽거나, 비활성화) 순찰 상태로 돌아갑니다.
            if (_monster.target == null)
            {
                _monster.StateMachine.ChangeState(_monster.PatrolState);
                return;
            }

            float distanceToPlayer = Vector3.Distance(_monster.transform.position, _monster.target.position);

            if (distanceToPlayer <= _monster.Data.attackRange)
            {
                _monster.StateMachine.ChangeState(_monster.BattleState);
                return;
            }

            // 플레이어가 인식 범위를 벗어났다면,
            if (distanceToPlayer > _monster.Data.detectionRange)
            {
                Debug.Log("플레이어를 놓쳤습니다.");
                _monster.target = null; // 타겟을 해제합니다.
                _monster.StateMachine.ChangeState(_monster.PatrolState);
                return;
            }

            _monster.Agent.SetDestination(_monster.target.position);
        }

        public void Exit()
        {
            _monster.AnimController.SetWalking(false);
        }
    }
}