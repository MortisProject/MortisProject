// Assets/Skripts/World/Manager/VFXManager.cs
using System.Collections.Generic;
using UnityEngine;
using World.Effects;

namespace World.Manager
{
    /// <summary>
    /// VFX(파티클 시스템 등) 오브젝트 풀을 관리하는 싱글턴 매니저입니다.
    /// Tag를 사용하여 VFX를 요청하고 재생합니다.
    /// </summary>
    public class VFXManager : MonoBehaviour
    {
        #region Singleton
        public static VFXManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                // DontDestroyOnLoad(gameObject); // 필요에 따라 씬 전환 시 유지 설정
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            InitializePools();
        }
        #endregion

        [System.Serializable]
        public class Pool
        {
            [Tooltip("VFX를 식별하기 위한 고유 태그입니다.")]
            public string tag;
            [Tooltip("풀링할 VFX 프리팹입니다. ParticleSystem 컴포넌트가 있어야 합니다.")]
            public GameObject prefab;
            [Tooltip("초기에 생성할 VFX 개수입니다.")]
            public int size = 10;
        }

        [Header("VFX 풀 목록")]
        public List<Pool> pools;

        // 실제 VFX 오브젝트들을 저장할 딕셔너리 (Key: tag, Value: Queue of VFX GameObjects)
        private Dictionary<string, Queue<GameObject>> _poolDictionary;
        // 생성된 모든 풀 객체를 담을 부모 Transform
        private Transform _poolParent;

        /// <summary>
        /// Inspector에 정의된 풀 목록을 기반으로 오브젝트 풀을 초기화합니다.
        /// </summary>
        private void InitializePools()
        {
            _poolDictionary = new Dictionary<string, Queue<GameObject>>();
            _poolParent = new GameObject("-----[ Pool VFX ]").transform;
            //_poolParent.SetParent(this.transform); // VFXManager 하위에 정리 (선택 사항)

            foreach (Pool pool in pools)
            {
                Queue<GameObject> objectQueue = new Queue<GameObject>();
                for (int i = 0; i < pool.size; i++)
                {
                    GameObject obj = Instantiate(pool.prefab, _poolParent);

                    // --- 콜백 방식 연동을 위한 설정 ---
                    var autoReturn = obj.AddComponent<VFXAutoReturn>(); // 자동 반환 스크립트 추가
                    autoReturn.poolTag = pool.tag; // 자신의 태그 알려주기
                    // --- 연동 설정 끝 ---

                    obj.SetActive(false);
                    objectQueue.Enqueue(obj);
                }
                _poolDictionary.Add(pool.tag, objectQueue);
            }
        }

        /// <summary>
        /// 지정된 태그의 VFX를 풀에서 가져옵니다. (주로 지속 효과용)
        /// </summary>
        /// <param name="tag">가져올 VFX의 태그</param>
        /// <returns>비활성화된 VFX 게임 오브젝트 또는 null</returns>
        public GameObject GetFromPool(string tag)
        {
            if (!_poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning($"[VFXManager] Pool with tag '{tag}' doesn't exist.");
                return null;
            }

            if (_poolDictionary[tag].Count == 0)
            {
                // TODO: 풀이 비었을 때 동적으로 확장하는 로직 추가 가능
                Debug.LogWarning($"[VFXManager] Pool with tag '{tag}' is empty. Consider increasing pool size.");
                // 예시: 확장 로직
                // Pool pool = pools.Find(p => p.tag == tag);
                // if (pool != null) {
                //     GameObject obj = Instantiate(pool.prefab, _poolParent);
                //     var autoReturn = obj.AddComponent<VFXAutoReturn>();
                //     autoReturn.poolTag = pool.tag;
                //     obj.SetActive(false);
                //     return obj;
                // }
                return null;
            }

            GameObject objectToGet = _poolDictionary[tag].Dequeue();
            return objectToGet;
        }

