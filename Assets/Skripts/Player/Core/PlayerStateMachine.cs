// Assets/Scripts/Player/Core/PlayerStateMachine.cs
using Player.Animation;
using Player.Data;
using Player.States;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Player
{
    public class PlayerStateMachine : MonoBehaviour
    {
        [Header("Ground Check Settings")]
        [Tooltip("지면으로 인식할 레이어")]
        [SerializeField] private LayerMask _groundLayerMask;

        [Tooltip("지면을 감지할 SphereCast의 시작점")]
        [SerializeField] private Transform _footTransform;

        [Tooltip("지면 감지 거리, 캐릭터 발보다 살짝 아래까지 닿는 짧은 거리여야함")]
        [SerializeField] private float _groundCheckDistance = 0.1f;

        [Tooltip("지면 감지 SphereCast의 반지름")]
        [SerializeField] private float _groundCheckRadius = 0.2f;

        [Header("Component References")]
        [SerializeField] private CharacterStats _stats;
        [SerializeField] private PlayerAnimationController _animController; 
        [SerializeField] private PlayerSO _data;

        // Push Pop State를 사용하기위한 스택
        private readonly List<IState> _stateStack = new List<IState>();

        // 현재 상태
        public IState CurrentState => _stateStack.LastOrDefault();

        // 지면 착지 여부 (모든 상태에서 이 값을 참조)
        public bool IsGrounded { get; private set; }

        /// <summary>
        /// 캐릭터가 공중에 떠 있던 시간을 기록
        /// </summary>
        public float Flytime { get; private set; }

        /// <summary>
        /// 현재 조준하고 있거나 부착된 와이어 타겟
        /// </summary>
        public Transform WireTarget { get; set; }

        /// <summary>
        /// 상태 로직을 실행하기 전에 먼저 지면 감지를 수행
        /// </summary>
        private void Update()
        {

            CheckGrounded();

            _animController.SetGrounded(IsGrounded);

            if (IsGrounded)
            {
                // 땅에 있으면 Flytime을 0으로 초기화
                Flytime = 0f;
                _stats.ResetDoubleJump();

            }
            else
            {
                // 공중에 있으면 Flytime을 계속 증가시킴
                Flytime += Time.deltaTime;
            }
            CurrentState?.Update();
        }

        /// <summary>
        /// 상태 머신을 특정 상태로 초기화
        /// 스택을 비우고 새로운 상태 추가
        /// </summary>
        public void Initialize(IState startingState)
        {
            _stateStack.Clear();
            _stateStack.Add(startingState);
            CurrentState.Enter();
        }

        /// <summary>
        /// 현재 상태를 새로운 상태로 교체
        /// </summary>
        public void ChangeState(IState newState)
        {
            if (CurrentState != null)
            {
                CurrentState.Exit();
                _stateStack.Remove(CurrentState);
            }

            _stateStack.Add(newState);
            newState.Enter();
        }

        /// <summary>
        /// 외부의 강제적인 이벤트에 의해 현재 상태를 교체
        /// </summary>
        public void ForceChangeState(IState newState)
        {
            ChangeState(newState);
        }

        /// <summary>
        /// 현재 상태 위에 새로운 상태를 추가, 이전 상태는 일시정지
        /// </summary>
        public void PushState(IState newState)
        {
            _stateStack.Add(newState);
            newState.Enter();
        }

        /// <summary>
        /// 현재 상태를 제거하고 이전 상태로 복귀
        /// </summary>
        public void PopState()
        {
            if (CurrentState != null)
            {
                CurrentState.Exit();
                _stateStack.Remove(CurrentState);
            }
        }

        /// <summary>
        /// 발밑으로 SphereCast를 쏘아 지면 착지 여부를 확인하고 IsGrounded 값을 업데이트
        /// </summary>
        /// <summary>
        /// 발밑으로 SphereCast를 쏘고 경사각을 계산하여 최종 지면 착지 여부를 결정
        /// </summary>
        private void CheckGrounded()
        {
            // SphereCast를 발사하여 무언가에 닿았는지 확인
            if (Physics.SphereCast(
                    _footTransform.position,
                    _groundCheckRadius,
                    Vector3.down,
                    out RaycastHit hit,
                    _groundCheckDistance,
                    _groundLayerMask))
            {
                // 충돌한 표면의 경사각을 계산
                float slopeAngle = Vector3.Angle(Vector3.up, hit.normal);

                // 경사각이 걸을 수 있는 최대 각도보다 작거나 같다면, 땅으로 인정
                if (slopeAngle <= _data.maxSlopeAngle)
                {
                    IsGrounded = true;
                    return;
                }
            }

            // SphereCast에 아무것도 닿지 않았거나, 경사각이 너무 가파르면 땅이 아님
            IsGrounded = false;
        }

#if UNITY_EDITOR
        /// <summary>
        /// 에디터용 기즈모
        /// </summary>
        private void OnDrawGizmos()
        {
            if (_footTransform == null) return;

            Gizmos.color = Color.red;

            // SphereCast의 시작점과 끝점을 계산
            Vector3 origin = _footTransform.position;
            Vector3 destination = origin + Vector3.down * _groundCheckDistance;

            // SphereCast의 경로를 선으로 그리고, 최종 위치에 구체를 그립니다.
            Gizmos.DrawLine(origin, destination);
            Gizmos.DrawWireSphere(destination, _groundCheckRadius);
        }
        /// <summary>
        /// 디버깅을 위해 현재 상태와 IsGrounded 값을 게임 화면에 표시
        /// </summary>
        private void OnGUI()
        {
            GUI.color = Color.black;
            GUI.Label(new Rect(10, 10, 500, 20), $"Current State: {CurrentState}");
            GUI.Label(new Rect(10, 30, 500, 20), $"IsGrounded: {IsGrounded}");
        }
#endif
    }
}