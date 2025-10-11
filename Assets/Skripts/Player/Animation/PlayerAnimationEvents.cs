// Assets/Scripts/Player/Animation/PlayerAnimationEvents.cs
using Player.Combat;
using Player.Data;
using Player.States;
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
        //public void ActivateWhipWeakHitbox(int index)
        //{
        //    // 현재 상태가 공격 상태가 아니면 아무것도 하지 않음 (오류 방지)
        //    if (!(_player.StateMachine.CurrentState is PlayerAttackState attackState)) return;

        //    // 유효한 인덱스인지 확인
        //    if (index < 0 || index >= whipWeakAttackHitboxes.Length || index >= whipWeakAttackSkills.Length) return;

        //    // 데미지 계산 및 히트박스 활성화
        //    float baseDamage = _player.Stats.attackValue;
        //    float damageMultiplier = whipWeakAttackSkills[index].damageMultiplier;
        //    float finalDamage = baseDamage * damageMultiplier;
        //    float duration = 0.2f; // 히트박스 지속 시간 (임시)

        //    whipWeakAttackHitboxes[index].Activate(finalDamage, duration);
        //}

        /// <summary>
        /// (애니메이션 이벤트) 현재 무기의 약공격 히트박스를 활성화합니다.
        /// </summary>
        /// <param name="index">활성화할 히트박스의 번호 (콤보 순서, 0부터 시작)</param>
        public void ActivateWeakHitbox(int index)
        {
            WeaponData currentWeapon = _player.Stats.CurrentWeaponData;
            if (currentWeapon == null) return;

            // 현재 무기 데이터에서 스킬 정보를 가져옵니다.
            if (index < 0 || index >= currentWeapon.weakAttackSkills.Length)
            {
                Debug.LogWarning($"Current Weapon ({currentWeapon.weaponType}) does not have a weak attack skill definition for index {index}.");
                return;
            }
            SkillData skill = currentWeapon.weakAttackSkills[index];

            // 데미지 계산
            float baseDamage = _player.Stats.attackValue;
            float damageMultiplier = skill.damageMultiplier / 100f; // 120 -> 1.2로 변환
            float finalDamage = baseDamage * damageMultiplier;
            float duration = 0.2f; // TODO: 이 값도 SkillData에서 가져오도록 확장 가능

            // 활성화할 히트박스 배열을 선택합니다.
            Hitbox[] targetHitboxArray = null;
            switch (currentWeapon.weaponType)
            {
                case WeaponType.Whip:
                    targetHitboxArray = whipWeakAttackHitboxes;
                    break;
                    // TODO: 다른 근접 무기 타입이 추가되면 여기에 case를 추가합니다.
                    // case WeaponType.Dagger:
                    //     targetHitboxArray = daggerWeakAttackHitboxes;
                    //     break;
            }

            // 선택된 배열에서 올바른 히트박스를 찾아 활성화합니다.
            if (targetHitboxArray != null && index < targetHitboxArray.Length)
            {
                targetHitboxArray[index].Activate(finalDamage, duration);
                Debug.Log($"{currentWeapon.weaponType}의 {index + 1}번째 약공격 발동! 데미지: {finalDamage}");
            }
            else
            {
                Debug.LogWarning($"Hitbox for {currentWeapon.weaponType} weak attack index {index} is not assigned or out of range.");
            }
        }

        /// <summary>
        /// (애니메이션 이벤트) 현재 무기의 발사체를 발사합니다.
        /// </summary>
        /// <param name="muzzleIndex">발사될 총구의 인덱스 (여러개일 경우 대비)</param>
        public void FireProjectile(int muzzleIndex)
        {
            PlayerAttackState attackState = GetCurrentAttackState();
            if (attackState == null) return;

            // 현재 무기의 발사체 데이터를 가져옵니다.
            ProjectileData projectileData = _player.Stats.CurrentWeaponData?.projectileData;
            if (projectileData == null)
            {
                Debug.LogWarning($"현재 무기({_player.Stats.CurrentWeaponData.weaponType})에 ProjectileData가 설정되지 않았습니다.");
                return;
            }

            // 1. 발사체 풀에서 발사체를 가져옵니다.
            string poolTag = "RaygunProjectile"; // TODO: 이 태그도 WeaponData에서 가져오도록 확장 가능
            GameObject projectileObject = ProjectilePoolManager.Instance.GetFromPool(poolTag);
            if (projectileObject == null) return;

            // 2. 발사 위치와 방향 설정
            Transform muzzle = _player.WireOrigin; // TODO: 실제 총구 Transform을 참조하도록 변경 필요
            Vector3 fireDirection = attackState.AimDirection; // 공격 상태에 저장된 조준 방향 사용

            projectileObject.transform.position = muzzle.position;
            projectileObject.transform.rotation = Quaternion.LookRotation(fireDirection);

            // 3. 발사체 초기화 및 발사
            if (projectileObject.TryGetComponent<Projectile>(out Projectile projectile))
            {
                projectile.Initialize(poolTag, fireDirection, _player.Stats.attackValue, projectileData);
                projectileObject.SetActive(true);
            }
        }
    }
}