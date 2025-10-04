// Assets/Scripts/Player/Core/PlayerStateMachine.cs
using Player.Animation;
using Player.States;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Player
{
    public class PlayerStateMachine : MonoBehaviour
    {
        [Header("Ground Check Settings")]
        [Tooltip("�������� �ν��� ���̾ �����մϴ�.")]
        [SerializeField] private LayerMask _groundLayerMask;

        [Tooltip("������ ������ SphereCast�� �������Դϴ�.")]
        [SerializeField] private Transform _footTransform;

        [Tooltip("���� ���� �Ÿ��Դϴ�. ĳ���� �ߺ��� ��¦ �Ʒ����� ��� ª�� �Ÿ����� �մϴ�.")]
        [SerializeField] private float _groundCheckDistance = 0.1f;

        [Tooltip("���� ���� SphereCast�� �������Դϴ�.")]
        [SerializeField] private float _groundCheckRadius = 0.2f;

        [Header("Component References")]
        [SerializeField] private PlayerAnimationController _animController;

        // Push Pop State�� ����ϱ����� ����
        private readonly List<IState> _stateStack = new List<IState>();

        // ���� ����
        public IState CurrentState => _stateStack.LastOrDefault();

        // ���� ���� ���� (��� ���¿��� �� ���� ����)
        public bool IsGrounded { get; private set; }

        /// <summary>
        /// ĳ���Ͱ� ���߿� �� �ִ� �ð��� ����մϴ�.
        /// </summary>
        public float Flytime { get; private set; }

        /// <summary>
        /// �� �����Ӹ��� ȣ��˴ϴ�.
        /// ���� ������ �����ϱ� ���� ���� ���� ������ �����մϴ�.
        /// </summary>
        private void Update()
        {

            CheckGrounded();

            _animController.SetGrounded(IsGrounded);

            if (IsGrounded)
            {
                // ���� ������ Flytime�� 0���� �ʱ�ȭ
                Flytime = 0f;
            }
            else
            {
                // ���߿� ������ Flytime�� ��� ������Ŵ
                Flytime += Time.deltaTime;
            }
            CurrentState?.Update();
        }

        /// <summary>
        /// ���� �ӽ��� Ư�� ���·� �ʱ�ȭ�մϴ�.
        /// ������ ���� ���ο� ���� �߰�
        /// </summary>
        public void Initialize(IState startingState)
        {
            _stateStack.Clear();
            _stateStack.Add(startingState);
            CurrentState.Enter();
        }

        /// <summary>
        /// ���� ���¸� ���ο� ���·� ��ü �մϴ�.
        /// </summary>
        public void ChangeState(IState newState)
        {
            if (CurrentState != null)
            {
                CurrentState.Exit();
                _stateStack.Remove(CurrentState);
            }

            Debug.Log($"[State] {CurrentState} -> {newState}");

            _stateStack.Add(newState);
            newState.Enter();
        }

        /// <summary>
        /// �ܺ��� �������� �̺�Ʈ�� ���� ���� ���¸� '��ü'�մϴ�. (��: �ǰ�)
        /// �̸��� �ٸ� ��, ����� ChangeState�� �����ϸ� "�ܺο��� ȣ��ȴ�"�� �ǵ��� ��Ȯ�� �մϴ�.
        /// </summary>
        public void ForceChangeState(IState newState)
        {
            ChangeState(newState);
        }

        /// <summary>
        /// ���� ���� ���� ���ο� ���¸� '�߰�'�մϴ�. ���� ���´� �Ͻ������˴ϴ�. (��: Move -> ItemUse)
        /// </summary>
        public void PushState(IState newState)
        {
            _stateStack.Add(newState);
            newState.Enter();
        }

        /// <summary>
        /// ���� ���¸� '����'�ϰ� ���� ���·� ���ư��ϴ�. (��: ItemUse -> Move)
        /// </summary>
        public void PopState()
        {
            if (CurrentState != null)
            {
                CurrentState.Exit();
                _stateStack.Remove(CurrentState);
            }
        }

        /// <summary>
        /// �߹����� SphereCast�� ��� ���� ���� ���θ� Ȯ���ϰ� IsGrounded ���� ������Ʈ�մϴ�.
        /// </summary>
        private void CheckGrounded()
        {
            // Physics.SphereCast�� true�� ��ȯ�ϸ�(����� �浹�ϸ�) IsGrounded�� true�� �˴ϴ�.
            IsGrounded = Physics.SphereCast(
                _footTransform.position,
                _groundCheckRadius,
                Vector3.down,
                out RaycastHit hit,
                _groundCheckDistance,
                _groundLayerMask);
        }

#if UNITY_EDITOR
        /// <summary>
        /// ����Ƽ �������� Scene �信���� �۵��ϸ�, ����� �������� ������ �׷��ݴϴ�.
        /// </summary>
        private void OnDrawGizmos()
        {
            if (_footTransform == null) return;

            Gizmos.color = Color.red;

            // SphereCast�� �������� ������ ����մϴ�.
            Vector3 origin = _footTransform.position;
            Vector3 destination = origin + Vector3.down * _groundCheckDistance;

            // SphereCast�� ��θ� ������ �׸���, ���� ��ġ�� ��ü�� �׸��ϴ�.
            Gizmos.DrawLine(origin, destination);
            Gizmos.DrawWireSphere(destination, _groundCheckRadius);
        }
        /// <summary>
        /// ������� ���� ���� ���¿� IsGrounded ���� ���� ȭ�鿡 ǥ���մϴ�.
        /// </summary>
        private void OnGUI()
        {
            GUI.color = Color.black;
            GUI.Label(new Rect(10, 10, 500, 20), $"Current State: {CurrentState}");
            GUI.Label(new Rect(10, 30, 500, 20), $"IsGrounded: {IsGrounded}");
        }
#endif
    }
}