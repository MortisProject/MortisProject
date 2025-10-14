// Assets/Scripts/Monster/States/MonsterIdleState.cs
using UnityEngine;

namespace Monster.States
{
    public class MonsterIdleState : IMonsterState
    {
        private readonly Monster _monster;
        private float _idleTimer;
        private float _searchTimer;

        public MonsterIdleState(Monster monster)
        {
            _monster = monster;
        }

        public void Enter()
        {
            // Idle 상태에 진입하면 0.5초 대기 타이머를 설정합니다.
            _idleTimer = 0.5f;
            Debug.Log("Idle 상태 시작. 0.5초 대기합니다.");

            // 이동을 멈춥니다.
            _monster.Agent.ResetPath(); // _monster.를 통해 기능에 접근
            _monster.AnimController.SetWalking(false); // 애니메이션 제어

            _searchTimer = 0.5f; // 0.5초마다 주변을 탐색
        }

        public void Update()
        {
            // 몬스터에게 타겟이 없다면,
            if (_monster.target == null)
            {
                _searchTimer -= Time.deltaTime;
                if (_searchTimer <= 0f)
                {
                    LookForPlayer(); // 주변을 탐색합니다.
                    _searchTimer = 0.5f; // 타이머 초기화
                }
            }
            else // 타겟을 찾았다면 즉시 추격 상태로 전환합니다.
            {
                _monster.StateMachine.ChangeState(_monster.ChaseState);
                return;
            }

            _idleTimer -= Time.deltaTime;
            if (_idleTimer <= 0f)
            {
                _monster.StateMachine.ChangeState(_monster.PatrolState);
            }
        }

        public void Exit()
        {
            // Idle 상태를 벗어날 때 특별히 처리할 내용은 현재 없습니다.
        }

        private void LookForPlayer()
        {
            // 몬스터의 인식 범위 내에 있는 모든 콜라이더를 가져옵니다.
            Collider[] colliders = Physics.OverlapSphere(_monster.transform.position, _monster.Data.detectionRange);
            foreach (var collider in colliders)
            {
                // 그 중 "Player" 태그를 가진 오브젝트가 있다면,
                if (collider.CompareTag("Player"))
                {
                    // target으로 설정하고 탐색을 종료합니다.
                    _monster.target = collider.transform;
                    Debug.Log("플레이어 발견!");
                    return;
                }
            }
        }
    }
}