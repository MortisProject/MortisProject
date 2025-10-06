// Assets/Scripts/Player/Player.cs
using Player.Animation;
using Player.Data;
using Player.States;
using Unity.Cinemachine;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    /// <summary>
    /// 플레이어의 모든 컴포넌트와 상태를 총괄하는 메인 컨트롤러입니다.
    /// FSM을 초기화하고 실행하는 역할을 합니다.
    /// </summary>
    public class Player : MonoBehaviour
    {
        [Header("Data")]
        [Tooltip("플레이어의 모든 데이터를 담고 있는 ScriptableObject 입니다.")]
        public PlayerSO Data; //대신 PlayerSO 참조

        [Header("Component References")]
        [Tooltip("플레이어의 입력 처리기")]
        public PlayerInput Input;
        [Tooltip("플레이어의 스텟")]
        public CharacterStats Stats;
        [Tooltip("플레이어의 상태 머신")]
        public PlayerStateMachine StateMachine;
        [Tooltip("플레이어의 이동 모터")]
        public PlayerMotor Motor;
        [Tooltip("애니메이션 중앙 컨트롤러")]
        public PlayerAnimationController AnimController;
        [Tooltip("플레이어의 콜라이더")]
        public CapsuleCollider CapsuleCollider;
        [Tooltip("와이어가 시작될 위치의 Transform 입니다.")]
        public Transform WireOrigin;
        [Tooltip("와이어의 시각적 표현을 담당하는 스크립트입니다.")]
        public WireRenderer WireRenderer;

        [Header("Physics Materials")]
        public PhysicsMaterial HighFrictionMaterial;
        public PhysicsMaterial FrictionlessMaterial;

        [Header("UI References")]
        [Tooltip("와이어 조준점(Reticle)으로 사용할 UI Image 입니다.")]
        public Image BestWireReticuleUI; // Best Target UI
        public Image NormalWireReticuleUI; // Normal Target UI
        public GameObject WireFireEffectPrefab; // 발사 효과 프리팹

        [Header("Camera References")]
        public CinemachineCamera AimCamera; // 조준용 가상 카메라

        // --- 상태 클래스 인스턴스 ---
        public PlayerIdleState IdleState { get; private set; }
        public PlayerMoveState MoveState { get; private set; }
        public PlayerJumpState JumpState { get; private set; }
        public PlayerFallState FallState { get; private set; }
        public PlayerWireAimState WireAimState { get; private set; }
        public PlayerWireLaunchState WireLaunchState { get; private set; }
        public PlayerWireMoveState WireMoveState { get; private set; }
        // TODO: 추후 Attack, Dodge 등의 상태를 여기에 추가합니다.

        /// <summary>
        /// 게임이 시작되기 전, 모든 컴포넌트와 상태를 초기화합니다.
        /// </summary>
        private void Awake()
        {
            // 모든 상태 클래스의 인스턴스를 생성합니다.
            // 이 때, 각 상태가 필요로 하는 모든 컴포넌트와 참조를 '생성자'를 통해 전달해줍니다. (의존성 주입)
            IdleState = new PlayerIdleState(this, StateMachine, Input, Motor, AnimController);
            MoveState = new PlayerMoveState(this, StateMachine, Input, Motor, Data, AnimController);
            JumpState = new PlayerJumpState(this, StateMachine, Input, Motor, Data, AnimController);
            FallState = new PlayerFallState(this, StateMachine, Input, Motor, Data, AnimController);
            WireAimState = new PlayerWireAimState(this, StateMachine, Input, Data, Stats);
            WireLaunchState = new PlayerWireLaunchState(this, StateMachine, Motor, Data, AnimController);
            WireMoveState = new PlayerWireMoveState(this, StateMachine, Input, Motor, Data, AnimController);
        }

        /// <summary>
        /// 첫 프레임이 업데이트되기 전, 상태 머신을 시작 상태로 초기화합니다.
        /// </summary>
        private void Start()
        {
            // FSM의 시작 상태를 IdleState로 지정합니다.
            StateMachine.Initialize(IdleState);
        }
    }
}