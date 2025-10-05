// Assets/Scripts/Player/States/Wire/PlayerWireMoveState.cs
using Player.Animation;
using Player.Data;
using UnityEngine;

namespace Player.States
{
    public class PlayerWireMoveState : IState
    {
        private readonly Player _player;
        private readonly PlayerStateMachine _stateMachine;
        private readonly PlayerInput _input;
        private readonly PlayerMotor _motor;
        private readonly PlayerSO _data;
        private readonly PlayerAnimationController _animController;

        private Vector3 _currentVelocity; // 현재 관성 속도
        private bool _canAirDash; // 추가 대쉬 사용 가능 여부

        public PlayerWireMoveState(Player player, PlayerStateMachine stateMachine, PlayerInput input, PlayerMotor motor, PlayerSO data, PlayerAnimationController animController)
        {
            _player = player;
            _stateMachine = stateMachine;
            _input = input;
            _motor = motor;
            _data = data;
            _animController = animController;
        }

        public void Enter()
        {
            // 상태 진입 시, 카메라 방향으로 첫 대쉬를 '발사'
            Vector3 launchDirection = Camera.main.transform.forward;
            launchDirection.y = 0;
            _currentVelocity = launchDirection.normalized * _data.wireLaunchSpeed;

            // 추가 대쉬 사용 가능으로 초기화
            _canAirDash = true;

            // 충돌 이벤트 구독
            _motor.OnCollision += OnHitObject;

            // TODO: 와이어 이동 애니메이션 시작
        }

        public void Update()
        {
            // 1. 수동 해제 (우클릭 떼기)
            if (!_input.IsWireAiming)
            {
                HandleDetach();
                return;
            }

            // 2. 추가 대쉬 (LeftShift)
            if (_canAirDash && _input.IsRunning) // IsRunning이 LeftShift와 연결되어 있음
            {
                Vector3 dashDirection = Camera.main.transform.forward;
                dashDirection.y = 0;
                _currentVelocity = dashDirection.normalized * _data.wireAirDashSpeed;
                _canAirDash = false;
            }

            // 3. 궤도 수정 (A/D)
            float turnAmount = _input.MoveInput.x * _data.wireTurnSpeed * Time.deltaTime;
            Quaternion turnRotation = Quaternion.Euler(0f, turnAmount, 0f);
            _currentVelocity = turnRotation * _currentVelocity;

            // 4. 와이어 길이 체크 (스윙/탈출)
            CheckWireLength();

            // 5. 최종 속도로 모터에 이동 명령
            _motor.Move(_currentVelocity);

            // TODO: 현재 속도에 맞춰 애니메이션 파라미터 설정
        }

        public void Exit()
        {
            _stateMachine.WireTarget = null; // 와이어 타겟 정보 초기화

            // 충돌 이벤트 구독 해제
            _motor.OnCollision -= OnHitObject;
            // TODO: 와이어 이동 애니메이션 종료, 와이어 VFX 제거
            _animController.SetWireMove(false);
        }

        private void CheckWireLength()
        {
            if (_stateMachine.WireTarget == null) return;

            Vector3 playerPos = _player.transform.position;
            Vector3 targetPos = _stateMachine.WireTarget.position;

            // Y축을 포함한 실제 3D 거리 계산
            float distance = Vector3.Distance(playerPos, targetPos);

            if (distance > _data.wireMaxLength)
            {
                // 와이어 포인트에서 플레이어로 향하는 벡터 (원의 반지름)
                Vector3 radiusVector = (playerPos - targetPos).normalized;

                // 플레이어가 원 밖으로 향하고 있을 때만 스윙/탈출 판정
                if (Vector3.Dot(_currentVelocity, radiusVector) > 0)
                {
                    // 이동 방향과 반지름 벡터 사이의 각도 계산
                    float angle = Vector3.Angle(_currentVelocity.normalized, radiusVector);

                    // [탈출 조건] 각도가 120도 이상 벌어지면(거의 정면으로 당겨질 때) 탈출
                    if (angle > 120f)
                    {
                        // 현재 속도를 그대로 유지한 채 FallState로 전환 (관성 유지)
                        _motor.ApplyRawVelocity(_currentVelocity);
                        _stateMachine.ChangeState(_player.FallState);
                    }
                    // [스윙 조건]
                    else
                    {
                        // 속도를 원의 접선 방향으로 투영(Project)하여 부드럽게 방향을 틉니다.
                        _currentVelocity = Vector3.ProjectOnPlane(_currentVelocity, radiusVector);
                    }
                }
            }
        }

        private void OnHitObject(Collision collision)
        {
            // 땅과의 충돌은 무시
            if (collision.gameObject.layer == LayerMask.NameToLayer("Ground")) return;

            // 충돌 지점의 법선 벡터를 가져옵니다.
            Vector3 normal = collision.contacts[0].normal;

            // 캐릭터의 이동 방향(속도)과 충돌 지점의 법선이 거의 반대 방향일 때(정면 충돌)만 탈출합니다.
            // Vector3.Dot() 결과가 -1에 가까울수록 정면 충돌입니다.
            if (Vector3.Dot(_currentVelocity.normalized, normal) < -0.7f)
            {
                HandleDetach();
            }
        }

        /// <summary>
        /// 와이어를 해제하고 FallState로 전환하는 공통 로직
        /// </summary>
        private void HandleDetach()
        {
            //_motor.ResetVerticalVelocity();
            _motor.Jump(_data.wireDetachVerticalBonus);
            _stateMachine.ChangeState(_player.FallState);
        }
    }
}