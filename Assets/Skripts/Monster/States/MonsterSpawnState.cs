// Assets/Scripts/Monster/States/MonsterSpawnState.cs
using UnityEngine;

namespace Monster.States
{
    public class MonsterSpawnState : IMonsterState
    {
        private readonly Monster _monster;
        private float _spawnTimer;

        // 생성자에서 Monster 컨트롤러를 참조합니다.
        public MonsterSpawnState(Monster monster)
        {
            _monster = monster;
        }

        public void Enter()
        {
            // 스폰 상태에 진입하면 타이머를 2초로 설정합니다.
            _spawnTimer = 2f;
            Debug.Log("몬스터 스폰! 2초간 무적입니다.");

            // TODO: 스폰 시 무적 상태를 나타내는 시각 효과(VFX)를 재생할 수 있습니다.
            // TODO: 몬스터 모델이 서서히 나타나는 등의 스폰 애니메이션을 재생할 수 있습니다.
        }

        public void Update()
        {
            // 매 프레임 타이머를 감소시킵니다.
            _spawnTimer -= Time.deltaTime;

            // 타이머가 0 이하로 떨어지면, Idle 상태로 전환합니다.
            if (_spawnTimer <= 0f)
            {
                _monster.StateMachine.ChangeState(_monster.IdleState);
            }
        }

        public void Exit()
        {
            Debug.Log("스폰 완료. Idle 상태로 전환합니다.");
            // TODO: 스폰 관련 효과(VFX, 애니메이션)를 여기서 종료합니다.
        }
    }
}