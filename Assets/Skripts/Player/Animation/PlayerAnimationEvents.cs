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
    /// 애니메이션 클립의 특정 프레임에서 발생하는 이벤트를 수신하고 처리
    /// 이 스크립트의 public 메서드들은 애니메이션 이벤트에서 직접 호출
    /// </summary>
    public class PlayerAnimationEvents : MonoBehaviour
    {
        private Player _player;

        [Serializable]
        public class HitboxGroup
        {
            [Tooltip("히트박스 그룹을 식별하기 위한 태그 (예: Weak, Strong)")]
            public string tag;
            [Tooltip("이 그룹에 속한 히트박스")]
            public Hitbox[] hitboxes;
        }

        [Header("Combat References")]
        [Tooltip("무기 타입별로 히트박스 그룹을 관리")]
        public HitboxGroup[] whipHitboxGroups;

        [Header("Muzzle References")]
        [Tooltip("투사체가 발사될 위치(들)입니다. 오른손, 왼손 등 필요한 만큼 설정")]
        public Transform[] muzzles;

        [Header("Weapon Trail References")]
        [Tooltip("무기 궤적 효과를 위한 Trail Renderer")]
        public TrailRenderer[] weaponTrail; // 인스펙터에서 Whip_Trail_FX를 연결

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
        /// (애니메이션 이벤트) 현재 버스트 스킬 상태를 가져오는 도우미 메서드
        /// </summary>
        private PlayerBurstSkillState GetCurrentBurstSkillState()
        {
            return _player.StateMachine.CurrentState as PlayerBurstSkillState;
        }
        /// <summary>
        /// (애니메이션 이벤트) 다음 콤보 입력을 저장하기 시작하는 시점을 알림
        /// </summary>
        public void OnStartInputSave()
        {
            GetCurrentAttackState()?.OpenInputWindow();
        }

        /// <summary>
        /// (애니메이션 이벤트) 후딜레이 시작을 알림, 예약된 다음 공격이 있다면 즉시 전환
        /// </summary>
        public void OnStartAttackDelay()
        {
            GetCurrentAttackState()?.StartAttackDelay();
        }

        /// <summary>
        /// (애니메이션 이벤트) 후딜레이 모션이 완전히 끝났음을 알림 (콤보 종료)
        /// </summary>
        public void OnEndAttackDelay()
        {
            GetCurrentAttackState()?.EndAttackDelay();

        }

        /// <summary>
        /// (애니메이션 이벤트) 현재 콤보에 해당하는 스킬 효과를 실행하도록 요청
        /// </summary>
        public void OnExecuteAttackEffect()
        {
            var attackState = GetCurrentAttackState();
            if (attackState != null)
            {
                attackState.ExecuteAttackEffects();
            }
        }

        /// <summary>
        /// (애니메이션 이벤트) 버스트 스킬의 공격 효과를 실행
        /// </summary>
        public void OnExecuteBurstEffect()
        {
            var burstState = GetCurrentBurstSkillState();
            if (burstState == null) return;

            // 현재 무기의 버스트 스킬 데이터를 가져옴
            SkillData burstSkill = _player.Stats.CurrentWeaponData.burstSkill;
            if (burstSkill == null || burstSkill.effects == null)
            {
                Debug.LogWarning("현재 무기에 버스트 스킬 데이터가 없거나, 효과(effects)가 비어있습니다.");
                return;
            }

            // 스킬의 모든 효과를 실행
            foreach (var effect in burstSkill.effects)
            {
                if (effect != null)
                {
                    // 5-3에서 수정할 Execute 메서드를 호출
                    // burstState는 IState로 취급되어 전달
                    effect.Execute(_player, this, burstState);
                }
            }
        }

        /// <summary>
        /// (애니메이션 이벤트) 버스트 스킬 애니메이션이 끝났음을 알림
        /// </summary>
        public void OnBurstSkillEnd()
        {
            // 현재 상태가 BurstSkillState일 때만 OnAnimationFinished()를 호출
            GetCurrentBurstSkillState()?.OnAnimationFinished();
        }

        /// <summary>
        /// (애니메이션 이벤트) 변환 공격의 'Pre-Swap' 효과 실행을 요청
        /// </summary>
        public void OnExecutePreSwapEffect()
        {
            GetCurrentAttackState()?.ExecutePreSwapEffects();
        }

        /// <summary>
        /// (애니메이션 이벤트) 실제 무기 교체 실행을 요청
        /// </summary>
        public void OnPerformSwap()
        {
            GetCurrentAttackState()?.PerformSwap();
        }

        /// <summary>
        /// (애니메이션 이벤트) 변환 공격의 'Post-Swap' 효과 실행을 요청
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
        /// (애니메이션 이벤트) 회피 이동 시작을 모터에 알림
        /// </summary>
        public void OnDodgeMoveStart()
        {
            _player.Motor.StartDodgeMovement();
        }

        /// <summary>
        /// (애니메이션 이벤트) 회피 최대 속도 도달을 모터에 알림
        /// </summary>
        public void OnDodgeReachMaxSpeed()
        {
            _player.Motor.SetDodgeMaxSpeed();
        }

        /// <summary>
        /// (애니메이션 이벤트) 회피 감속 시작을 모터에 알림
        /// </summary>
        public void OnDodgeStartDeceleration()
        {
            _player.Motor.StartDodgeDeceleration();
        }

        /// <summary>
        /// (애니메이션 이벤트) 회피 이동 종료를 모터에 알리고 상태 전환을 요청
        /// </summary>
        public void OnDodgeMoveEnd()
        {
            _player.Motor.EndDodgeMovement();
            // 현재 상태가 DodgeState일 때만 상태 종료를 요청
            GetCurrentDodgeState()?.FinishDodge();
        }

        /// <summary>
        /// (애니메이션 이벤트) 가드 브레이크 애니메이션 종료 시 호출
        /// </summary>
        public void OnGuardBreakAnimationEnd()
        {
            // 현재 상태가 GuardBreakState일 때만 상태 종료를 요청
            (_player.StateMachine.CurrentState as PlayerGuardBreakState)?.OnAnimationFinished();
        }

        /// <summary>
        /// (애니메이션 이벤트) 공격 시작 시 무기 궤적을 활성화
        /// </summary>
        // ------------------ TODO: 하드코딩 수정 필요 ------------------
        public void StartWeaponTrail() 
        {
            weaponTrail[0].Clear(); // 이전 궤적을 지웁니다.
            weaponTrail[1].Clear(); // 이전 궤적을 지웁니다.
            weaponTrail[0].emitting = true; // 궤적 그리기를 시작합니다.
            weaponTrail[1].emitting = true; // 궤적 그리기를 시작합니다.
        }

        /// <summary>
        /// (애니메이션 이벤트) 공격 종료 시 무기 궤적을 비활성화합니다.
        /// </summary>
        public void StopWeaponTrail()
        {
            weaponTrail[0].emitting = false; // 궤적 그리기를 중지합니다.
            weaponTrail[1].emitting = false; // 궤적 그리기를 중지합니다.
        }
    }
}