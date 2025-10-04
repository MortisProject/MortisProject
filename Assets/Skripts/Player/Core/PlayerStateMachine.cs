// Assets/Scripts/Player/Core/PlayerStateMachine.cs
using Player.States;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Player
{
    public class PlayerStateMachine : MonoBehaviour
    {
        [Header("Ground Check Settings")]
        [Tooltip("지면으로 인식할 레이어를 설정합니다.")]
        [SerializeField] private LayerMask _groundLayerMask;

        [Tooltip("지면을 감지할 SphereCast의 시작점입니다.")]
        [SerializeField] private Transform _footTransform;

        [Tooltip("지면 감지 거리입니다. 캐릭터 발보다 살짝 아래까지 닿는 짧은 거리여야 합니다.")]
        [SerializeField] private float _groundCheckDistance = 0.1f;

        [Tooltip("지면 감지 SphereCast의 반지름입니다.")]
        [SerializeField] private float _groundCheckRadius = 0.2f;

        // Push Pop State를 사용하기위한 스택
        private readonly List<IState> _stateStack = new List<IState>();

        // 현재 상태
        public IState CurrentState => _stateStack.LastOrDefault();

        // 지면 착지 여부 (모든 상태에서 이 값을 참조)
        public bool IsGrounded { get; private set; }

        /// <summary>
        /// 매 프레임마다 호출됩니다.
        /// 상태 로직을 실행하기 전에 먼저 지면 감지를 수행합니다.
        /// </summary>
        private void Update()
        {
            CheckGrounded();

            CurrentState?.Update();
        }

        /// <summary>
        /// 상태 머신을 특정 상태로 초기화합니다.
        /// 스택을 비우고 새로운 상태 추가
        /// </summary>
        public void Initialize(IState startingState)
        {
            _stateStack.Clear();
            _stateStack.Add(startingState);
            CurrentState.Enter();
        }

        /// <summary>
        /// 현재 상태를 새로운 상태로 교체 합니다.
        /// </summary>
        public void ChangeState(IState newState)
        {
            if (CurrentState != null)
            {
                CurrentState.Exit();
                _stateStack.Remove(CurrentState);
            }

            Debug.Log($"[State] {CurrentState} -> {newState}");

            _stateStack.Add(newState);
            newState.Enter();
        }

        /// <summary>
        /// 외부의 강제적인 이벤트에 의해 현재 상태를 '교체'합니다. (예: 피격)
        /// 이름만 다를 뿐, 기능은 ChangeState와 동일하며 "외부에서 호출된다"는 의도를 명확히 합니다.
        /// </summary>
        public void ForceChangeState(IState newState)
        {
            ChangeState(newState);
        }

        /// <summary>
        /// 현재 상태 위에 새로운 상태를 '추가'합니다. 이전 상태는 일시정지됩니다. (예: Move -> ItemUse)
        /// </summary>
        public void PushState(IState newState)
        {
            _stateStack.Add(newState);
            newState.Enter();
        }

        /// <summary>
        /// 현재 상태를 '제거'하고 이전 상태로 돌아갑니다. (예: ItemUse -> Move)
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
        /// 발밑으로 SphereCast를 쏘아 지면 착지 여부를 확인하고 IsGrounded 값을 업데이트합니다.
        /// </summary>
        private void CheckGrounded()
        {
            // Physics.SphereCast가 true를 반환하면(지면과 충돌하면) IsGrounded는 true가 됩니다.
            IsGrounded = Physics.SphereCast(
                _footTransform.position,
                _groundCheckRadius,
                Vector3.down,
                out RaycastHit hit,
                _groundCheckDistance,
                _groundLayerMask);
        }
    }
}