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

        // 캐릭터의 현재 월드 수평속도 반환
        public Vector3 HorizontalVelocity => new Vector3(_controller.velocity.x, 0, _controller.velocity.z);
        // 캐릭터의 현재 월드 수직속도 반환
        public float VerticalVelocity => _verticalVelocity.y;

        // 외부(State)에서 전달받는 값
        private Vector3 _movementVelocity;

        // 내부(Motor)에서만 관리하는 값
        private Vector3 _verticalVelocity;

        private void Awake()
        {
            // 컴포넌트가 할당되지 않았다면 직접 찾아옵니다.
            if (_controller == null)
                _controller = GetComponent<CharacterController>();
        }

        private void Update()
        {
            // 지면 감지
            bool isGrounded = _stateMachine.IsGrounded;

            if (isGrounded && _verticalVelocity.y < 0)
            {
                // 땅에 있을 경우:
                // 수직 속도를 바닥에 붙어있을 정도로만 유지하고, 중력을 누적시키지 않습니다.
                _verticalVelocity.y = -2f;
            }
            else
            {
                // 공중에 있을 경우:
                // 매 프레임 중력을 적용하여 수직 속도를 누적시킵니다.
                _verticalVelocity.y += _gravity * Time.deltaTime;
            }


            // 상태가 전달한 수평 이동과 내부에서 계산한 수직 이동을 합칩니다.
            Vector3 finalVelocity = _movementVelocity + _verticalVelocity;

            // 최종 계산된 속도로 CharacterController를 움직입니다.
            _controller.Move(finalVelocity * Time.deltaTime);

            // 매 프레임 수평 이동 속도를 초기화하여, 명령이 없으면 멈추도록 합니다.
            _movementVelocity = Vector3.zero;
        }

        /// <summary>
        /// 상태(State)로부터 매 프레임 호출될 이동 명령입니다.
        /// </summary>
        public void Move(Vector3 movement)
        {
            _movementVelocity = movement;
        }

        /// <summary>
        /// 상태(State)로부터 호출될 점프 명령입니다.
        /// </summary>
        public void Jump(float jumpHeight)
        {
            // 땅에 있을 때만 점프가 가능하도록 합니다.
            if (_stateMachine.IsGrounded)
            {
                _verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * _gravity);
            }
        }
    }
}