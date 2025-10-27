// Assets/Scripts/Monster/States/MonsterBattleState.cs
using Player.States;
using UnityEngine;
using Monster.Data;

namespace Monster.States
{
    public class MonsterBattleState : IMonsterState, IMonsterAttackState
    {
        private readonly Monster _monster;
        private bool _isAttackFinished = true; // 공격 애니메이션이 끝났는지 확인하는 플래그
        private float _attackCooldownTimer = 0f; // 공격 후 대기 시간 타이머

        private const float AttackCooldown = 3.5f; // 공격 후 다음 공격까지의 최소 대기 시간
        private const float FacingThreshold = 0.95f;
        public MonsterBattleState(Monster monster)
        {
            _monster = monster;
        }

        public void Enter()
        {
            Debug.Log("전투 상태 시작.");
            _isAttackFinished = true; // 전투 상태에 진입하면 바로 공격 시작
            _attackCooldownTimer = 0f; // 쿨다운 초기화
            _monster.Agent.ResetPath(); // 이동을 즉시 멈춥니다.
        }

        public void Update()
        {
            // 공격 애니메이션이 아직 끝나지 않았다면 아무것도 하지 않고 대기합니다.
            if (!_isAttackFinished) return;

            // 공격이 끝났다면, 쿨다운 타이머를 감소시킵니다.
            _attackCooldownTimer -= Time.deltaTime;

            if (!_monster.target)
            {
                _monster.StateMachine.ChangeState(_monster.PatrolState);
                return;
            }

            // 플레이어와의 거리를 확인합니다.
            float distanceToPlayer = Vector3.Distance(_monster.transform.position, _monster.target.position);

            // 1. 플레이어가 공격 범위를 벗어났지만 아직 인식 범위 안에 있다면, 추격 상태로 전환합니다.
            if (distanceToPlayer > _monster.Data.attackRange && distanceToPlayer <= _monster.Data.detectionRange)
            {
                _monster.StateMachine.ChangeState(_monster.ChaseState);
                return;
            }
            // 2. 플레이어가 인식 범위를 완전히 벗어났다면, 순찰 상태로 전환합니다.
            else if (distanceToPlayer > _monster.Data.detectionRange)
            {
                _monster.target = null;
                _monster.StateMachine.ChangeState(_monster.PatrolState);
                return;
            }

            // 3. 공격 범위 내에 있다면,
            // 항상 플레이어를 향해 몸을 돌립니다.
            FaceTarget();

            // 쿨다운이 끝났고, 플레이어를 충분히 바라보고 있다면 공격을 시작합니다.
            if (_attackCooldownTimer <= 0f && IsFacingTarget())
            {
                // 우선순위 1: 노란 공격 (정예)
                if (_monster.IsYellowAttackReady)
                {
                    // 그룹 쿨다운 확인
                    if (_monster.Spawner.RequestSpecialAttack())
                    {
                        Debug.Log("노란 공격(YellowAttack) 준비!");
                        // 그룹 쿨다운 확보! 노란 공격 준비 상태로 전환
                        _isAttackFinished = false;
                        _monster.NextSpecialAttackType = MonsterSkillData.AttackType.Yellow;
                        _monster.SetSpecialAttacking(true); // 경직 면역 시작
                        _monster.StateMachine.ChangeState(_monster.SpecialAttackReadyState);
                    }
                    else
                    {
                        Debug.Log("노란 공격(YellowAttack) 준비실패!");
                        // 그룹 쿨다운 실패 (다른 몬스터가 특수 공격 중) -> 일반 공격
                        _isAttackFinished = false;
                        _monster.AnimController.PlayAttack();
                    }
                }
                // 우선순위 2: 파란 공격
                else if (_monster.IsBlueAttackReady)
                {
                    if (_monster.Spawner.RequestSpecialAttack())
                    {
                        Debug.Log("파란 공격(BlueAttack) 준비!");
                        // 그룹 쿨다운 확보! 파란 공격 준비 상태로 전환
                        _isAttackFinished = false;
                        _monster.NextSpecialAttackType = MonsterSkillData.AttackType.Blue;
                        _monster.SetSpecialAttacking(true); // 경직 면역 시작
                        _monster.StateMachine.ChangeState(_monster.SpecialAttackReadyState);
                    }
                    else
                    {
                        Debug.Log("파란 공격(BlueAttack) 준비실패!");
                        // 그룹 쿨다운 실패 시, '만약 노란 공격도 준비됐었다면' 페널티 적용
                        if (_monster.IsYellowAttackReady)
                        {
                            _monster.ApplyYellowAttackPenalty();
                        }

                        // 그룹 쿨다운 실패 -> 일반 공격
                        _isAttackFinished = false;
                        _monster.AnimController.PlayAttack();
                    }
                }
                // 우선순위 3: 일반 공격
                else
                {
                    _isAttackFinished = false;
                    _monster.AnimController.PlayAttack();
                }
            }
        }

        public void Exit()
        {
            _attackCooldownTimer = 0f;
        }

        /// <summary>
        /// (애니메이션 이벤트에서 호출될) 공격 애니메이션이 끝났음을 알리는 메서드입니다.
        /// </summary>
        public void OnAttackFinished()
        {
            _attackCooldownTimer = 3.5f;
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
        /// <summary>
        /// 몬스터가 타겟을 정면으로 바라보고 있는지 확인합니다.
        /// </summary>
        /// <returns>정면으로 보고 있으면 true, 아니면 false를 반환합니다.</returns>
        private bool IsFacingTarget()
        {
            if (_monster.target == null) return false;

            // 몬스터의 정면 방향 벡터
            Vector3 forward = _monster.transform.forward;
            // 몬스터에서 타겟을 향하는 방향 벡터
            Vector3 directionToTarget = (_monster.target.position - _monster.transform.position).normalized;

            // 두 벡터의 내적(dot product) 값이 임계값보다 높으면 정면으로 간주합니다.
            return Vector3.Dot(forward, directionToTarget) > FacingThreshold;
        }
    }
}