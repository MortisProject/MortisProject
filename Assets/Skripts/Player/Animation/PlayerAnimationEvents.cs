// Assets/Scripts/Player/Animation/PlayerAnimationEvents.cs
using UnityEngine;

namespace Player.Animation
{
    /// <summary>
    /// 애니메이션 클립의 특정 프레임에서 발생하는 이벤트를 수신하고 처리합니다.
    /// 이 스크립트의 public 메서드들은 애니메이션 이벤트에서 직접 호출됩니다.
    /// </summary>
    public class PlayerAnimationEvents : MonoBehaviour
    {
        private Player _player;

        private void Awake()
        {
            // 부모 오브젝트에서 Player 컴포넌트를 찾아 할당
            _player = GetComponentInParent<Player>();
        }

        /// <summary>
        /// (애니메이션 이벤트) 공격 판정이 시작되는 프레임에서 호출되어 히트박스를 활성화합니다.
        /// </summary>
        public void OnActivateHitbox()
        {
            // 현재 플레이어의 상태가 '지상 공격 상태'인지 확인
            if (_player.StateMachine.CurrentState is States.PlayerGroundedAttackState attackState)
            {
                // 맞다면, 공격 상태의 히트박스 활성화 메서드를 호출
                attackState.ActivateHitbox();
            }
        }
    }
}