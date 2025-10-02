// Assets/Scripts/Player/Controller/CameraController.cs
using UnityEngine;

namespace Player
{
    /// <summary>
    /// PlayerInput으로부터 Look 입력을 받아 플레이어의 좌우 회전과
    /// 카메라 타겟의 상하 회전을 담당합니다.
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("입력 값을 받아올 PlayerInput 스크립트입니다.")]
        [SerializeField] private PlayerInput _input;

        [Tooltip("좌우 회전을 적용할 플레이어의 Transform 입니다.")]
        [SerializeField] private Transform _playerBody;

        [Header("Camera Control Settings")]
        [Tooltip("마우스 감도입니다.")]
        [SerializeField] private float _mouseSensitivity = 1.5f;

        [Tooltip("카메라의 상하 회전 각도를 제한합니다. X = 최소, Y = 최대")]
        [SerializeField] private Vector2 _pitchMinMax = new Vector2(-40, 85);

        // TODO: 게임패드 사용 시 감도를 별도로 설정할 수 있도록 변수를 추가하면 좋습니다.

        private float _yaw;   // 좌우 회전 누적 값
        private float _pitch; // 상하 회전 누적 값

        /// <summary>
        /// Update 이후에 호출되어 카메라 움직임이 캐릭터 움직임을 따라가도록 합니다.
        /// </summary>
        private void LateUpdate()
        {
            // 1. PlayerInput 스크립트에서 마우스 움직임(LookInput) 값을 가져옵니다.
            Vector2 lookInput = _input.LookInput;

            // 2. 마우스 입력과 감도를 곱해 회전 값을 누적합니다. Time.deltaTime을 곱해 프레임에 독립적으로 만듭니다.
            _yaw += lookInput.x * _mouseSensitivity * Time.deltaTime;
            _pitch -= lookInput.y * _mouseSensitivity * Time.deltaTime;

            // 3. 상하 회전(Pitch) 각도를 _pitchMinMax 값 사이로 제한합니다.
            _pitch = Mathf.Clamp(_pitch, _pitchMinMax.x, _pitchMinMax.y);

            // 4. 좌우 회전(Yaw)은 플레이어 몸체 전체에 적용합니다.
            _playerBody.eulerAngles = new Vector3(0, _yaw, 0);

            // 5. 상하 회전(Pitch)은 이 스크립트가 붙어있는 CameraTarget에만 적용합니다.
            transform.localEulerAngles = new Vector3(_pitch, 0, 0);
        }
    }
}