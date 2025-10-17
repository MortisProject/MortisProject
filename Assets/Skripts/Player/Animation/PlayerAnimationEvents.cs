// Assets/Scripts/Player/Animation/PlayerAnimationEvents.cs
using Player.Combat;
using Player.Data;
using Player.States;
using System;
using UnityEngine;
using World;

namespace Player.Animation
{
    /// <summary>
    /// 애니메이션 클립의 특정 프레임에서 발생하는 이벤트를 수신하고 처리합니다.
    /// 이 스크립트의 public 메서드들은 애니메이션 이벤트에서 직접 호출됩니다.
    /// </summary>
    public class PlayerAnimationEvents : MonoBehaviour
    {
        private Player _player;

        [Serializable]
        public class HitboxGroup
        {
            [Tooltip("히트박스 그룹을 식별하기 위한 태그입니다. (예: Weak, Strong)")]
            public string tag;
            [Tooltip("이 그룹에 속한 히트박스들입니다.")]
            public Hitbox[] hitboxes;
        }

        [Header("Combat References")]
        [Tooltip("무기 타입별로 히트박스 그룹을 관리합니다.")]
        public HitboxGroup[] whipHitboxGroups;

        [Header("Muzzle References")]
        [Tooltip("투사체가 발사될 위치(들)입니다. 오른손, 왼손 등 필요한 만큼 설정합니다.")]
        public Transform[] muzzles;

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
            return _player.StateMachine.CurrentState as PlayerAttackState;
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

        }

        /// <summary>
        /// (애니메이션 이벤트) 현재 콤보에 해당하는 스킬 효과(들)를 실행하도록 요청합니다.
        /// </summary>
        public void OnExecuteAttackEffect()
        {
            GetCurrentAttackState()?.ExecuteAttackEffects();
        }

        /// <summary>
        /// (애니메이션 이벤트) 변환 공격의 'Pre-Swap' 효과 실행을 요청합니다.
        /// </summary>
        public void OnExecutePreSwapEffect()
        {
            GetCurrentAttackState()?.ExecutePreSwapEffects();
        }

        /// <summary>
        /// (애니메이션 이벤트) 실제 무기 교체 실행을 요청합니다.
        /// </summary>
        public void OnPerformSwap()
        {
            GetCurrentAttackState()?.PerformSwap();
        }

        /// <summary>
        /// (애니메이션 이벤트) 변환 공격의 'Post-Swap' 효과 실행을 요청합니다.
        /// </summary>
        public void OnExecutePostSwapEffect()
        {
            GetCurrentAttackState()?.ExecutePostSwapEffects();
        }


        /// <summary>
        /// (애니메이션 이벤트) PlayerDodgeState를 가져오는 도우미 메서드
        /// </summary>
        private PlayerDodgeState GetCurrentDodgeState()
        {
            return _player.StateMachine.CurrentState as PlayerDodgeState;
        }

        /// <summary>
        /// (AnimEvent) 회피 이동 시작을 모터에 알립니다.
        /// </summary>
        public void OnDodgeMoveStart()
        {
            _player.Motor.StartDodgeMovement();
        }

        /// <summary>
        /// (AnimEvent) 회피 최대 속도 도달을 모터에 알립니다.
        /// </summary>
        public void OnDodgeReachMaxSpeed()
        {
            _player.Motor.SetDodgeMaxSpeed();
        }

        /// <summary>
        /// (AnimEvent) 회피 감속 시작을 모터에 알립니다.
        /// </summary>
        public void OnDodgeStartDeceleration()
        {
            _player.Motor.StartDodgeDeceleration();
        }

        /// <summary>
        /// (AnimEvent) 회피 이동 종료를 모터에 알리고, 상태 전환을 요청합니다.
        /// </summary>
        public void OnDodgeMoveEnd()
        {
            _player.Motor.EndDodgeMovement();
            // 현재 상태가 DodgeState일 때만 상태 종료를 요청합니다.
            GetCurrentDodgeState()?.FinishDodge();
        }

        /// <summary>
        /// (AnimEvent) 가드 브레이크 애니메이션 종료 시 호출됩니다.
        /// </summary>
        public void OnGuardBreakAnimationEnd()
        {
            // 현재 상태가 GuardBreakState일 때만 상태 종료를 요청합니다.
            (_player.StateMachine.CurrentState as PlayerGuardBreakState)?.OnAnimationFinished();
        }
    }
}