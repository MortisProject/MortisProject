// Assets/Scripts/Player/Data/PlayerSO.cs
using UnityEngine;

namespace Player.Data
{
    // CreateAssetMenu를 사용하면 유니티 에디터의 Create 메뉴에서 이 에셋을 쉽게 생성할 수 있습니다.
    [CreateAssetMenu(fileName = "NewPlayerData", menuName = "Data/Player Data")]
    public class PlayerSO : ScriptableObject
    {
        [Header("Movement Stats")]
        [Tooltip("걷기 속도입니다.")]
        public float walkSpeed = 5f;

        [Tooltip("달리기 속도입니다.")]
        public float runSpeed = 9f;



        [Header("Grounded Settings")]
        [Tooltip("걸을 수 있는 최대 경사각입니다. (단위: 도)")]
        [Range(0f, 90f)]
        public float maxSlopeAngle = 45f;



        [Header("Acrobatic Stats")]
        [Tooltip("Rigidbody.AddForce로 가해지는 점프의 힘입니다.")]
        public float jumpForce = 15f;

        [Tooltip("더블 점프시 위로 점프할 때의 힘입니다. 관성을 끊고 제어된 점프를 할 때 사용됩니다.")]
        public float doubleJumpForce = 12f;

        [Tooltip("더블 점프시 앞으로 튕겨나갈 힘입니다. ")]
        public float doubleJumpDashForce = 5f;

        [Tooltip("공중에서 좌우로 움직일 때 가해지는 힘입니다.")]
        public float airControlForce = 1f;

        [Tooltip("와이어, 점프 등 모든 움직임에서 도달할 수 있는 최대 '수평' 속도입니다.")]
        public float maxHorizontalSpeed = 35f;



        [Header("Camera Control")]
        [Tooltip("카메라 감도 조절입니다.")]
        public float mouseSensitivity = 1.5f;

        [Tooltip("카메라의 상하 회전 각도를 제한합니다. X = 최소, Y = 최대")]
        public Vector2 pitchMinMax = new Vector2(-40, 85);



        [Header("Wire Action Stats")]
        [Tooltip("발사될 와이어 훅의 프리팹입니다.")]
        public GameObject wireHookPrefab;

        [Tooltip("와이어를 발사하여 타겟에 '걸 수 있는' 최대 사거리입니다.")]
        public float wireGrappleRange = 50f;

        [Tooltip("와이어에 매달려 '스윙할 때'의 실제 와이어 길이입니다. GrappleRange보다 짧게 설정하면 와이어가 감기는 효과를 냅니다.")]
        public float wireSwingLength = 15f;

        [Tooltip("화면 중앙에서 와이어 포인트를 탐색할 반경입니다. (0~1 사이 값)")]
        [Range(0f, 1f)]
        public float wireAimSearchRadius = 0.5f;

        [Tooltip("와이어 이동 시작 시의 초기 대쉬 속도입니다.")]
        public float wireLaunchSpeed = 25f;

        [Tooltip("공중에서 사용하는 추가 대쉬의 속도입니다.")]
        public float wireAirDashSpeed = 30f;

        [Tooltip("와이어 탈출 시 추가되는 수직 점프 높이입니다.")]
        public float wireDetachVerticalBonus = 2f;

        [Tooltip("와이어 발사 시 뒤로 살짝 점프하는 힘의 크기입니다.")]
        public float wireLaunchBackwardImpulse = 3f;


        [Header("Spring Joint Settings")]
        [Tooltip("와이어의 탄성 계수입니다. 높을수록 팽팽해집니다.")]
        public float wireSpringForce = 1000f;

        [Tooltip("와이어의 출렁임을 흡수하는 정도입니다. 높을수록 빨리 안정됩니다.")]
        public float wireDamper = 100f;

        // TODO: 닷지 지속시간, 와이어 속도 등 플레이어 전용 데이터를 여기에 계속 추가합니다.
    }
}