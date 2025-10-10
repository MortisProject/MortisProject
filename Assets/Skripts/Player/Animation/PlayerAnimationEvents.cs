// Assets/Scripts/Player/Animation/PlayerAnimationEvents.cs
using Player.Combat;
using Player.Data;
using Player.States;
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

        [Header("Combat References")]
        [Header("Whip Hitboxes")]
        public Hitbox[] whipWeakAttackHitboxes;
        public Hitbox[] whipStrongAttackHitboxes;

        [Header("Whip Skill Data")]
        public SkillData[] whipWeakAttackSkills;
        public SkillData[] whipStrongAttackSkills;

        private void Awake()
        {
            // 부모 오브젝트에서 Player 컴포넌트를 찾아 할당
            _player = GetComponentInParent<Player>();
        }

        /// <summary>
        /// (애니메이션 이벤트) 현재 공격 상태를 가져오는 도우미 메서드
        /// </summary>
        private PlayerAttackState GetCurrentAttackState()
        {
            if (_player.StateMachine.CurrentState is PlayerAttackState attackState)
            {
                return attackState;
            }
            return null;
        }

        /// <summary>
        /// (애니메이션 이벤트) 다음 콤보 입력을 저장하기 시작하는 시점을 알립니다.
        /// </summary>
        public void OnStartInputSave()
        {
            GetCurrentAttackState()?.OpenInputWindow();
        }

        /// <summary>
        /// (애니메이션 이벤트) 후딜레이 시작을 알립니다. 예약된 다음 공격이 있다면 즉시 전환됩니다.
        /// </summary>
        public void OnStartAttackDelay()
        {
            GetCurrentAttackState()?.StartAttackDelay();
        }

        /// <summary>
        /// (애니메이션 이벤트) 후딜레이 모션이 완전히 끝났음을 알립니다. 콤보가 종료됩니다.
        /// </summary>
        public void OnEndAttackDelay()
        {
            GetCurrentAttackState()?.EndAttackDelay();
            Debug.Log("EndAttackDelay");

        }

        /// <summary>
        /// (애니메이션 이벤트에서 호출) 지정된 인덱스의 채찍 약공격 히트박스를 활성화합니다.
        /// </summary>
        /// <param name="index">활성화할 히트박스의 번호 (0부터 시작)</param>
        public void ActivateWhipWeakHitbox(int index)
        {
            // 현재 상태가 공격 상태가 아니면 아무것도 하지 않음 (오류 방지)
            if (!(_player.StateMachine.CurrentState is PlayerAttackState attackState)) return;

            // 유효한 인덱스인지 확인
            if (index < 0 || index >= whipWeakAttackHitboxes.Length || index >= whipWeakAttackSkills.Length) return;

            // 데미지 계산 및 히트박스 활성화
            float baseDamage = _player.Stats.attackValue;
            float damageMultiplier = whipWeakAttackSkills[index].damageMultiplier;
            float finalDamage = baseDamage * damageMultiplier;
            float duration = 0.2f; // 히트박스 지속 시간 (임시)

            whipWeakAttackHitboxes[index].Activate(finalDamage, duration);
        }
    }
}