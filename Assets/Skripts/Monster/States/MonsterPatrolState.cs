// Assets/Scripts/Monster/States/MonsterPatrolState.cs
using UnityEngine;
using UnityEngine.AI;

namespace Monster.States
{
    public class MonsterPatrolState : IMonsterState
    {
        private readonly Monster _monster;
        private Vector3 _spawnPoint; // 배회 범위의 중심점이 될 스폰 위치
        private float _searchTimer;

        public MonsterPatrolState(Monster monster)
        {
            _monster = monster;
        }

        public void Enter()
        {
            Debug.Log("순찰 상태 시작.");
            _spawnPoint = _monster.transform.position;
            _monster.Agent.isStopped = false;
            _monster.AnimController.SetWalking(true);
            _searchTimer = 0.5f; // 탐색 타이머 초기화

            // 새로운 순찰 지점을 설정하고 이동을 시작합니다.
            SetNewPatrolDestination();

            // TODO: 걷는 애니메이션을 재생합니다.
        }

        public void Update()
        {
            // 1. 타겟을 찾았는지 먼저 확인합니다.
            if (FindPlayer())
            {
                // 찾았다면 즉시 ChaseState로 전환하고, 이번 프레임의 나머지 로직은 실행하지 않습니다.
                _monster.StateMachine.ChangeState(_monster.ChaseState);
                return;
            }

            // 2. 타겟이 없을 때만 순찰 로직을 수행합니다.
            // 목적지에 거의 도착했는지 확인합니다.
            if (!_monster.Agent.pathPending && _monster.Agent.remainingDistance < 0.5f)
            {
                // 도착했다면 Idle 상태로 전환하여 잠시 대기합니다.
                _monster.StateMachine.ChangeState(_monster.IdleState);
            }
        }

        public void Exit()
        {
            _monster.AnimController.SetWalking(false);
        }

        /// <summary>
        /// 몬스터의 배회 범위 내에서 랜덤한 목적지를 설정합니다.
        /// </summary>
        private void SetNewPatrolDestination()
        {
            // patrolRange 내에서 랜덤한 방향과 거리를 정합니다.
            Vector3 randomDirection = Random.insideUnitSphere * _monster.Data.patrolRange;
            randomDirection += _spawnPoint; // 스폰 위치를 중심으로 한 좌표로 변환

            // NavMesh 위에서 가장 가까운 유효한 위치를 찾습니다.
            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, _monster.Data.patrolRange, 1))
            {
                // 찾은 위치로 NavMeshAgent의 목적지를 설정합니다.
                _monster.Agent.SetDestination(hit.position);
            }
        }

        /// <summary>
        /// 주변을 탐색하여 플레이어를 찾고, 성공 여부를 bool로 반환합니다.
        /// </summary>
        private bool FindPlayer()
        {
            // 이미 타겟이 있다면 탐색할 필요가 없습니다.
            if (_monster.target != null) return true;

            _searchTimer -= Time.deltaTime;
            if (_searchTimer > 0f) return false; // 아직 탐색할 시간이 아니라면 종료

            _searchTimer = 0.5f; // 타이머 초기화

            Collider[] colliders = Physics.OverlapSphere(_monster.transform.position, _monster.Data.detectionRange);
            foreach (var collider in colliders)
            {
                if (collider.CompareTag("Player"))
                {
                    _monster.target = collider.transform;
                    Debug.Log("플레이어 발견!");
                    return true; // 플레이어를 찾았음을 알림
                }
            }

            return false; // 플레이어를 찾지 못함
        }
    }
}