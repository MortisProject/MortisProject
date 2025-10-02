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
        // Animator �Ķ���� �̸��� �̸� �ؽð����� ��ȯ�Ͽ� ���� ���
        private readonly int _moveXHash = Animator.StringToHash("MoveX");
        private readonly int _moveYHash = Animator.StringToHash("MoveY");
        private readonly int _jumpHash = Animator.StringToHash("Jump");

        // TODO: ���� ����, ���� ���� �ִϸ��̼� �ؽð��� ���⿡ �߰��մϴ�.
        // private readonly int _attackTriggerHash = Animator.StringToHash("Attack");

        private Animator _animator;

        private void Awake()
        {
            // Animator ������Ʈ�� ĳ��
            _animator = GetComponent<Animator>();
        }

        /// <summary>
        /// �̵� �ִϸ��̼��� ���� Ʈ�� ���� �����մϴ�.
        /// </summary>
        /// <param name="x">�¿� �̵� ��</param>
        /// <param name="y">�յ� �̵� ��</param>
        public void SetMove(float x, float y)
        {
            _animator.SetFloat(_moveXHash, x, 0.1f, Time.deltaTime);
            _animator.SetFloat(_moveYHash, y, 0.1f, Time.deltaTime);
        }

        /// <summary>
        /// ���� �ִϸ��̼��� ����մϴ�.
        /// </summary>
        public void PlayJump()
        {
            _animator.SetTrigger(_jumpHash);
        }
    }
}