// Assets/Scripts/Player/Movement/PlayerMotor.cs
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMotor : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CharacterController _controller;
        [SerializeField] private PlayerStateMachine _stateMachine;

        [Header("Settings")]
        [SerializeField] private float _gravity = -19.62f;

        // ĳ������ ���� ���� ����ӵ� ��ȯ
        public Vector3 HorizontalVelocity => new Vector3(_controller.velocity.x, 0, _controller.velocity.z);
        // ĳ������ ���� ���� �����ӵ� ��ȯ
        public float VerticalVelocity => _verticalVelocity.y;

        // �ܺ�(State)���� ���޹޴� ��
        private Vector3 _movementVelocity;

        // ����(Motor)������ �����ϴ� ��
        private Vector3 _verticalVelocity;

        private void Awake()
        {
            // ������Ʈ�� �Ҵ���� �ʾҴٸ� ���� ã�ƿɴϴ�.
            if (_controller == null)
                _controller = GetComponent<CharacterController>();
        }

        private void Update()
        {
            // ���� ����
            bool isGrounded = _stateMachine.IsGrounded;

            if (isGrounded && _verticalVelocity.y < 0)
            {
                // ���� ���� ���:
                // ���� �ӵ��� �ٴڿ� �پ����� �����θ� �����ϰ�, �߷��� ������Ű�� �ʽ��ϴ�.
                _verticalVelocity.y = -2f;
            }
            else
            {
                // ���߿� ���� ���:
                // �� ������ �߷��� �����Ͽ� ���� �ӵ��� ������ŵ�ϴ�.
                _verticalVelocity.y += _gravity * Time.deltaTime;
            }


            // ���°� ������ ���� �̵��� ���ο��� ����� ���� �̵��� ��Ĩ�ϴ�.
            Vector3 finalVelocity = _movementVelocity + _verticalVelocity;

            // ���� ���� �ӵ��� CharacterController�� �����Դϴ�.
            _controller.Move(finalVelocity * Time.deltaTime);

            // �� ������ ���� �̵� �ӵ��� �ʱ�ȭ�Ͽ�, ����� ������ ���ߵ��� �մϴ�.
            _movementVelocity = Vector3.zero;
        }

        /// <summary>
        /// ����(State)�κ��� �� ������ ȣ��� �̵� ����Դϴ�.
        /// </summary>
        public void Move(Vector3 movement)
        {
            _movementVelocity = movement;
        }

        /// <summary>
        /// ����(State)�κ��� ȣ��� ���� ����Դϴ�.
        /// </summary>
        public void Jump(float jumpHeight)
        {
            // ���� ���� ���� ������ �����ϵ��� �մϴ�.
            if (_stateMachine.IsGrounded)
            {
                _verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * _gravity);
            }
        }
    }
}