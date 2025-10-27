// Assets/Scripts/Monster/Monster.cs
using Monster.Animation;
using Monster.States;
using Monster.Data;
using World.Manager;
using System.Collections;
using UnityEngine;
using UnityEngine.AI; 

#if UNITY_EDITOR
using UnityEditor; // Handles 클래스를 사용하기 위해 에디터 네임스페이스를 추가합니다.
#endif

namespace Monster
{
    /// <summary>
    /// 몬스터의 모든 컴포넌트와 상태를 총괄하는 메인 컨트롤러입니다.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))] // 몬스터는 반드시 NavMeshAgent를 가지도록 강제
    public class Monster : MonoBehaviour
    {
        [Header("Data")]
        [Tooltip("몬스터의 모든 데이터를 담고 있는 ScriptableObject 입니다.")]
        public MonsterSO Data;

        [Header("Component References")]
        public MonsterStateMachine StateMachine { get; private set; }
        public NavMeshAgent Agent { get; private set; }
        public MonsterAnimatorController AnimController { get; private set; }
        public MonsterSpawner Spawner { get; private set; }

        [Header("Runtime Variables")]
        [Tooltip("몬스터의 현재 체력입니다. (런타임에 자동 초기화)")]
        public float currentHp;

        [Tooltip("몬스터가 추적하거나 공격할 대상입니다.")]
        public Transform target;

        [Tooltip("몬스터의 피격 VFX가 활성화될 위치.")]
        public Transform HitEffectTarget;        
        
        [Tooltip("몬스터의 특수공격(Blue, Yellow) VFX가 활성화될 위치.")]
        public Transform SpecialAttackEffectTarget;

        [Header("특수공격 콤보 (런타임)")]
        [Tooltip("현재 누적된 일반 공격 횟수입니다.")]
        public int CurrentSkillCount { get; private set; } = 0;

        [Tooltip("현재 특수 공격(Ready, Attack)을 시전 중인지 여부입니다. (경직 면역)")]
        public bool IsSpecialAttacking { get; private set; } = false;

        [Tooltip("이번에 실행할 특수 공격의 타입입니다. (Ready 상태가 설정)")]
        public MonsterSkillData.AttackType NextSpecialAttackType { get; set; }

        private Coroutine _knockbackCoroutine;
        // --- 상태 클래스 인스턴스 ---
        public MonsterSpawnState SpawnState { get; private set; }
        public MonsterIdleState IdleState { get; private set; }
        public MonsterPatrolState PatrolState { get; private set; }
        public MonsterChaseState ChaseState { get; private set; }
        public MonsterBattleState BattleState { get; private set; }
        public MonsterHitState HitState { get; private set; }
        public MonsterDieState DieState { get; private set; }
        public MonsterSpecialAttackReadyState SpecialAttackReadyState { get; private set; }
        public MonsterYellowAttackState YellowAttackState { get; private set; }
        public MonsterBlueAttackState BlueAttackState { get; private set; }

        public string PoolTag { get; private set; }

        /// <summary>
        /// 게임이 시작되기 전, 모든 컴포넌트와 상태를 초기화합니다.
        /// </summary>
        private void Awake()
        {
            StateMachine = GetComponent<MonsterStateMachine>();
            Agent = GetComponentInChildren<NavMeshAgent>();
            AnimController = GetComponentInChildren<MonsterAnimatorController>();

            // 모든 상태 클래스의 인스턴스를 생성하고, Monster 참조를 넘겨줍니다.
            SpawnState = new MonsterSpawnState(this);
            IdleState = new MonsterIdleState(this);
            PatrolState = new MonsterPatrolState(this);
            ChaseState = new MonsterChaseState(this);
            BattleState = new MonsterBattleState(this);
            HitState = new MonsterHitState(this);
            DieState = new MonsterDieState(this);
            SpecialAttackReadyState = new MonsterSpecialAttackReadyState(this);
            YellowAttackState = new MonsterYellowAttackState(this);
            BlueAttackState = new MonsterBlueAttackState(this);
        }

