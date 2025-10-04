// Assets/Scripts/Player/Player.cs
using Player.Animation;
using Player.Data;
using Player.States;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

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
        public PlayerSO Data; // CharacterStats 대신 PlayerSO 참조

        [Header("Component References")]
        [Tooltip("플레이어의 입력 처리기")]
        public PlayerInput Input;
        [Tooltip("플레이어의 상태 머신")]
        public PlayerStateMachine StateMachine;
        [Tooltip("플레이어의 이동 모터")]
        public PlayerMotor Motor;
        [Tooltip("애니메이션 중앙 컨트롤러")]
        public PlayerAnimationController AnimController;

        // --- 상태 클래스 인스턴스 ---
        public PlayerIdleState IdleState { get; private set; }
        public PlayerMoveState MoveState { get; private set; }
        public PlayerJumpState JumpState { get; private set; }
        public PlayerFallState FallState { get; private set; }
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
            JumpState = new PlayerJumpState(this, StateMachine, Motor, Data, AnimController);
            FallState = new PlayerFallState(this, StateMachine, Motor, AnimController);
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