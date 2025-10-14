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

        // TODO: 향후 몬스터 스킬 데이터를 연결할 부분을 추가할 수 있습니다.
        // public MonsterSkillData skill1;
        // public MonsterSkillData skill2;
    }
}