        /// <summary>
        /// 첫 프레임이 업데이트되기 전, 상태 머신을 시작 상태로 초기화합니다.
        /// </summary>
        private void Start()
        {
            currentHp = Data.maxHp;
            StateMachine.Initialize(SpawnState);
        }

        /// <summary>
        /// 매 프레임 현재 상태의 Update를 호출합니다.
        /// </summary>
        private void Update()
        {
            StateMachine.CurrentState?.Update();
        }

        /// <summary>
        /// 몬스터가 '노란 공격'을 할 준비가 되었는지 확인합니다.
        /// </summary>
        public bool IsYellowAttackReady
        {
            get
            {
                // 정예 몬스터(Elite)이고, 노란 공격 횟수(Threshold)가 설정되어 있으며, 현재 콤보가 기준치를 넘었는지 확인
                return Data.grade == MonsterGrade.Elite &&
                       Data.yellowAttackThreshold > 0 &&
                       CurrentSkillCount > 0 &&
                       (CurrentSkillCount % Data.yellowAttackThreshold == 0);
            }
        }

        /// <summary>
        /// 몬스터가 '파란 공격'을 할 준비가 되었는지 확인합니다.
        /// </summary>
        public bool IsBlueAttackReady
        {
            get
            {
                // 파란 공격 횟수(Threshold)가 설정되어 있으며, 현재 콤보가 기준치를 넘었는지 확인
                return Data.blueAttackThreshold > 0 &&
                       CurrentSkillCount > 0 &&
                       (CurrentSkillCount % Data.blueAttackThreshold == 0);
            }
        }
        /// <summary>
        /// 지정된 양의 데미지를 받아 체력을 감소시킵니다.
        /// </summary>
        public void TakeDamage(float damage, Vector3 knockbackDirection, float knockbackForce, bool isKnockback)
        {
            // 특수 공격 중에는 경직 면역
            if (IsSpecialAttacking)
            {
                isKnockback = false;
            }

            if (StateMachine.CurrentState is MonsterDieState || StateMachine.CurrentState is MonsterSpawnState)
            {
                return;
            }

            currentHp -= damage;
            VFXManager.Instance.PlayVFX("MonsterHit", HitEffectTarget.position);
            Debug.Log($"{gameObject.name}이(가) {damage}의 피해를 입었습니다! 현재 체력: {currentHp}");

            if (currentHp <= 0)
            {
                StateMachine.ChangeState(DieState);
            }
            else if (isKnockback)
            {
                ApplyKnockback(knockbackDirection, knockbackForce);
                StateMachine.ChangeState(HitState);
            }
        }

        /// <summary>
        /// (애니메이션 이벤트에서 호출됨) 공격 애니메이션이 종료되었을 때 호출됩니다.
        /// </summary>
        /// <param name="attackType">MonsterAnimationEvents가 전달한 공격의 타입 (Normal, Blue, Yellow)</param>
        public void OnAttackFinished(MonsterSkillData.AttackType attackType)
        {
            // 특수 공격 상태(경직 면역)를 해제합니다.
            SetSpecialAttacking(false);

            if (attackType == MonsterSkillData.AttackType.Normal)
            {
                // 일반 공격은 횟수를 1 누적합니다.
                IncrementSkillCount();
            }
            else
            {
                Debug.Log("이 몬스터가 사용한 특수 공격이 끝났으므로, 그룹 쿨다운을 해제합니다.");
                // 이 몬스터가 사용한 특수 공격이 끝났으므로, 그룹 쿨다운을 해제합니다.
                Spawner.ResetSpecialAttackCooldown();
                IncrementSkillCount();
            }

            // 현재 상태가 BattleState가 아닐 수도 있으므로 (e.g., YellowAttackState),
            // 현재 상태에게 공격이 끝났음을 알립니다.
            (StateMachine.CurrentState as IMonsterAttackState)?.OnAttackFinished();
        }

        /// <summary>
        /// 일반 공격 횟수를 1 증가시킵니다.
        /// </summary>
        public void IncrementSkillCount()
        {
            CurrentSkillCount++;
            Debug.Log($"[Skill Count] {name}: {CurrentSkillCount}");
        }

