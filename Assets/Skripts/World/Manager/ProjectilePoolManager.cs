// Assets/Skripts/World/ProjectilePoolManager.cs
using Player.Combat;
using System.Collections.Generic;
using UnityEngine;

namespace World
{
    /// <summary>
    /// 모든 발사체 오브젝트를 관리하는 중앙 오브젝트 풀입니다. (싱글턴)
    /// </summary>
    public class ProjectilePoolManager : MonoBehaviour
    {
        // 싱글턴 인스턴스: 씬의 어디에서든 'ProjectilePoolManager.Instance'로 접근 가능
        public static ProjectilePoolManager Instance { get; private set; }

        [System.Serializable]
        public class Pool
        {
            [Tooltip("구분을 위한 발사체의 이름 태그입니다.")]
            public string tag;
            [Tooltip("풀링할 발사체 프리팹입니다.")]
            public GameObject prefab;
            [Tooltip("초기에 생성할 발사체의 개수입니다.")]
            public int size;
        }

        [Header("Pools")]
        [Tooltip("관리할 발사체 풀 목록입니다.")]
        public List<Pool> pools;

        // 실제 오브젝트들을 저장할 딕셔너리
        private Dictionary<string, Queue<GameObject>> _poolDictionary;

        private void Awake()
        {
            // 싱글턴 인스턴스 설정
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject); // 중복 생성 방지
                return;
            }

            _poolDictionary = new Dictionary<string, Queue<GameObject>>();

            // 각 풀에 대해 미리 오브젝트를 생성합니다.
            foreach (Pool pool in pools)
            {
                Queue<GameObject> objectQueue = new Queue<GameObject>();
                for (int i = 0; i < pool.size; i++)
                {
                    GameObject obj = Instantiate(pool.prefab);
                    obj.SetActive(false); // 비활성화 상태로 생성
                    objectQueue.Enqueue(obj);
                }
                _poolDictionary.Add(pool.tag, objectQueue);
            }
        }

        /// <summary>
        /// 지정된 태그의 발사체를 풀에서 가져옵니다.
        /// </summary>
        public GameObject GetFromPool(string tag)
        {
            if (!_poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning($"Pool with tag '{tag}' doesn't exist.");
                return null;
            }

            // TODO: 만약 풀이 비어있다면 동적으로 새로 생성하는 로직을 추가할 수 있습니다.
            GameObject objectToSpawn = _poolDictionary[tag].Dequeue();
            return objectToSpawn;
        }

        /// <summary>
        /// 사용이 끝난 발사체를 다시 풀에 반납합니다.
        /// </summary>
        public void ReturnToPool(string tag, GameObject objectToReturn)
        {
            if (!_poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning($"Pool with tag '{tag}' doesn't exist.");
                return;
            }

            objectToReturn.SetActive(false); // 비활성화하여 반납
            _poolDictionary[tag].Enqueue(objectToReturn);
        }
    }
}