// Assets/Scripts/Monster/Manager/MonsterPoolManager.cs
using System.Collections.Generic;
using UnityEngine;

namespace Monster.Manager
{
    /// <summary>
    /// 모든 몬스터 오브젝트를 관리하는 중앙 오브젝트 풀입니다. (싱글턴)
    /// </summary>
    public class MonsterPoolManager : MonoBehaviour
    {
        // 싱글턴 인스턴스
        public static MonsterPoolManager Instance { get; private set; }

        [System.Serializable]
        public class Pool
        {
            [Tooltip("구분을 위한 몬스터의 이름 태그입니다.")]
            public string tag;
            [Tooltip("풀링할 몬스터 프리팹입니다.")]
            public GameObject prefab;
            [Tooltip("초기에 생성할 몬스터의 개수입니다.")]
            public int size;
        }

        [Header("몬스터 풀 목록")]
        public List<Pool> pools;

        // 실제 몬스터 오브젝트들을 저장할 딕셔너리
        private Dictionary<string, Queue<GameObject>> _poolDictionary;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            // 풀링된 몬스터들을 담을 부모 오브젝트를 생성합니다.
            GameObject monsterPoolParent = new GameObject("-----[ Pool Monster ]");
            _poolDictionary = new Dictionary<string, Queue<GameObject>>();

            foreach (Pool pool in pools)
            {
                Queue<GameObject> objectQueue = new Queue<GameObject>();
                for (int i = 0; i < pool.size; i++)
                {
                    GameObject obj = Instantiate(pool.prefab, monsterPoolParent.transform);
                    obj.SetActive(false);
                    objectQueue.Enqueue(obj);
                }
                _poolDictionary.Add(pool.tag, objectQueue);
            }
        }

        /// <summary>
        /// 지정된 태그의 몬스터를 풀에서 가져옵니다.
        /// </summary>
        public GameObject GetFromPool(string tag)
        {
            if (!_poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning($"Pool with tag '{tag}' doesn't exist.");
                return null;
            }

            // 풀에 남아있는 오브젝트가 없으면 null을 반환합니다.
            if (_poolDictionary[tag].Count == 0)
            {
                Debug.LogWarning($"Pool with tag '{tag}' is empty.");
                return null;
            }

            GameObject objectToSpawn = _poolDictionary[tag].Dequeue();
            return objectToSpawn;
        }

        /// <summary>
        /// 사용이 끝난 몬스터를 다시 풀에 반납합니다.
        /// </summary>
        public void ReturnToPool(string tag, GameObject objectToReturn)
        {
            if (!_poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning($"Pool with tag '{tag}' doesn't exist.");
                return;
            }

            objectToReturn.SetActive(false);
            _poolDictionary[tag].Enqueue(objectToReturn);
        }
    }
}