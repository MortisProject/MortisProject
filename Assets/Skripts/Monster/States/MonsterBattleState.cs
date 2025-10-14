// Assets/Scripts/Monster/States/MonsterBattleState.cs
using UnityEngine;

namespace Monster.States
{
    public class MonsterBattleState : IMonsterState
    {
        private readonly Monster _monster;
        private bool _isAttackFinished = true; // 공격 애니메이션이 끝났는지 확인하는 플래그
        private float _attackCooldownTimer = 0f; // 공격 후 대기 시간 타이머

        private const float AttackCooldown = 1.5f; // 공격 후 다음 공격까지의 최소 대기 시간

        public MonsterBattleState(Monster monster)
        {
            _monster = monster;
        }

        public void Enter()
        {
            Debug.Log("전투 상태 시작.");
            _isAttackFinished = false; // 전투 상태에 진입하면 바로 공격 시작
            _attackCooldownTimer = AttackCooldown; // 쿨다운 초기화

            // 플레이어를 바라보게 하고 공격 애니메이션을 실행합니다.
            FaceTarget();
            _monster.AnimController.PlayAttack();
        }

        public void Update()
        {
            // 공격 애니메이션이 아직 끝나지 않았다면 아무것도 하지 않고 대기합니다.
            if (!_isAttackFinished)
            {
                // 공격 중에도 계속 플레이어를 바라보도록 합니다.
                FaceTarget();
                return;
            }

            // 공격이 끝났다면, 쿨다운 타이머를 감소시킵니다.
            _attackCooldownTimer -= Time.deltaTime;

            // 플레이어와의 거리를 확인합니다.
            float distanceToPlayer = Vector3.Distance(_monster.transform.position, _monster.target.position);

            // 1. 플레이어가 공격 범위를 벗어났지만 아직 인식 범위 안에 있다면, 추격 상태로 전환합니다.
            if (distanceToPlayer > _monster.Data.attackRange && distanceToPlayer <= _monster.Data.detectionRange)
            {
                _monster.StateMachine.ChangeState(_monster.ChaseState);
                return;
            }

            // 2. 플레이어가 인식 범위까지 완전히 벗어났다면, 순찰 상태로 전환합니다.
            if (distanceToPlayer > _monster.Data.detectionRange)
            {
                _monster.StateMachine.ChangeState(_monster.PatrolState);
                return;
            }

            // 3. 공격 쿨다운이 끝나고, 플레이어가 여전히 공격 범위 안에 있다면 다시 공격합니다.
            if (_attackCooldownTimer <= 0f && distanceToPlayer <= _monster.Data.attackRange)
            {
                Enter(); // Enter 메서드를 다시 호출하여 공격을 시작합니다.
            }
        }

        public void Exit()
        {
            // 전투 상태를 벗어날 때 특별히 처리할 내용은 현재 없습니다.
        }

        /// <summary>
        /// (애니메이션 이벤트에서 호출될) 공격 애니메이션이 끝났음을 알리는 메서드입니다.
        /// </summary>
        public void OnAttackFinished()
        {
            _isAttackFinished = true;
            Debug.Log("몬스터 공격 애니메이션 종료.");
        }

        /// <summary>
        /// 몬스터가 타겟(플레이어)을 바라보게 합니다.
        /// </summary>
        private void FaceTarget()
        {
            Vector3 direction = (_monster.target.position - _monster.transform.position).normalized;
            // Y축 회전만 적용하여 몬스터가 기울어지지 않도록 합니다.
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            // 부드럽게 회전하도록 Slerp를 사용합니다.
            _monster.transform.rotation = Quaternion.Slerp(_monster.transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }
}