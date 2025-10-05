// Assets/Scripts/Player/Movement/PlayerMotor.cs
using Player.Data;
using System;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class PlayerMotor : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private PlayerSO _data; // 각종 데이터를 가져오기 위한 참조

        // Rigidbody의 현재 속도를 외부에서 읽을 수 있도록 프로퍼티로 제공
        public Vector3 Velocity => _rigidbody.linearVelocity;
        public event Action<Collision> OnCollision;

        private void Awake()
        {
            if (_rigidbody == null) _rigidbody = GetComponent<Rigidbody>();
        }

        /// <summary>
        /// 물리 계산은 고정된 주기로 호출되는 FixedUpdate에서 처리해야 안정적입니다.
        /// </summary>
        private void FixedUpdate()
        {
            // 최대 속도를 초과하지 않도록 속도를 제한합니다.

            //LimitHorizontalVelocity();
        }

        /// <summary>
        /// MoveState로부터 호출되어, 원하는 방향으로 캐릭터를 움직입니다.
        /// </summary>
        public void Move(Vector3 desiredVelocity)
        {
            // Y축(수직) 속도는 유지한 채, X와 Z축(수평) 속도만 변경합니다.
            _rigidbody.linearVelocity = new Vector3(desiredVelocity.x, _rigidbody.linearVelocity.y, desiredVelocity.z);
        }

        /// <summary>
        /// JumpState로부터 호출되어, 캐릭터를 점프시킵니다.
        /// </summary>
        public void Jump(float jumpForce)
        {
            // 순간적인 힘을 가하여 위로 띄웁니다.
            _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        /// <summary>
        /// IdleState로부터 호출되어, 캐릭터를 멈춥니다.
        /// </summary>
        public void Stop()
        {
            // Y축 속도는 유지한 채 수평 속도만 0으로 만듭니다.
            _rigidbody.linearVelocity = new Vector3(0f, _rigidbody.linearVelocity.y, 0f);
        }

        /// <summary>
        /// 와이어 탈출 등, 특수한 상황에서 수직 속도를 0으로 초기화합니다.
        /// </summary>
        public void ResetVerticalVelocity()
        {
            _rigidbody.linearVelocity = new Vector3(_rigidbody.linearVelocity.x, 0, _rigidbody.linearVelocity.z);
        }

        /// <summary>
        /// 와이어 탈출 점프 등, 계산된 특정 속도를 Rigidbody에 직접 주입합니다.
        /// </summary>
        public void ApplyRawVelocity(Vector3 velocity)
        {
            _rigidbody.linearVelocity = velocity;
        }

        /// <summary>
        /// 캐릭터의 수평 속도가 최대 속도를 넘지 않도록 제한합니다.
        /// </summary>
        private void LimitHorizontalVelocity()
        {
            Vector3 horizontalVelocity = new Vector3(_rigidbody.linearVelocity.x, 0, _rigidbody.linearVelocity.z);

            // 현재 수평 속도가 최대 속도(runSpeed)를 초과하면
            if (horizontalVelocity.sqrMagnitude > _data.runSpeed * _data.runSpeed)
            {
                // 속도를 최대 속도로 제한합니다.
                Vector3 limitedVelocity = horizontalVelocity.normalized * _data.runSpeed;
                _rigidbody.linearVelocity = new Vector3(limitedVelocity.x, _rigidbody.linearVelocity.y, limitedVelocity.z);
            }
        }

        /// <summary>
        /// Rigidbody가 다른 콜라이더와 충돌을 시작했을 때 호출되는 Unity 내장 메시지입니다.
        /// </summary>
        private void OnCollisionEnter(Collision collision)
        {
            // 충돌 이벤트를 외부에 알립니다.
            OnCollision?.Invoke(collision);
        }
    }
}