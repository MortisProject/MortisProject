// Assets/Scripts/Player/Animation/PlayerAnimationController.cs
using UnityEngine;

namespace Player.Animation
{
    /// <summary>
    /// Animator ������Ʈ�� ���� �����ϴ� �߾� ��Ʈ�ѷ�.
    /// ��� ����(State)�� �� ��ũ��Ʈ�� ���� �ִϸ��̼� ����� ��û�մϴ�.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class PlayerAnimationController : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("�ִϸ��̼� �Ķ���Ͱ� ��ǥ ������ ���ϴ� �� �ɸ��� �ð��Դϴ�.")]
        [SerializeField] private float _smoothTime = 0.1f;

        // Animator �Ķ���� �̸��� �̸� �ؽð����� ��ȯ�Ͽ� ���� ���
        private readonly int _moveXHash = Animator.StringToHash("MoveX");
        private readonly int _moveYHash = Animator.StringToHash("MoveY");
        private readonly int _jumpHash = Animator.StringToHash("Jump");
        private readonly int _landHash = Animator.StringToHash("Land");

        // TODO: ���� ����, ���� ���� �ִϸ��̼� �ؽð��� ���⿡ �߰��մϴ�.
        // private readonly int _attackTriggerHash = Animator.StringToHash("Attack");

        private Animator _animator;

        // ����(State)�� ������ ��ǥ �̵� ��
        private Vector2 _targetMove;

        // SmoothDamp�� ���� ���� �ӵ� �� (���������� ����)
        private Vector2 _currentMoveVelocity;

        private void Awake()
        {
            // Animator ������Ʈ�� ĳ��
            _animator = GetComponent<Animator>();
        }

        /// <summary>
        /// �� ������ ȣ��Ǿ� ���� �ִϸ��̼� ���� ��ǥ ������ �ε巴�� �̵���ŵ�ϴ�.
        /// </summary>
        private void Update()
        {
            // ���� �ִϸ������� MoveX, MoveY ���� �����ɴϴ�.
            float currentX = _animator.GetFloat(_moveXHash);
            float currentY = _animator.GetFloat(_moveYHash);

            
            // Mathf.SmoothDamp�� ����Ͽ� ���� ���� ��ǥ ������ �ε巴�� �����մϴ�.
            float smoothedX = Mathf.SmoothDamp(currentX, _targetMove.x, ref _currentMoveVelocity.x, _smoothTime);
            float smoothedY = Mathf.SmoothDamp(currentY, _targetMove.y, ref _currentMoveVelocity.y, _smoothTime);

            // ������ 0�� �����ϸ� 0���� ����
            if (Mathf.Abs(smoothedX) < 0.01f) smoothedX = 0f;
            if (Mathf.Abs(smoothedY) < 0.01f) smoothedY = 0f;

            // ���� ���� �ε巯�� ���� �ִϸ����Ϳ� ���� �����մϴ�. (dampTime ����)
            _animator.SetFloat(_moveXHash, smoothedX);
            _animator.SetFloat(_moveYHash, smoothedY);
        }

        /// <summary>
        /// �̵� �ִϸ��̼��� ���� Ʈ�� ���� �����մϴ�.
        /// </summary>
        /// <param name="x">�¿� �̵� ��</param>
        /// <param name="y">�յ� �̵� ��</param>
        public void SetMove(float x, float y)
        {
            _targetMove.x = x;
            _targetMove.y = y;
        }

        /// <summary>
        /// ���� �ִϸ��̼��� ����մϴ�.
        /// </summary>
        public void PlayJump()
        {
            _animator.SetTrigger(_jumpHash);
        }

        /// <summary>
        /// ���� �ִϸ��̼��� ����մϴ�.
        /// </summary>
        public void PlayLand()
        {
            _animator.SetTrigger(_landHash);
        }
    }
}