        /// <summary>
        /// 사용이 끝난 VFX 오브젝트를 풀에 반환합니다. (VFXAutoReturn 스크립트가 주로 호출)
        /// </summary>
        /// <param name="tag">반환할 VFX의 태그</param>
        /// <param name="objectToReturn">반환할 게임 오브젝트</param>
        public void ReturnToPool(string tag, GameObject objectToReturn)
        {
            if (!_poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning($"[VFXManager] Trying to return object to non-existent pool with tag '{tag}'.");
                Destroy(objectToReturn); // 풀이 없으면 그냥 파괴
                return;
            }

            // 풀 부모 하위로 이동 (씬 정리를 위해)
            objectToReturn.transform.SetParent(_poolParent);
            objectToReturn.SetActive(false);
            _poolDictionary[tag].Enqueue(objectToReturn);
        }

        // --- VFX 재생 API ---

        /// <summary>
        /// 지정된 태그의 VFX를 특정 위치와 회전으로 재생합니다. (단발성 효과에 적합)
        /// </summary>
        /// <param name="tag">재생할 VFX 태그</param>
        /// <param name="position">월드 위치</param>
        /// <param name="rotation">월드 회전</param>
        /// <returns>재생된 VFX 게임 오브젝트 또는 null</returns>
        public GameObject PlayVFX(string tag, Vector3 position, Quaternion rotation)
        {
            GameObject vfxObject = GetFromPool(tag);
            if (vfxObject != null)
            {
                vfxObject.transform.SetPositionAndRotation(position, rotation);
                vfxObject.transform.SetParent(null); // 월드 기준으로 재생
                vfxObject.SetActive(true); // 활성화 시 VFXAutoReturn이 ParticleSystem을 Play함

                // 파티클 시스템 강제 재시작 (필요에 따라)
                var ps = vfxObject.GetComponent<ParticleSystem>();
                if (ps != null) ps.Play(true);
            }
            return vfxObject;
        }

        /// <summary>
        /// 지정된 태그의 VFX를 특정 위치에서 기본 회전으로 재생합니다. (단발성 효과에 적합)
        /// </summary>
        public GameObject PlayVFX(string tag, Vector3 position)
        {
            return PlayVFX(tag, position, Quaternion.identity);
        }

        /// <summary>
        /// 지정된 태그의 VFX를 특정 부모 Transform에 부착하여 로컬 위치/회전 0으로 재생합니다. (지속 효과에 적합)
        /// 사용 후에는 반드시 수동으로 ReturnToPool을 호출해야 합니다!
        /// </summary>
        /// <param name="tag">재생할 VFX 태그</param>
        /// <param name="parent">부착할 부모 Transform</param>
        /// <param name="localPosition">부모 기준 로컬 위치 (기본값: Zero)</param>
        /// <param name="localRotation">부모 기준 로컬 회전 (기본값: Identity)</param>
        /// <returns>재생된 VFX 게임 오브젝트 또는 null</returns>
        public GameObject PlayAttachedVFX(string tag, Transform parent, Vector3 localPosition = default, Quaternion localRotation = default)
        {
            GameObject vfxObject = GetFromPool(tag);
            if (vfxObject != null)
            {
                vfxObject.transform.SetParent(parent);
                vfxObject.transform.localPosition = localPosition == default ? Vector3.zero : localPosition;
                vfxObject.transform.localRotation = localRotation == default ? Quaternion.identity : localRotation;
                vfxObject.SetActive(true);

                var ps = vfxObject.GetComponent<ParticleSystem>();
                if (ps != null) ps.Play(true);

                // **중요:** 이 메서드로 재생된 VFX는 자동으로 반환되지 않으므로,
                // 호출한 쪽에서 사용이 끝나면 반드시 ReturnToPool(tag, vfxObject)를 호출해야 합니다.
                // (예: MonsterSpecialAttackReadyState의 Exit에서 호출)

                // 지속 효과는 VFXAutoReturn의 자동 반환 기능을 비활성화
                var autoReturn = vfxObject.GetComponent<VFXAutoReturn>();
                if (autoReturn != null) autoReturn.enabled = false;
            }
            return vfxObject;
        }
    }
}