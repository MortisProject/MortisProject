// Assets/Scripts/Monster/States/MonsterYellowAttackState.cs
using UnityEngine;

namespace Monster.States
{
    /// <summary>
    /// 노란 공격(Yellow Attack)을 '실행'하는 상태입니다.
    /// Ready 상태와 달리, 이 상태에서는 플레이어를 즉시 추적하며 바라봅니다.
    /// </summary>
    public class MonsterYellowAttackState : IMonsterState, IMonsterAttackState
    {
        private readonly Monster _monster;
        private bool _isAttackFinished = false;
        private const float FacingThreshold = 0.95f; // BattleState와 동일한 값

        public MonsterYellowAttackState(Monster monster)
        {
            _monster = monster;
        }

        public void Enter()
        {
            Debug.Log("노란 공격(YellowAttack) 상태 시작!");
            _isAttackFinished = false;

            // 이동은 멈춘 상태를 유지합니다.
            _monster.Agent.ResetPath();

            // 노란 공격 애니메이션을 재생합니다.
            _monster.AnimController.PlayYellowAttack();
        }

        public void Update()
        {
            // (애니메이터 예시) "Start" 상태에서는 플레이어를 바라봅니다.
            if (!_isAttackFinished)
            {
                FaceTarget();
            }
        }

        public void Exit()
        {
            // 상태를 나갈 때 특별히 처리할 것은 없습니다.
            // (경직 면역 해제, 콤보 리셋 등은 Monster.cs의 OnAttackFinished에서 처리)
        }

        /// <summary>
        /// (Monster.cs -> AnimationEvents를 통해 호출됨)
        /// 애니메이션이 종료되면 BattleState로 복귀합니다.
        /// </summary>
        public void OnAttackFinished()
        {
            _isAttackFinished = true;
            Debug.Log("노란 공격(YellowAttack) 종료. BattleState로 복귀.");
            _monster.StateMachine.ChangeState(_monster.BattleState);
        }

        // --- BattleState와 동일한 바라보기 헬퍼 메서드 ---

        private void FaceTarget()
        {
            if (_monster.target == null) return;
            Vector3 direction = (_monster.target.position - _monster.transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            _monster.transform.rotation = Quaternion.Slerp(_monster.transform.rotation, lookRotation, Time.deltaTime * 5f);
        }

        private bool IsFacingTarget()
        {
            if (_monster.target == null) return false;
            Vector3 forward = _monster.transform.forward;
            Vector3 directionToTarget = (_monster.target.position - _monster.transform.position).normalized;
            return Vector3.Dot(forward, directionToTarget) > FacingThreshold;
        }
    }
}