        /// <summary>
        /// 일반 공격 횟수를 0으로 초기화합니다.
        /// </summary>
        public void ResetSkillCount()
        {
            CurrentSkillCount = 0;
            Debug.Log($"[Skill Count] {name}: RESET (0)");
        }

        /// <summary>
        /// (기획서 우선순위 로직) 파란 공격 쿨다운 중 노란 공격이 준비되면,
        /// 노란 공격 횟수를 2 감소시킵니다.
        /// </summary>
        public void ApplyYellowAttackPenalty()
        {
            // 0 미만으로 내려가지 않도록 보정
            CurrentSkillCount = Mathf.Max(0, CurrentSkillCount - 1);
            Debug.Log($"[Skill Count] {name}: Yellow Attack Penalty (-2) -> {CurrentSkillCount}");
        }

        /// <summary>
        /// 특수 공격 상태(경직 면역)를 설정합니다.
        /// </summary>
        public void SetSpecialAttacking(bool status)
        {
            IsSpecialAttacking = status;
        }

        /// <summary>
        /// 스포너가 몬스터를 활성화할 때 호출하여 기본 정보를 설정합니다.
        /// </summary>
        public void Setup(string poolTag, MonsterSpawner spawner)
        {
            PoolTag = poolTag;
            Spawner = spawner; // 스포너 참조 저장
        }

        /// <summary>
        /// 몬스터를 풀에서 재사용하기 위해 모든 상태를 초기화합니다.
        /// </summary>
        public void ResetMonster()
        {
            currentHp = Data.maxHp;
            StateMachine.Initialize(SpawnState);
            GetComponent<Collider>().enabled = true;
            Agent.enabled = true;

            // 콤보 카운트 및 상태 초기화
            ResetSkillCount();
            SetSpecialAttacking(false);
            NextSpecialAttackType = MonsterSkillData.AttackType.Normal;
        }

        /// <summary>
        /// 몬스터에게 넉백을 적용합니다. NavMeshAgent와 충돌을 피하기 위해 코루틴을 사용합니다.
        /// </summary>
        private void ApplyKnockback(Vector3 direction, float force)
        {
            if (_knockbackCoroutine != null)
            {
                StopCoroutine(_knockbackCoroutine);
            }
            _knockbackCoroutine = StartCoroutine(KnockbackCoroutine(direction, force));
        }

        private IEnumerator KnockbackCoroutine(Vector3 direction, float force)
        {
            // 넉백 시작 시 NavMeshAgent의 제어를 잠시 비활성화합니다.
            Agent.enabled = false;

            float timer = 0f;
            float knockbackDuration = 0.2f; // 넉백이 지속될 시간
            Vector3 startPosition = transform.position;
            Vector3 targetPosition = startPosition + direction * (force * 0.1f); // 힘에 비례하여 거리 조절

            while (timer < knockbackDuration)
            {
                // Lerp를 사용하여 부드럽게 목표 위치로 이동합니다.
                transform.position = Vector3.Lerp(startPosition, targetPosition, timer / knockbackDuration);
                timer += Time.deltaTime;
                yield return null;
            }

            // 넉백이 끝나면 NavMeshAgent를 다시 활성화합니다.
            Agent.enabled = true;
            _knockbackCoroutine = null;
        }

#if UNITY_EDITOR
        /// <summary>
        /// 유니티 에디터의 Scene 뷰에서만 작동하며, 디버깅 목적으로 도형을 그려줍니다.
        /// 이 오브젝트가 선택되었을 때만 호출됩니다.
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            // MonsterSO 데이터가 할당되지 않았다면 오류 방지를 위해 실행하지 않습니다.
            if (Data == null) return;

            // 기즈모의 색상과 투명도를 설정합니다.
            Handles.color = new Color(1f, 1f, 0f, 0.2f); // 노란색 (감지 범위)
            // 몬스터의 위치를 중심으로 채워진 원반을 그립니다.
            Handles.DrawSolidDisc(transform.position, Vector3.up, Data.detectionRange);

            Handles.color = new Color(1f, 0f, 0f, 0.2f); // 빨간색 (공격 범위)
            Handles.DrawSolidDisc(transform.position, Vector3.up, Data.attackRange);
        }
#endif
    }
}