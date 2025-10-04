// Assets/Scripts/Player/Core/CharacterStats.cs
using UnityEngine;

namespace Player
{
    /// <summary>
    /// ĳ����(�÷��̾�, ���� ��)�� ���������� ������ �⺻ ������ �����մϴ�.
    /// </summary>
    public class CharacterStats : MonoBehaviour
    {
        [Header("Core Stats")]
        [Tooltip("�ִ� ü���Դϴ�.")]
        public float maxHp = 120f;

        [Tooltip("���� ü���� ��Ÿ���ϴ�.")]
        public float currentHp;

        // TODO: ���� ���ݷµ� �÷��̾�� ���Ͱ� �����ϴ� ���� �߰�

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