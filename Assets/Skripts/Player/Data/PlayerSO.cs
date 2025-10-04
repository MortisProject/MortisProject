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

        [Header("Acrobatic Stats")]
        [Tooltip("점프 시 도달할 목표 높이입니다.")]
        public float jumpHeight = 2.0f;

        // TODO: 닷지 지속시간, 와이어 속도 등 플레이어 전용 데이터를 여기에 계속 추가합니다.
    }
}