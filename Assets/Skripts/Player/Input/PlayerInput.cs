// Assets/Scripts/Player/Input/PlayerInput.cs
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    /// <summary>
    /// PlayerInputActions 에셋과 연동하여 실제 입력을 처리하고,
    /// 다른 스크립트들이 참조할 수 있도록 입력 값을 프로퍼티로 제공합니다.
    /// </summary>
    public class PlayerInput : MonoBehaviour
    {
        // PlayerInputActions 클래스의 인스턴스
        private PlayerInputActions _input;

        [Header("Input Values (For Debugging)")]
        [Tooltip("이동 입력 값 (Vector2)")]
        public Vector2 MoveInput { get; private set; }

        [Tooltip("시점 변경 입력 값 (Vector2)")]
        public Vector2 LookInput { get; private set; }

        [Tooltip("달리기 입력 여부 (bool)")]
        public bool IsRunning { get; private set; }

        [Tooltip("점프 입력 여부 (bool, 1프레임)")]
        public bool IsJumpPressed { get; private set; }

        // TODO: 공격, 닷지 등 추후 추가될 액션에 대한 프로퍼티를 여기에 선언합니다.

        /// <summary>
        /// 컴포넌트가 활성화될 때 Input Actions 인스턴스를 생성하고 이벤트를 구독합니다.
        /// </summary>
        private void OnEnable()
        {
            // Input Actions 인스턴스가 없으면 생성
            if (_input == null)
            {
                _input = new PlayerInputActions();
            }

            // Player 액션맵 활성화
            _input.Player.Enable();

            // 각 액션에 대한 콜백(이벤트)을 구독(+=)
            _input.Player.Move.performed += OnMove;
            _input.Player.Move.canceled += OnMove;

            _input.Player.Look.performed += OnLook;
            _input.Player.Look.canceled += OnLook;

            _input.Player.Run.performed += OnRun;
            _input.Player.Run.canceled += OnRun;

            _input.Player.Jump.performed += OnJump;
        }

        /// <summary>
        /// 컴포넌트가 비활성화될 때 이벤트 구독을 해제합니다.
        /// </summary>
        private void OnDisable()
        {
            _input.Player.Disable();

            // 메모리 누수 방지를 위해 등록했던 모든 콜백을 해제(-=)
            _input.Player.Move.performed -= OnMove;
            _input.Player.Move.canceled -= OnMove;

            _input.Player.Look.performed -= OnLook;
            _input.Player.Look.canceled -= OnLook;

            _input.Player.Run.performed -= OnRun;
            _input.Player.Run.canceled -= OnRun;

            _input.Player.Jump.performed -= OnJump;
        }

        /// <summary>
        /// 모든 Update 로직이 끝난 후 호출되어, 프레임 단위 입력을 초기화합니다.
        /// </summary>
        private void LateUpdate()
        {
            // 점프는 한 프레임에만 감지되어야 하므로 매 프레임 끝에 false로 초기화합니다.
            IsJumpPressed = false;
        }

        // 각 콜백 메서드는 private으로 선언하여 외부에서 직접 호출하지 않도록 합니다.
        private void OnMove(InputAction.CallbackContext context)
        {
            Debug.Log($"OnMove Called! --- Phase: {context.phase}, Value: {context.ReadValue<Vector2>()}");
            MoveInput = context.ReadValue<Vector2>();
        }

        private void OnLook(InputAction.CallbackContext context)
        {
            LookInput = context.ReadValue<Vector2>();
        }

        private void OnRun(InputAction.CallbackContext context)
        {
            IsRunning = context.ReadValueAsButton();
        }

        private void OnJump(InputAction.CallbackContext context)
        {
            IsJumpPressed = true;
        }
    }
}