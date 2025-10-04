// Assets/Scripts/Player/Data/PlayerSO.cs
using UnityEngine;

namespace Player.Data
{
    // CreateAssetMenu�� ����ϸ� ����Ƽ �������� Create �޴����� �� ������ ���� ������ �� �ֽ��ϴ�.
    [CreateAssetMenu(fileName = "NewPlayerData", menuName = "Data/Player Data")]
    public class PlayerSO : ScriptableObject
    {
        [Header("Movement Stats")]
        [Tooltip("�ȱ� �ӵ��Դϴ�.")]
        public float walkSpeed = 5f;
        [Tooltip("�޸��� �ӵ��Դϴ�.")]
        public float runSpeed = 9f;

        [Header("Acrobatic Stats")]
        [Tooltip("���� �� ������ ��ǥ �����Դϴ�.")]
        public float jumpHeight = 2.0f;

        // TODO: ���� ���ӽð�, ���̾� �ӵ� �� �÷��̾� ���� �����͸� ���⿡ ��� �߰��մϴ�.
    }
}