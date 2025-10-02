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
        // TODO: 발소리를 재생할 오디오 시스템이나 공격 판정을 처리할 전투 시스템의 참조가 필요합니다.

        /// <summary>
        /// 걷거나 뛸 때 발이 땅에 닿는 프레임에서 호출될 함수입니다.
        /// </summary>
        public void OnFootstep()
        {
            // Debug.Log("Footstep!");
            // TODO: 발소리 사운드 재생 로직을 여기에 추가합니다.
        }

        /// <summary>
        /// 특정 애니메이션이 끝났음을 FSM에 알려줄 필요가 있을 때 사용될 수 있습니다.
        /// </summary>
        public void OnAnimationEnd()
        {
            // TODO: 현재 상태(State)에 애니메이션이 끝났다고 알려주는 로직을 추가합니다.
            //       (예: 공격 상태에서 Idle 상태로 자동 전환)
        }
    }
}