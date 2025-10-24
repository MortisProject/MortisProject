// Assets/Scripts/Monster/States/MonsterHitState.cs
using UnityEngine;
using World.Manager;

namespace Monster.States
{
    public class MonsterHitState : IMonsterState
    {
        private readonly Monster _monster;
        private float _hitTimer;
        private const float HitDuration = 1.5f; // 기획서의 최소 상태 유지 시간

        public MonsterHitState(Monster monster)
        {
            _monster = monster;
        }

        public void Enter()
        {
            Debug.Log("피격 상태 시작!");
            _hitTimer = HitDuration;

            // 테스트용 피격 성공시 잠깐의 불릿타임
            // 느낌 별로임
            //BulletTimeManager.Instance.StartBulletTime(0.01f,0.05f,0.01f,0.01f);

            // 피격 애니메이션을 재생합니다.
            _monster.AnimController.PlayHit();
        }

        public void Update()
        {
            _hitTimer -= Time.deltaTime;

            // 경직 시간이 끝나면,
            if (_hitTimer <= 0f)
            {
                
                if (!_monster.target)
                {
                    _monster.StateMachine.ChangeState(_monster.IdleState); // 범위 밖 -> 대기
                    return;
                }

                // 플레이어와의 거리를 다시 확인하여 다음 상태를 결정합니다.
                float distanceToPlayer = Vector3.Distance(_monster.transform.position, _monster.target.position);
                if (distanceToPlayer <= _monster.Data.attackRange)
                {
                    _monster.StateMachine.ChangeState(_monster.BattleState); // 공격 범위 내 -> 전투
                }
                else if (distanceToPlayer <= _monster.Data.detectionRange)
                {
                    _monster.StateMachine.ChangeState(_monster.ChaseState); // 인식 범위 내 -> 추격
                }
            }
        }

        public void Exit()
        {
            // 피격 상태를 벗어날 때 특별히 처리할 내용은 현재 없습니다.
        }
    }
}