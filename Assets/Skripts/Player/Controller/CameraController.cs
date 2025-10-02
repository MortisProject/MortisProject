// Assets/Scripts/Player/Controller/CameraController.cs
using UnityEngine;

namespace Player
{
    /// <summary>
    /// PlayerInput���κ��� Look �Է��� �޾� �÷��̾��� �¿� ȸ����
    /// ī�޶� Ÿ���� ���� ȸ���� ����մϴ�.
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("�Է� ���� �޾ƿ� PlayerInput ��ũ��Ʈ�Դϴ�.")]
        [SerializeField] private PlayerInput _input;

        [Tooltip("�¿� ȸ���� ������ �÷��̾��� Transform �Դϴ�.")]
        [SerializeField] private Transform _playerBody;

        [Header("Camera Control Settings")]
        [Tooltip("���콺 �����Դϴ�.")]
        [SerializeField] private float _mouseSensitivity = 1.5f;

        [Tooltip("ī�޶��� ���� ȸ�� ������ �����մϴ�. X = �ּ�, Y = �ִ�")]
        [SerializeField] private Vector2 _pitchMinMax = new Vector2(-40, 85);

        // TODO: �����е� ��� �� ������ ������ ������ �� �ֵ��� ������ �߰��ϸ� �����ϴ�.

        private float _yaw;   // �¿� ȸ�� ���� ��
        private float _pitch; // ���� ȸ�� ���� ��

        /// <summary>
        /// Update ���Ŀ� ȣ��Ǿ� ī�޶� �������� ĳ���� �������� ���󰡵��� �մϴ�.
        /// </summary>
        private void LateUpdate()
        {
            // 1. PlayerInput ��ũ��Ʈ���� ���콺 ������(LookInput) ���� �����ɴϴ�.
            Vector2 lookInput = _input.LookInput;

            // 2. ���콺 �Է°� ������ ���� ȸ�� ���� �����մϴ�. Time.deltaTime�� ���� �����ӿ� ���������� ����ϴ�.
            _yaw += lookInput.x * _mouseSensitivity * Time.deltaTime;
            _pitch -= lookInput.y * _mouseSensitivity * Time.deltaTime;

            // 3. ���� ȸ��(Pitch) ������ _pitchMinMax �� ���̷� �����մϴ�.
            _pitch = Mathf.Clamp(_pitch, _pitchMinMax.x, _pitchMinMax.y);

            // 4. �¿� ȸ��(Yaw)�� �÷��̾� ��ü ��ü�� �����մϴ�.
            _playerBody.eulerAngles = new Vector3(0, _yaw, 0);

            // 5. ���� ȸ��(Pitch)�� �� ��ũ��Ʈ�� �پ��ִ� CameraTarget���� �����մϴ�.
            transform.localEulerAngles = new Vector3(_pitch, 0, 0);
        }
    }
}