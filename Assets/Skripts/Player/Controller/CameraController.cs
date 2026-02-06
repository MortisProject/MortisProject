// Assets/Scripts/Player/Controller/CameraController.cs
using Player.Data;
using TMPro;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// PlayerInput으로부터 Look 입력을 받아 플레이어의 좌우 회전과
    /// 카메라 타겟의 상하 회전을 담당
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerInput _input;
        [SerializeField] private Transform _playerBody;
        [SerializeField] private PlayerSO _data;

        float _mouseSensitivity;
        Vector2 _pitchMinMax;
        private float _yaw;   // 좌우 회전 누적 값
        private float _pitch; // 상하 회전 누적 값

        private void Awake()
        {
            _mouseSensitivity = _data.mouseSensitivity;
            _pitchMinMax = _data.pitchMinMax;
        }

        /// <summary>
        /// Update 이후에 호출되어 카메라 움직임이 캐릭터 움직임을 따라가도록함
        /// </summary>
        private void LateUpdate()
        {
            // 1. PlayerInput 스크립트에서 마우스 움직임(LookInput) 값을 가져옴
            Vector2 lookInput = _input.LookInput;

            // 2. 마우스 입력과 감도를 곱해 회전 값을 누적, Time.deltaTime을 곱해 프레임에 독립적으로 만듦
            _yaw += lookInput.x * _mouseSensitivity * Time.deltaTime;
            _pitch -= lookInput.y * _mouseSensitivity * Time.deltaTime;

            // 3. 상하 회전(Pitch) 각도를 _pitchMinMax 값 사이로 제한
            _pitch = Mathf.Clamp(_pitch, _pitchMinMax.x, _pitchMinMax.y);

            // 4. 좌우 회전(Yaw)은 플레이어 몸체 전체에 적용
            _playerBody.eulerAngles = new Vector3(0, _yaw, 0);

            // 5. 상하 회전(Pitch)은 이 스크립트가 붙어있는 CameraTarget에만 적용
            transform.localEulerAngles = new Vector3(_pitch, 0, 0);
        }
    }
}