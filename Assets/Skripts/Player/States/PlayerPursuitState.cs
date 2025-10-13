// Assets/Skripts/Player/States/PlayerPursuitState.cs
using Player.Animation;
using Player.Data;
using UnityEngine;

namespace Player.States
{
    public class PlayerPursuitState : IState
    {
        private readonly Player _player;
        private readonly PlayerStateMachine _stateMachine;
        private readonly PlayerMotor _motor;
        private readonly PlayerAnimationController _animController;

        private Transform _target;
        private PursuitData _pursuitData;

        public PlayerPursuitState(Player player, PlayerStateMachine stateMachine, PlayerMotor motor, PlayerAnimationController animController)
        {
            _player = player;
            _stateMachine = stateMachine;
            _motor = motor;
            _animController = animController;
        }

        /// <summary>
        /// 추격에 필요한 타겟과 데이터를 설정하는 초기화 메서드입니다.
        /// </summary>
        public void SetPursuitData(Transform target, PursuitData data)
        {
            _target = target;
            _pursuitData = data;
        }

        public void Enter()
        {
            Debug.Log("[State] -> PlayerPursuitState");

            // 추격 중에는 플레이어의 물리적 충돌과 중력을 끕니다.
            _player.CapsuleCollider.enabled = false;
            _motor.GetComponent<Rigidbody>().useGravity = false;

            // TODO: 추격 시작 애니메이션 트리거를 여기에 추가합니다.
            // _animController.PlayPursuitStart();
        }

        public void Update()
        {
            if (_target == null || _pursuitData == null)
            {
                // 타겟이나 데이터가 없으면 즉시 상태 종료
                _stateMachine.ChangeState(_player.FallState);
                return;
            }

            // 1. 타겟 방향으로 플레이어 이동
            Vector3 directionToTarget = (_target.position - _player.transform.position).normalized;
            _motor.Move(directionToTarget * _pursuitData.pursuitSpeed);

            // 2. 플레이어가 타겟을 바라보도록 회전
            _player.transform.rotation = Quaternion.LookRotation(directionToTarget);

            // 3. 목표에 도달했는지 확인
            float distanceToTarget = Vector3.Distance(_player.transform.position, _target.position);
            if (distanceToTarget <= _pursuitData.stoppingDistance)
            {
                // 목표에 도달하면 마무리 일격 실행
                ExecuteFinisher();
            }
        }

        public void Exit()
        {
            // 상태를 나갈 때, 물리 효과를 다시 켭니다.
            _player.CapsuleCollider.enabled = true;
            _motor.GetComponent<Rigidbody>().useGravity = true;

            // 추격이 끝나면 속도를 0으로 초기화하여 관성을 없앱니다.
            _motor.Stop();
        }

        /// <summary>
        /// PursuitData에 정의된 마무리 일격을 실행합니다.
        /// </summary>
        private void ExecuteFinisher()
        {
            if (_pursuitData.finisherEffect != null)
            {
                var hitboxProvider = _player.GetComponentInChildren<PlayerAnimationEvents>();
                // PlayerAttackState가 아니므로, 세 번째 인자로 null을 전달합니다.
                _pursuitData.finisherEffect.Execute(_player, hitboxProvider, null);
            }

            // 마무리 일격 후 공중 상태로 전환
            _stateMachine.ChangeState(_player.FallState);
        }
    }
}