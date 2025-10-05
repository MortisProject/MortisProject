// Assets/Scripts/Player/States/Wire/PlayerWireAimState.cs
using Player.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Player.States
{
    public class PlayerWireAimState : IState
    {
        private readonly Player _player;
        private readonly PlayerStateMachine _stateMachine;
        private readonly PlayerInput _input;
        private readonly PlayerSO _data;
        private readonly Image _wireReticule;

        private Transform _bestTarget; // 현재 가장 적합한 타겟

        public PlayerWireAimState(Player player, PlayerStateMachine stateMachine, PlayerInput input, PlayerSO data, Image wireReticule)
        {
            _player = player;
            _stateMachine = stateMachine;
            _input = input;
            _data = data;
            _wireReticule = wireReticule;
        }

        public void Enter()
        {
            _bestTarget = null;
            _wireReticule.gameObject.SetActive(true); // 조준 시작 시 UI 활성화
        }

        public void Update()
        {
            if (!_input.IsWireAiming)
            {
                // 이전 상태로 복귀
                if (_stateMachine.IsGrounded) _stateMachine.ChangeState(_player.IdleState);
                else _stateMachine.ChangeState(_player.FallState);
                return;
            }

            FindBestWireTarget();
            UpdateReticulePosition();

            if (_bestTarget != null && _input.IsWireFirePressed)
            {
                _stateMachine.WireTarget = _bestTarget;
                _stateMachine.ChangeState(_player.WireLaunchState);
            }
        }

        public void Exit()
        {
            _wireReticule.gameObject.SetActive(false); // 조준 종료 시 UI 비활성화
            // TODO: 조준 UI를 비활성화하고 카메라 줌을 원래대로 되돌립니다.
            // TODO: 강조 표시된 타겟의 하이라이트를 해제합니다.
        }

        /// <summary>
        /// 화면 중앙에서 가장 가깝고 유효한 와이어 포인트를 찾습니다.
        /// </summary>
        private void FindBestWireTarget()
        {
            _bestTarget = null;
            float closestAngle = float.MaxValue;

            Collider[] colliders = Physics.OverlapSphere(_player.transform.position, _data.wireMaxLength);

            foreach (var collider in colliders)
            {
                if (collider.TryGetComponent<WirePoint>(out WirePoint wirePoint))
                {
                    Vector3 directionToTarget = (wirePoint.transform.position - Camera.main.transform.position);

                    // 화면 밖에 있는 타겟은 제외
                    if (Vector3.Dot(Camera.main.transform.forward, directionToTarget.normalized) < 0) continue;

                    // 화면 좌표로 변환 (0~1 범위)
                    Vector2 screenPoint = Camera.main.WorldToViewportPoint(wirePoint.transform.position);
                    // 화면 중앙(0.5, 0.5)으로부터의 거리 계산
                    float distanceFromCenter = Vector2.Distance(screenPoint, new Vector2(0.5f, 0.5f));

                    // 설정한 탐색 반경 안에 있고, 이전 타겟보다 더 중앙에 가깝다면
                    if (distanceFromCenter < _data.wireAimSearchRadius && distanceFromCenter < closestAngle)
                    {
                        closestAngle = distanceFromCenter;
                        _bestTarget = wirePoint.transform;
                    }
                }
            }
        }

        /// <summary>
        /// 조준점 UI의 위치를 업데이트합니다.
        /// </summary>
        private void UpdateReticulePosition()
        {
            if (_bestTarget == null)
            {
                _wireReticule.enabled = false; // 타겟이 없으면 숨김
            }
            else
            {
                _wireReticule.enabled = true; // 타겟이 있으면 표시
                // 타겟의 월드 좌표를 스크린 좌표로 변환하여 UI 위치 설정
                _wireReticule.transform.position = Camera.main.WorldToScreenPoint(_bestTarget.position);
            }
        }
    }
}