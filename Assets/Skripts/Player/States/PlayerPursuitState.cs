// Assets/Skripts/Player/States/PlayerPursuitState.cs
using Player.Animation;
using Player.Data;
using UnityEngine;

namespace Player.States
{
    public class PlayerPursuitState : IState
    {
        // 추격 상태의 내부 단계를 구분하기 위한 열거형
        private enum PursuitPhase { Ascending, Descending }
        private PursuitPhase _currentPhase;
        
        private readonly Player _player;
        private readonly PlayerStateMachine _stateMachine;
        private readonly PlayerMotor _motor;
        private readonly PlayerAnimationController _animController;

        private Transform _target;
        private PursuitData _pursuitData;

        private Vector3 _apexDestination; // 도달해야 할 최고점 위치

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
            if (_target == null || _pursuitData == null)
            {
                _stateMachine.ChangeState(_player.FallState);
                return;
            }

            // --- 코드 블럭 단위로 제공 (수정된 부분) ---
            // 1. 목표 최고점(Apex) 위치 계산
            Vector3 directionFromTarget = (_player.transform.position - _target.position).normalized;
            directionFromTarget.y = 0; // 수평 방향만 사용
            Vector3 horizontalTargetPos = _target.position + directionFromTarget * _pursuitData.horizontalOffset;
            _apexDestination = new Vector3(horizontalTargetPos.x, _target.position.y + _pursuitData.verticalOffset, horizontalTargetPos.z);

            // 2. 물리 효과 끄고, '도약' 단계로 시작
            _player.CapsuleCollider.enabled = false;
            _motor.GetComponent<Rigidbody>().useGravity = false;
            _motor.Stop(); // 이전 속도 제거
            _currentPhase = PursuitPhase.Ascending;

            // TODO: 추격 시작 애니메이션 트리거를 여기에 추가합니다.
            // _animController.PlayPursuitStart();
        }

        public void Update()
        {
            _motor.ApplyGravityForce(_pursuitData.gravityMultiplier);

            if (_target == null || _pursuitData == null)
            {
                // 타겟이나 데이터가 없으면 즉시 상태 종료
                _stateMachine.ChangeState(_player.FallState);
                return;
            }

            switch (_currentPhase)
            {
                case PursuitPhase.Ascending:
                    UpdateAscending();
                    break;
                case PursuitPhase.Descending:
                    UpdateDescending();
                    break;
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
        /// 최고점을 향해 도약하는 단계의 로직
        /// </summary>
        private void UpdateAscending()
        {
            // 목표 최고점을 향해 플레이어 이동
            _player.transform.position = Vector3.MoveTowards(_player.transform.position, _apexDestination, _pursuitData.pursuitSpeed * Time.deltaTime);

            // 플레이어가 타겟을 계속 바라보도록 회전
            Vector3 lookDirection = (_target.position - _player.transform.position).normalized;
            lookDirection.y = 0;
            _player.transform.rotation = Quaternion.LookRotation(lookDirection);

            // 최고점에 도달했는지 확인
            if (Vector3.Distance(_player.transform.position, _apexDestination) < 0.1f)
            {
                // 도달했다면 '낙하' 단계로 전환
                _currentPhase = PursuitPhase.Descending;

                // 중력을 다시 켜서 자연스럽게 낙하 시작
                _motor.GetComponent<Rigidbody>().useGravity = true;

                // 낙하를 시작하는 이 시점에 마무리 일격 효과를 '미리' 발동
                ExecuteFinisher();

                // TODO: 낙하 강타 애니메이션 재생
                // _animController.PlayPursuitSlam();
            }
        }

        /// <summary>
        /// 강타하며 낙하하는 단계의 로직
        /// </summary>
        private void UpdateDescending()
        {
            // 땅에 착지했는지 확인
            if (_stateMachine.IsGrounded)
            {
                // 착지했다면 모든 시퀀스를 종료하고 Idle 상태로 전환
                _stateMachine.ChangeState(_player.IdleState);
            }
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