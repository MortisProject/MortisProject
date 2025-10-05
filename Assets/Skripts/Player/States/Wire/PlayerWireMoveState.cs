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
        private SpringJoint _springJoint;

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
            _player.CapsuleCollider.material = _player.FrictionlessMaterial; // 마찰력 없는 재질로 교체

            // 1. 플레이어 오브젝트에 SpringJoint 컴포넌트를 동적으로 추가합니다.
            _springJoint = _player.gameObject.AddComponent<SpringJoint>();

            // 2. SpringJoint를 설정합니다.
            _springJoint.autoConfigureConnectedAnchor = false;
            _springJoint.connectedAnchor = _stateMachine.WireTarget.position;

            // 와이어의 최소/최대 길이를 설정합니다. (거의 늘어나지 않도록 비슷하게 설정)
            _springJoint.minDistance = 0.5f;
            _springJoint.maxDistance = Vector3.Distance(_player.transform.position, _stateMachine.WireTarget.position) * 0.8f;

            // SO 데이터에서 탄성/감쇠 값을 가져옵니다.
            _springJoint.spring = _data.wireSpringForce;
            _springJoint.damper = _data.wireDamper;

            // 3. 초기 발사 속도를 부여합니다. (이전과 동일)
            Vector3 launchDirection = Camera.main.transform.forward;
            launchDirection.y = 0;
            _motor.ApplyRawVelocity(launchDirection.normalized * _data.wireLaunchSpeed);
            _player.CapsuleCollider.material = _player.FrictionlessMaterial;
            _motor.OnCollision += OnHitObject;

            // 추가 대쉬 사용 가능으로 초기화
            _canAirDash = true;

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

            Vector3 controlDirection = new Vector3(_input.MoveInput.x, 0, _input.MoveInput.y);
            _motor.AirMove(controlDirection, _data.airControlForce * 0.5f); // 공중 제어 힘을 약하게 적용


            // 2. 추가 대쉬 (LeftShift)
            if (_canAirDash && _input.IsRunning) // IsRunning이 LeftShift와 연결되어 있음
            {
                Vector3 dashDirection = Camera.main.transform.forward;
                dashDirection.y = 0;
                _currentVelocity = dashDirection.normalized * _data.wireAirDashSpeed;
                _canAirDash = false;
            }

            // TODO: 현재 속도에 맞춰 애니메이션 파라미터 설정
        }

        public void Exit()
        {
            if (_springJoint != null)
            {
                Object.Destroy(_springJoint);
            }

            _player.WireRenderer.Deactivate();

            _stateMachine.WireTarget = null; // 와이어 타겟 정보 초기화

            // 충돌 이벤트 구독 해제
            _motor.OnCollision -= OnHitObject;
            // TODO: 와이어 이동 애니메이션 종료, 와이어 VFX 제거
            _animController.SetWireMove(false);

            _player.CapsuleCollider.material = _player.HighFrictionMaterial; // 원래의 마찰력 높은 재질로 복구
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