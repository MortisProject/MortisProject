// Assets/Scripts/Player/Input/PlayerInput.cs
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    /// <summary>
    /// PlayerInputActions ���°� �����Ͽ� ���� �Է��� ó���ϰ�,
    /// �ٸ� ��ũ��Ʈ���� ������ �� �ֵ��� �Է� ���� ������Ƽ�� �����մϴ�.
    /// </summary>
    public class PlayerInput : MonoBehaviour
    {
        // PlayerInputActions Ŭ������ �ν��Ͻ�
        private PlayerInputActions _input;

        [Header("Input Values (For Debugging)")]
        [Tooltip("�̵� �Է� �� (Vector2)")]
        public Vector2 MoveInput { get; private set; }

        [Tooltip("���� ���� �Է� �� (Vector2)")]
        public Vector2 LookInput { get; private set; }

        [Tooltip("�޸��� �Է� ���� (bool)")]
        public bool IsRunning { get; private set; }

        [Tooltip("���� �Է� ���� (bool, 1������)")]
        public bool IsJumpPressed { get; private set; }

        // TODO: ����, ���� �� ���� �߰��� �׼ǿ� ���� ������Ƽ�� ���⿡ �����մϴ�.

        /// <summary>
        /// ������Ʈ�� Ȱ��ȭ�� �� Input Actions �ν��Ͻ��� �����ϰ� �̺�Ʈ�� �����մϴ�.
        /// </summary>
        private void OnEnable()
        {
            // Input Actions �ν��Ͻ��� ������ ����
            if (_input == null)
            {
                _input = new PlayerInputActions();
            }

            // Player �׼Ǹ� Ȱ��ȭ
            _input.Player.Enable();

            // �� �׼ǿ� ���� �ݹ�(�̺�Ʈ)�� ����(+=)
            _input.Player.Move.performed += OnMove;
            _input.Player.Move.canceled += OnMove;

            _input.Player.Look.performed += OnLook;
            _input.Player.Look.canceled += OnLook;

            _input.Player.Run.performed += OnRun;
            _input.Player.Run.canceled += OnRun;

            _input.Player.Jump.performed += OnJump;
        }

        /// <summary>
        /// ������Ʈ�� ��Ȱ��ȭ�� �� �̺�Ʈ ������ �����մϴ�.
        /// </summary>
        private void OnDisable()
        {
            _input.Player.Disable();

            // �޸� ���� ������ ���� ����ߴ� ��� �ݹ��� ����(-=)
            _input.Player.Move.performed -= OnMove;
            _input.Player.Move.canceled -= OnMove;

            _input.Player.Look.performed -= OnLook;
            _input.Player.Look.canceled -= OnLook;

            _input.Player.Run.performed -= OnRun;
            _input.Player.Run.canceled -= OnRun;

            _input.Player.Jump.performed -= OnJump;
        }

        /// <summary>
        /// ��� Update ������ ���� �� ȣ��Ǿ�, ������ ���� �Է��� �ʱ�ȭ�մϴ�.
        /// </summary>
        private void LateUpdate()
        {
            // ������ �� �����ӿ��� �����Ǿ�� �ϹǷ� �� ������ ���� false�� �ʱ�ȭ�մϴ�.
            IsJumpPressed = false;
        }

        // �� �ݹ� �޼���� private���� �����Ͽ� �ܺο��� ���� ȣ������ �ʵ��� �մϴ�.
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