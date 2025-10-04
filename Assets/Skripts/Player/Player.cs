// Assets/Scripts/Player/Player.cs
using Player.Animation;
using Player.Data;
using Player.States;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// �÷��̾��� ��� ������Ʈ�� ���¸� �Ѱ��ϴ� ���� ��Ʈ�ѷ��Դϴ�.
    /// FSM�� �ʱ�ȭ�ϰ� �����ϴ� ������ �մϴ�.
    /// </summary>
    public class Player : MonoBehaviour
    {
        [Header("Data")]
        [Tooltip("�÷��̾��� ��� �����͸� ��� �ִ� ScriptableObject �Դϴ�.")]
        public PlayerSO Data; // CharacterStats ��� PlayerSO ����

        [Header("Component References")]
        [Tooltip("�÷��̾��� �Է� ó����")]
        public PlayerInput Input;
        [Tooltip("�÷��̾��� ���� �ӽ�")]
        public PlayerStateMachine StateMachine;
        [Tooltip("�÷��̾��� �̵� ����")]
        public PlayerMotor Motor;
        [Tooltip("�ִϸ��̼� �߾� ��Ʈ�ѷ�")]
        public PlayerAnimationController AnimController;

        // --- ���� Ŭ���� �ν��Ͻ� ---
        public PlayerIdleState IdleState { get; private set; }
        public PlayerMoveState MoveState { get; private set; }
        public PlayerJumpState JumpState { get; private set; }
        public PlayerFallState FallState { get; private set; }
        // TODO: ���� Attack, Dodge ���� ���¸� ���⿡ �߰��մϴ�.

        /// <summary>
        /// ������ ���۵Ǳ� ��, ��� ������Ʈ�� ���¸� �ʱ�ȭ�մϴ�.
        /// </summary>
        private void Awake()
        {
            // ��� ���� Ŭ������ �ν��Ͻ��� �����մϴ�.
            // �� ��, �� ���°� �ʿ�� �ϴ� ��� ������Ʈ�� ������ '������'�� ���� �������ݴϴ�. (������ ����)
            IdleState = new PlayerIdleState(this, StateMachine, Input, Motor, AnimController);
            MoveState = new PlayerMoveState(this, StateMachine, Input, Motor, Data, AnimController);
            JumpState = new PlayerJumpState(this, StateMachine, Motor, Data, AnimController);
            FallState = new PlayerFallState(this, StateMachine, Motor, AnimController);
        }

        /// <summary>
        /// ù �������� ������Ʈ�Ǳ� ��, ���� �ӽ��� ���� ���·� �ʱ�ȭ�մϴ�.
        /// </summary>
        private void Start()
        {
            // FSM�� ���� ���¸� IdleState�� �����մϴ�.
            StateMachine.Initialize(IdleState);
        }
    }
}