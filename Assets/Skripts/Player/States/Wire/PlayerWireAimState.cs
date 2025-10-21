// Assets/Scripts/Player/States/Wire/PlayerWireAimState.cs
using Player.Data;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
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

        // UI 관련
        private readonly Image _bestReticule;
        private readonly List<Image> _normalReticulePool = new List<Image>();

        private Transform _bestTarget;
        private readonly List<Transform> _visibleTargets = new List<Transform>();


        public PlayerWireAimState(Player player, PlayerStateMachine stateMachine, PlayerInput input, PlayerSO data)
        {
            _player = player;
            _stateMachine = stateMachine;
            _input = input;
            _data = data;
            _bestReticule = _player.BestWireReticuleUI;
        }

        public void Enter()
        {
            _bestTarget = null;
            // 카메라 연출: AimCamera의 우선순위를 높여 줌인 효과 시작
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

            FindVisibleTargets();
            UpdateAllReticules();

            if (_bestTarget != null && _input.IsWireFirePressed)
            {
                // 발사 연출 UI 생성
                Object.Instantiate(_player.WireFireEffectPrefab, _bestReticule.transform.position, Quaternion.identity, _bestReticule.transform.parent);

                _stateMachine.WireTarget = _bestTarget;
                _stateMachine.ChangeState(_player.WireLaunchState);
            }
        }

        public void Exit()
        {
            // 모든 조준 UI 숨기기
            HideAllReticules();
        }

        /// <summary>
        /// 화면 중앙에서 가장 가깝고 유효한 와이어 포인트를 찾습니다.
        /// </summary>
        private void FindVisibleTargets()
        {
            _visibleTargets.Clear();
            _bestTarget = null;
            float closestAngle = float.MaxValue;

            Collider[] colliders = Physics.OverlapSphere(_player.transform.position, _data.wireGrappleRange);

            foreach (var collider in colliders)
            {
                if (collider.TryGetComponent<WirePoint>(out WirePoint wirePoint))
                {
                    Vector3 directionToTarget = (wirePoint.transform.position - Camera.main.transform.position);
                    if (Vector3.Dot(Camera.main.transform.forward, directionToTarget.normalized) < 0) continue;

                    Vector2 screenPoint = Camera.main.WorldToViewportPoint(wirePoint.transform.position);
                    if (screenPoint.x < 0 || screenPoint.x > 1 || screenPoint.y < 0 || screenPoint.y > 1) continue;

                    _visibleTargets.Add(wirePoint.transform); // 화면에 보이는 모든 타겟 추가

                    float distanceFromCenter = Vector2.Distance(screenPoint, new Vector2(0.5f, 0.5f));
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
        private void UpdateAllReticules()
        {
            // 모든 UI 숨기기 (버그 수정)
            HideAllReticules();

            int reticuleIndex = 0;
            foreach (var target in _visibleTargets)
            {
                if (target == _bestTarget)
                {
                    _bestReticule.gameObject.SetActive(true);
                    _bestReticule.transform.position = Camera.main.WorldToScreenPoint(target.position);
                }
                else
                {
                    // 일반 조준점 UI 풀링 (간단한 버전)
                    if (reticuleIndex >= _normalReticulePool.Count)
                    {
                        Image newReticule = Object.Instantiate(_player.NormalWireReticuleUI, _player.NormalWireReticuleUI.transform.parent);
                        _normalReticulePool.Add(newReticule);
                    }
                    Image normalReticule = _normalReticulePool[reticuleIndex];
                    normalReticule.gameObject.SetActive(true);
                    normalReticule.transform.position = Camera.main.WorldToScreenPoint(target.position);
                    reticuleIndex++;
                }
            }
        }

        private void HideAllReticules()
        {
            _bestReticule.gameObject.SetActive(false);
            foreach (var reticule in _normalReticulePool)
            {
                reticule.gameObject.SetActive(false);
            }
        }
    }
}