// Assets/Scripts/Monster/States/MonsterSpecialAttackReadyState.cs
using Monster.Data;
using World.Manager;
using UnityEngine;

namespace Monster.States
{
    /// <summary>
    /// 특수 공격(파랑/노랑)을 실행하기 전, 경고(VFX)를 표시하며 대기하는 상태입니다.
    /// 이 상태에서는 플레이어를 바라보지 않습니다.
    /// </summary>
    public class MonsterSpecialAttackReadyState : IMonsterState
    {
        private readonly Monster _monster;
        private float _readyTimer;

        public MonsterSpecialAttackReadyState(Monster monster)
        {
            _monster = monster;
        }

        public void Enter()
        {
            // 특수공격 대기 애니메이션 실행
            _monster.AnimController.PlaySpecialAttackReady();
            // MonsterSO에 정의된 대기 시간을 가져옵니다.
            _readyTimer = _monster.Data.specialAttackReadyDuration;

            // 이동을 즉시 멈춥니다.
            _monster.Agent.ResetPath();

            // Monster.cs의 IsSpecialAttacking 플래그는 BattleState에서 이미 true로 설정됨
            Debug.Log($"[{_monster.NextSpecialAttackType}] 공격 준비! ({_readyTimer}초 대기)");

            string vfxTag = "";
            switch (_monster.NextSpecialAttackType)
            {
                case MonsterSkillData.AttackType.Yellow:
                    vfxTag = "MonsterYellowAttackReady"; // Inspector에 설정한 태그
                    break;
                case MonsterSkillData.AttackType.Blue:
                    vfxTag = "MonsterBlueAttackReady";   // Inspector에 설정한 태그
                    break;
            }


            VFXManager.Instance.PlayVFX(vfxTag, _monster.SpecialAttackEffectTarget.position);
        }

        public void Update()
        {
            // (애니메이터 예시) "Ready" 상태에서는 플레이어를 바라보지 않습니다.
            // FaceTarget()을 호출하지 않습니다.

            _readyTimer -= Time.deltaTime;

            if (_readyTimer <= 0f)
            {
                // 대기 시간이 끝나면, Monster.cs에 저장된 타입에 따라
                // 실제 공격 상태로 전환합니다.
                switch (_monster.NextSpecialAttackType)
                {
                    case MonsterSkillData.AttackType.Yellow:
                        _monster.StateMachine.ChangeState(_monster.YellowAttackState);
                        break;

                    case MonsterSkillData.AttackType.Blue:
                        _monster.StateMachine.ChangeState(_monster.BlueAttackState);
                        break;

                    default:
                        // 예외 처리: BattleState로 안전하게 복귀
                        Debug.LogWarning("NextSpecialAttackType이 설정되지 않았습니다. BattleState로 복귀합니다.");
                        _monster.SetSpecialAttacking(false); // 경직 면역 해제
                        _monster.StateMachine.ChangeState(_monster.BattleState);
                        break;
                }
            }
        }

        public void Exit()
        {
        }
    }
}