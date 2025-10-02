// Assets/Scripts/Player/Core/CharacterStats.cs
using UnityEngine;

namespace Player
{
    /// <summary>
    /// �÷��̾� ĳ������ ��� ��ġ �����͸� �����ϰ� �����մϴ�.
    /// �ٸ� ��� ������Ʈ���� �� ��ũ��Ʈ�� �����Ͽ� �ʿ��� �ɷ�ġ ���� �������ϴ�.
    /// </summary>
    public class CharacterStats : MonoBehaviour
    {
        [Header("Core Stats")]
        [Tooltip("�ִ� ü���Դϴ�.")]
        public float maxHp = 120f;

        [Tooltip("���� ü���� ��Ÿ���ϴ�.")]
        public float currentHp;

        [Header("Movement Stats")]
        [Tooltip("�ȱ� �ӵ��Դϴ�.")]
        public float walkSpeed = 5f;
        [Tooltip("�޸��� �ӵ��Դϴ�.")]
        public float runSpeed = 9f;

        [Header("Acrobatic Stats")]
        [Tooltip("���� �� ������ ��ǥ �����Դϴ�.")]
        public float jumpHeight = 2.0f;

        // TODO: ����, ���� �� ���ο� ���°� �߰��� �� �ʿ��� ���ȵ��� ���⿡ �߰��մϴ�.
        // ��: public float dodgeDuration = 0.5f;
        // ��: public float attackDamage = 20f;

        /// <summary>
        /// ��ũ��Ʈ �ν��Ͻ��� �ε�� �� ȣ��˴ϴ�.
        /// </summary>
        private void Awake()
        {
            // ���� ���� �� ���� ü���� �ִ� ü������ �ʱ�ȭ�մϴ�.
            currentHp = maxHp;
        }
    }
}