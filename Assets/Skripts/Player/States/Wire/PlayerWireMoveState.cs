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
            launchDirection.y = 0; // 수평으로만 발사
            _currentVelocity = launchDirection.normalized * _data.wireLaunchSpeed;

            // 추가 대쉬 사용 가능으로 초기화
            _canAirDash = true; 

            // 충돌 이벤트 구독
            _motor.OnControllerHit += OnHitWall;

            // TODO: 와이어 이동 애니메이션 시작
        }

        public void Update()
        {
            // 1. 수동 해제 (Spacebar)
            if (_input.IsJumpPressed)
            {
                _stateMachine.ChangeState(_player.FallState);
                return;
            }

            // 2. 추가 대쉬 (LeftShift)
            if (_canAirDash && Input.GetKey(KeyCode.LeftShift))
            {
                Vector3 dashDirection = Camera.main.transform.forward;
                dashDirection.y = 0;
                _currentVelocity = dashDirection.normalized * _data.wireAirDashSpeed;
                _canAirDash = false; // 대쉬는 한 번만 사용 가능
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
            // 충돌 이벤트 구독 해제
            _motor.OnControllerHit -= OnHitWall;
            // TODO: 와이어 이동 애니메이션 종료, 와이어 VFX 제거
        }

        private void CheckWireLength()
        {
            if (_stateMachine.WireTarget == null) return;

            Vector3 playerPos = _player.transform.position;
            Vector3 targetPos = _stateMachine.WireTarget.position;
            float distance = Vector3.Distance(playerPos, targetPos);

            // 플레이어가 와이어 최대 길이를 벗어났다면
            if (distance > _data.wireMaxLength)
            {
                // 스윙/탈출 판별 로직
                Vector3 radiusVector = (playerPos - targetPos).normalized;

                if (Vector3.Dot(_currentVelocity, radiusVector) > 0)
                {
                    // 속도를 와이어 최대 길이의 구 표면에 반사시켜 궤도를 자연스럽게 휘게 함
                    _currentVelocity = Vector3.Reflect(_currentVelocity, -radiusVector);
                }
            }
        }

        // 충돌시 호출될 메서드
        private void OnHitWall()
        {
            _stateMachine.ChangeState(_player.FallState);
        }
    }
}