// Assets/Scripts/Player/Animation/PlayerAnimationEvents.cs
using UnityEngine;

namespace Player.Animation
{
    /// <summary>
    /// �ִϸ��̼� Ŭ���� Ư�� �����ӿ��� �߻��ϴ� �̺�Ʈ�� �����ϰ� ó���մϴ�.
    /// �� ��ũ��Ʈ�� public �޼������ �ִϸ��̼� �̺�Ʈ���� ���� ȣ��˴ϴ�.
    /// </summary>
    public class PlayerAnimationEvents : MonoBehaviour
    {
        // TODO: �߼Ҹ��� ����� ����� �ý����̳� ���� ������ ó���� ���� �ý����� ������ �ʿ��մϴ�.

        /// <summary>
        /// �Ȱų� �� �� ���� ���� ��� �����ӿ��� ȣ��� �Լ��Դϴ�.
        /// </summary>
        public void OnFootstep()
        {
            // Debug.Log("Footstep!");
            // TODO: �߼Ҹ� ���� ��� ������ ���⿡ �߰��մϴ�.
        }

        /// <summary>
        /// Ư�� �ִϸ��̼��� �������� FSM�� �˷��� �ʿ䰡 ���� �� ���� �� �ֽ��ϴ�.
        /// </summary>
        public void OnAnimationEnd()
        {
            // TODO: ���� ����(State)�� �ִϸ��̼��� �����ٰ� �˷��ִ� ������ �߰��մϴ�.
            //       (��: ���� ���¿��� Idle ���·� �ڵ� ��ȯ)
        }
    }
}