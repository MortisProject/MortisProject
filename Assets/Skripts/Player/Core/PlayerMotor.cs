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

        [Header("Dodge State (For Animation Events)")]
        [Tooltip("현재 회피로 인한 이동 속도입니다.")]
        [SerializeField] private float _currentDodgeSpeed;
        [Tooltip("회피 이동 방향입니다.")]
        [SerializeField] private Vector3 _dodgeDirection;
        private bool _isDodging = false;
        private bool _isDodgeDecelerating = false;

        public Vector3 Velocity => _rigidbody.linearVelocity;
        public event Action<Collision> OnCollision;

        private void Awake()
        {
            if (_rigidbody == null) _rigidbody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            // 최대 속도를 초과하지 않도록 속도를 제한

            if (_isDodging)
            {
                // 감속 중일 경우 속도를 부드럽게 0으로 줄여나감
                if (_isDodgeDecelerating)
                {
                    _currentDodgeSpeed = Mathf.Lerp(_currentDodgeSpeed, 0f, Time.fixedDeltaTime * _data.dodgeDeceleration);
                }
                _rigidbody.linearVelocity = _dodgeDirection * _currentDodgeSpeed;
            }
            else // 회피 중이 아닐 때만 기존 속도 제한 로직을 적용
            {
                LimitHorizontalVelocity();
            }
        }

        /// <summary>
        /// MoveState로부터 호출되어, 원하는 방향으로 캐릭터를 움직임
        /// </summary>
        public void Move(Vector3 desiredVelocity)
        {
            // Y축(수직) 속도는 유지한 채, X와 Z축(수평) 속도만 변경
            _rigidbody.linearVelocity = new Vector3(desiredVelocity.x, _rigidbody.linearVelocity.y, desiredVelocity.z);
        }

        /// <summary>
        /// AirborneState로부터 호출되어, 공중에서 캐릭터에 힘을준다.
        /// </summary>
        public void AirMove(Vector3 direction, float force)
        {
            // AddForce는 현재 속도에 힘을 더하는 방식이라 관성이 유지
            _rigidbody.AddForce(direction * force);
        }

        /// <summary>
        /// JumpState로부터 호출되어, 캐릭터를 점프시킴
        /// </summary>
        public void Jump(float jumpForce)
        {
            _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        /// <summary>
        /// IdleState로부터 호출되어, 캐릭터를 멈춤
        /// </summary>
        public void Stop()
        {
            // Y축 속도는 유지한 채 수평 속도만 0으로 만듦
            _rigidbody.linearVelocity = new Vector3(0f, _rigidbody.linearVelocity.y, 0f);
        }

        /// <summary>
        /// 와이어 탈출 등, 특수한 상황에서 수직 속도를 0으로 초기화
        /// </summary>
        public void ResetVerticalVelocity()
        {
            _rigidbody.linearVelocity = new Vector3(_rigidbody.linearVelocity.x, 0, _rigidbody.linearVelocity.z);
        }

        /// <summary>
        /// 와이어 탈출 점프 등, 계산된 특정 속도를 Rigidbody에 직접 주입
        /// </summary>
        public void ApplyRawVelocity(Vector3 velocity)
        {
            _rigidbody.linearVelocity = velocity;
        }

        /// <summary>
        /// 캐릭터의 수평 속도가 최대 속도를 넘지 않도록 제한
        /// </summary>
        private void LimitHorizontalVelocity()
        {
            // 현재 속도에서 수직(Y) 성분을 제외하여 순수 수평 속도를 계산
            Vector3 horizontalVelocity = new Vector3(_rigidbody.linearVelocity.x, 0, _rigidbody.linearVelocity.z);

            // 현재 수평 속도가 PlayerSO에 정의된 최대 수평 속도를 초과하면
            if (horizontalVelocity.sqrMagnitude > _data.maxHorizontalSpeed * _data.maxHorizontalSpeed)
            {
                // 속도를 최대 속도로 제한 (이동 방향은 유지)
                Vector3 limitedVelocity = horizontalVelocity.normalized * _data.maxHorizontalSpeed;
                // 제한된 수평 속도와 원래의 수직 속도를 합쳐 최종 속도를 설정
                _rigidbody.linearVelocity = new Vector3(limitedVelocity.x, _rigidbody.linearVelocity.y, limitedVelocity.z);
            }
        }

        /// <summary>
        /// Rigidbody에 추가적인 하강 힘을 가하여 중력을 강화하는 효과를냄
        /// </summary>
        /// <param name="multiplier">적용할 중력 배율</param>
        public void ApplyGravityForce(float multiplier)
        {
            // 기본 중력(Physics.gravity)에 배율을 곱하여 추가 힘을 계산
            // ForceMode.Acceleration은 질량에 관계없이 일정한 가속도를 적용
            _rigidbody.AddForce(Physics.gravity * (multiplier - 1f), ForceMode.Acceleration);
        }

        /// <summary>
        /// Rigidbody가 다른 콜라이더와 충돌을 시작했을 때 호출되는 Unity 내장 메시지
        /// </summary>
        private void OnCollisionEnter(Collision collision)
        {
            // 충돌 이벤트를 외부에 알림
            OnCollision?.Invoke(collision);
        }

        /// <summary>
        /// 공격자의 위치를 기반으로 플레이어를 반대 방향으로 밀어냄
        /// </summary>
        /// <param name="attackerPosition">공격자의 위치</param>
        /// <param name="force">밀려나는 힘의 크기</param>
        public void ApplyKnockback(Vector3 attackerPosition, float force)
        {
            _rigidbody.linearVelocity = Vector3.zero;
            
            // 공격자로부터 플레이어를 향하는 방향을 계산
            Vector3 knockbackDirection = (transform.position - attackerPosition).normalized;
            knockbackDirection.y = 0; // 수평으로만 밀려나도록 y값을 0으로 설정
            
            _rigidbody.AddForce(knockbackDirection * force, ForceMode.Impulse);
        }

        #region Animation Event Methods

        /// <summary>
        /// (AnimEvent) 회피 이동
        /// </summary>
        public void StartDodgeMovement()
        {
            _isDodging = true;
            _isDodgeDecelerating = false;

            Vector3 cameraForward = Camera.main.transform.forward;
            cameraForward.y = 0; // y축 값을 0으로 만들어 수평 방향만 사용
            _dodgeDirection = -cameraForward.normalized; // 정규화하여 방향 벡터로 만듬

            _currentDodgeSpeed = _data.dodgeInitialSpeed; // 초기 속도로 시작
        }

        /// <summary>
        /// (AnimEvent) 회피 속도를 최대치로 설정
        /// </summary>
        public void SetDodgeMaxSpeed()
        {
            _currentDodgeSpeed = _data.dodgeMaxSpeed;
        }

        /// <summary>
        /// (AnimEvent) 회피 감속을 시작
        /// </summary>
        public void StartDodgeDeceleration()
        {
            _isDodgeDecelerating = true;
        }

        /// <summary>
        /// (AnimEvent) 회피 이동을 완전히 종료
        /// </summary>
        public void EndDodgeMovement()
        {
            _isDodging = false;
            _isDodgeDecelerating = false;
            _currentDodgeSpeed = 0f;
            _rigidbody.linearVelocity = Vector3.zero; // 혹시 모를 잔여 속도 제거
        }

        #endregion
    }
}