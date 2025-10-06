// Assets/Scripts/Player/Movement/PlayerMotor.cs
using Player.Data;
using System;
using UnityEngine;
using System.Collections;

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
        private Coroutine _hoverCoroutine;

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

           LimitHorizontalVelocity();
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
        /// AirborneState로부터 호출되어, 공중에서 캐릭터에 힘을 가합니다.
        /// </summary>
        public void AirMove(Vector3 direction, float force)
        {
            // AddForce는 현재 속도에 힘을 더하는 방식이라 관성이 유지됩니다.
            _rigidbody.AddForce(direction * force);
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
            // 현재 속도에서 수직(Y) 성분을 제외하여 순수 수평 속도를 계산합니다.
            Vector3 horizontalVelocity = new Vector3(_rigidbody.linearVelocity.x, 0, _rigidbody.linearVelocity.z);

            // 현재 수평 속도가 PlayerSO에 정의된 최대 수평 속도를 초과하면
            if (horizontalVelocity.sqrMagnitude > _data.maxHorizontalSpeed * _data.maxHorizontalSpeed)
            {
                // 속도를 최대 속도로 제한합니다. (이동 방향은 유지)
                Vector3 limitedVelocity = horizontalVelocity.normalized * _data.maxHorizontalSpeed;
                // 제한된 수평 속도와 원래의 수직 속도를 합쳐 최종 속도를 설정합니다.
                _rigidbody.linearVelocity = new Vector3(limitedVelocity.x, _rigidbody.linearVelocity.y, limitedVelocity.z);
            }
        }
        /// <summary>
        /// 지정된 시간 동안 캐릭터의 중력을 무시하고 공중에 떠 있도록 합니다.
        /// </summary>
        /// <param name="duration">체공할 시간(초)</param>
        public void Hover(float duration)
        {
            // 이전에 실행 중이던 Hover 코루틴이 있다면 중지
            if (_hoverCoroutine != null)
            {
                StopCoroutine(_hoverCoroutine);
            }
            // 새로운 Hover 코루틴 시작
            _hoverCoroutine = StartCoroutine(HoverCoroutine(duration));
        }

        private IEnumerator HoverCoroutine(float duration)
        {
            // 1. 중력을 끕니다.
            _rigidbody.useGravity = false;
            // 2. 현재의 모든 수직 속도를 0으로 만들어 그 자리에 멈추게 합니다.
            _rigidbody.linearVelocity = new Vector3(_rigidbody.linearVelocity.x, 0, _rigidbody.linearVelocity.z);

            // 3. 지정된 시간만큼 기다립니다.
            yield return new WaitForSeconds(duration);

            // 4. 시간이 지나면 다시 중력을 켭니다.
            _rigidbody.useGravity = true;
            _hoverCoroutine = null;
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