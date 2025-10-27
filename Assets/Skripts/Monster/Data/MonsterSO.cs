// Assets/Scripts/Monster/Data/MonsterSO.cs
using UnityEngine;

namespace Monster
{
    // 공격 방식을 구분하기 위한 열거형입니다.
    public enum AttackType
    {
        Melee,      // 근거리
        Ranged,     // 원거리
        Mixed       // 혼합형
    }

    // 몬스터 등급을 구분하기 위한 열거형입니다.
    public enum MonsterGrade
    {
        Normal, // 일반
        Elite   // 정예
    }

    [CreateAssetMenu(fileName = "NewMonsterData", menuName = "Data/Monster Data")]
    public class MonsterSO : ScriptableObject
    {
        [Header("몬스터 기본 정보")]
        [Tooltip("몬스터의 등급입니다. (일반/정예)")]
        public MonsterGrade grade = MonsterGrade.Normal;

        [Tooltip("몬스터의 최대 체력입니다.")]
        public float maxHp = 100f;

        [Header("전투 설정")]
        [Tooltip("몬스터의 기본 공격력입니다.")]
        public float attackValue = 10f;

        [Header("행동 범위 설정")]
        [Tooltip("몬스터가 스폰 지점을 중심으로 배회할 수 있는 최대 반경입니다.")]
        public float patrolRange = 10f;

        [Tooltip("몬스터가 플레이어를 인식하고 추격을 시작하는 최대 거리입니다.")]
        public float detectionRange = 15f;

        [Header("전투 설정")]
        [Tooltip("몬스터의 공격 방식 및 공격 시작 거리입니다.")]
        public AttackType attackType = AttackType.Melee;

        [Tooltip("몬스터가 플레이어를 공격할 수 있는 최대 거리입니다.")]
        public float attackRange = 3f;

        [Header("특수공격 설정")]
        [Tooltip("파란 공격(Blue Attack) 발동에 필요한 일반 공격 횟수입니다. (일반: 6, 정예: 4)")]
        public int blueAttackThreshold = 6;

        [Tooltip("노란 공격(Yellow Attack) 발동에 필요한 일반 공격 횟수입니다. (정예: 9, 일반: 0이면 사용 안함)")]
        public int yellowAttackThreshold = 0;

        [Tooltip("특수 공격(파랑/노랑)에 진입하기 전, 경고 VFX 등을 표시하며 대기하는 시간(초)입니다.")]
        public float specialAttackReadyDuration = 1.0f;
    }
}