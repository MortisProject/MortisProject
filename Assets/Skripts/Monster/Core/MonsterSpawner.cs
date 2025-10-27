// Assets/Scripts/Monster/Core/MonsterSpawner.cs
using Monster.Manager;
using System.Collections.Generic;
using UnityEngine;

namespace Monster
{
    /// <summary>
    /// 외부 신호(Trigger)를 받아 지정된 위치에 몬스터를 스폰시킵니다.
    public class MonsterSpawner : MonoBehaviour
    {
        [Header("스폰 설정")]
        [Tooltip("이 스포너가 관리할 모든 MonsterSpawnPoint를 여기에 할당합니다.")]
        [SerializeField] private MonsterSpawnPoint[] _spawnPoints;

        [Header("트리거 설정")]
        [Tooltip("이 스포너를 활성화시킬 외부 트리거 오브젝트를 할당합니다. (스포너 초기화 시 재활성화 용도)")]
        [SerializeField] private GameObject _triggerObject;

        // 한 번 스폰이 실행되었는지 확인하는 플래그
        private bool _hasSpawned = false;

        // 이 스포너가 활성화시킨 몬스터들의 목록
        private List<GameObject> _spawnedMonsters = new List<GameObject>();

        [Header("특수공격 쿨다운 (런타임)")]
        [Tooltip("현재 이 그룹(스포너)이 특수 공격(파랑/노랑) 쿨다운 중인지 여부입니다.")]
        [SerializeField] private bool _isGroupSpecialAttackOnCooldown = false;

        /// <summary>
        /// 외부(SpawnTrigger)에서 호출하여 몬스터 스폰을 시작합니다.
        /// </summary>
        public void ActivateSpawner()
        {
            // 스폰된 적이 없을 때만 실행합니다.
            if (_hasSpawned) return;

            SpawnMonsters();
            _hasSpawned = true;
        }

        /// <summary>
        /// 모든 스폰 포인트에 몬스터를 스폰합니다.
        /// </summary>
        private void SpawnMonsters()
        {
            Debug.Log($"{name}에서 몬스터 스폰 시작!");
            foreach (var point in _spawnPoints)
            {
                // 1. 몬스터 풀 매니저에서 몬스터를 가져옵니다.
                GameObject monsterObj = MonsterPoolManager.Instance.GetFromPool(point.monsterTag);

                if (monsterObj != null)
                {
                    // 2. 몬스터의 위치와 회전을 스폰 포인트에 맞춥니다.
                    monsterObj.transform.position = point.transform.position;
                    monsterObj.transform.rotation = point.transform.rotation;

                    // 3. 몬스터 컴포넌트를 가져와 상태를 리셋하고 정보를 설정합니다.
                    if (monsterObj.TryGetComponent<Monster>(out Monster monster))
                    {
                        monster.Setup(point.monsterTag, this); // 몬스터에게 태그를 알려줍니다.
                        monster.ResetMonster();          // 몬스터의 모든 상태를 초기화합니다.
                    }

                    // 4. 몬스터를 활성화하고 목록에 추가합니다.
                    monsterObj.SetActive(true);
                    _spawnedMonsters.Add(monsterObj);
                }
            }
        }

        /// <summary>
        /// (플레이어 사망 시 호출) 스포너를 초기화하여 다시 몬스터를 스폰할 수 있게 합니다.
        /// </summary>
        public void ResetSpawner()
        {
            // 이미 스폰된 몬스터가 있다면 모두 풀에 반납합니다.
            foreach (var monsterObj in _spawnedMonsters)
            {
                if (monsterObj.activeSelf)
                {
                    var monster = monsterObj.GetComponent<Monster>();
                    MonsterPoolManager.Instance.ReturnToPool(monster.PoolTag, monsterObj);
                }
            }
            _spawnedMonsters.Clear();
            _hasSpawned = false;
            _isGroupSpecialAttackOnCooldown = false;

            // 연결된 트리거 오브젝트가 있다면, 다시 활성화시켜줍니다.
            if (_triggerObject != null)
            {
                _triggerObject.SetActive(true);
            }

            Debug.Log($"{name} 스포너가 초기화되었습니다.");
        }

        /// <summary>
        /// (Monster.cs가 호출) 그룹 내 몬스터가 특수 공격(파랑/노랑) 사용을 '요청'합니다.
        /// </summary>
        /// <returns>사용 가능하면 true, 쿨다운 중이라 불가능하면 false</returns>
        public bool RequestSpecialAttack()
        {
            if (_isGroupSpecialAttackOnCooldown)
            {
                return false; // 다른 몬스터가 이미 사용 중 (쿨다운 중)
            }

            // 쿨다운을 활성화하고 사용권을 부여합니다.
            _isGroupSpecialAttackOnCooldown = true;
            return true;
        }

        /// <summary>
        /// (Monster.cs가 호출) 특수 공격이 끝난 몬스터가 그룹 쿨다운을 '초기화'합니다.
        /// </summary>
        public void ResetSpecialAttackCooldown()
        {
            _isGroupSpecialAttackOnCooldown = false;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            // 스포너와 각 스폰 포인트 간의 연결을 선으로 표시합니다.
            if (_spawnPoints == null || _spawnPoints.Length == 0) return;

            Gizmos.color = Color.green; // 초록색
            foreach (var point in _spawnPoints)
            {
                if (point != null)
                {
                    Gizmos.DrawLine(transform.position, point.transform.position);
                }
            }
        }
#endif
    }
}