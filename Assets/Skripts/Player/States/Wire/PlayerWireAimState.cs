// Assets/Scripts/Player/States/Wire/PlayerWireAimState.cs
using Player.Data;
using UnityEngine;

namespace Player.States
{
    public class PlayerWireAimState : IState
    {
        private readonly Player _player;
        private readonly PlayerStateMachine _stateMachine;
        private readonly PlayerInput _input;
        private readonly PlayerSO _data;

        private Transform _bestTarget; // 현재 가장 적합한 타겟

        public PlayerWireAimState(Player player, PlayerStateMachine stateMachine, PlayerInput input, PlayerSO data)
        {
            _player = player;
            _stateMachine = stateMachine;
            _input = input;
            _data = data;
        }

        public void Enter()
        {
            _bestTarget = null;
            // TODO: 조준 UI를 활성화하고 카메라를 줌인하는 로직을 여기에 추가합니다.
        }

        public void Update()
        {
            // 조준을 취소했는지 확인
            if (!_input.IsWireAiming)
            {
                // 이전 상태로 돌아가야 함 (Grounded 또는 Airborne)
                // 현재는 IsGrounded 값에 따라 Idle 또는 Fall 상태로 전환
                if (_stateMachine.IsGrounded)
                {
                    _stateMachine.ChangeState(_player.IdleState);
                }
                else
                {
                    _stateMachine.ChangeState(_player.FallState);
                }
                return;
            }

            // 최적의 와이어 타겟 탐색
            FindBestWireTarget();

            // 타겟이 있고, 발사 키를 눌렀는지 확인
            if (_bestTarget != null && _input.IsWireFirePressed)
            {
                // StateMachine에 현재 타겟을 저장하고 WireLaunchState로 전환
                _stateMachine.WireTarget = _bestTarget;
                _stateMachine.ChangeState(_player.WireLaunchState);
            }
        }

        public void Exit()
        {
            // TODO: 조준 UI를 비활성화하고 카메라 줌을 원래대로 되돌립니다.
            // TODO: 강조 표시된 타겟의 하이라이트를 해제합니다.
        }

        /// <summary>
        /// 화면 중앙에서 가장 가깝고 유효한 와이어 포인트를 찾습니다.
        /// </summary>
        private void FindBestWireTarget()
        {
            _bestTarget = null;
            float closestDot = -1f; // 1에 가까울수록 화면 중앙에 가까움

            // 1. 플레이어 주변의 모든 와이어 포인트를 수집
            Collider[] colliders = Physics.OverlapSphere(_player.transform.position, _data.wireMaxLength);

            foreach (var collider in colliders)
            {
                if (collider.TryGetComponent<WirePoint>(out WirePoint wirePoint))
                {
                    Vector3 directionToTarget = (wirePoint.transform.position - Camera.main.transform.position).normalized;

                    // 2. 화면 중앙과의 각도(내적)를 계산
                    float dot = Vector3.Dot(Camera.main.transform.forward, directionToTarget);

                    // 3. 화면 정면에 있고(dot > 0), 이전 타겟보다 더 중앙에 있다면
                    if (dot > 0 && dot > closestDot)
                    {
                        // TODO: 벽 뒤에 있는지 Raycast로 한번 더 확인하면 더 좋습니다.
                        closestDot = dot;
                        _bestTarget = wirePoint.transform;
                    }
                }
            }

            // TODO: _bestTarget이 정해졌다면 시각적으로 강조 표시(하이라이트)하는 로직을 여기에 추가합니다.
        }
    }
}