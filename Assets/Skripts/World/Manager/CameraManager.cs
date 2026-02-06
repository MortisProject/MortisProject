// Assets/Skripts/World/Manager/CameraManager.cs
using UnityEngine;
using Unity.Cinemachine; // Cinemachine을 사용하기 위해 필수
using System.Collections;
using System.Collections.Generic; // Dictionary를 사용하기 위해 추가

namespace World.Manager
{
    /// <summary>
    /// 게임 내 모든 카메라 쉐이크(단발성/지속성)를 총괄하는 싱글턴 매니저입니다.
    /// </summary>
    public class CameraManager : MonoBehaviour
    {
        #region Singleton
        public static CameraManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            // --- 1. 단발성 쉐이크 초기화 ---
            // 인스펙터에서 설정한 리스트를 빠른 검색이 가능한 딕셔너리로 변환합니다.
            _impulseShakeDictionary = new Dictionary<ImpulseShakeType, CinemachineImpulseSource>();
            foreach (var entry in _impulseShakes)
            {
                if (entry.type != ImpulseShakeType.None && entry.impulseSource != null)
                {
                    _impulseShakeDictionary[entry.type] = entry.impulseSource;
                }
            }

            //--- 2. 지속성 쉐이크 초기화 (Perlin Noise) ---
            if (_continuousShakeCamera != null)
            {
                _perlinNoise = _continuousShakeCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
                if (_perlinNoise != null)
                {
                    _perlinNoise.AmplitudeGain = 0f; // 시작 시 진폭을 0으로 초기화
                }
                else
                {
                    Debug.LogWarning($"[CameraManager] '{_continuousShakeCamera.name}'에 'CinemachineBasicMultiChannelPerlin' 컴포넌트가 없습니다! 'Add Component'로 추가해주세요.", this);
                }
            }
            else
            {
                Debug.LogWarning("[CameraManager] 지속성 쉐이크용 가상 카메라(_continuousShakeCamera)가 할당되지 않았습니다.", this);
            }
        }
        #endregion

        // --- 1. 단발성 충격 (Impulse) ---

        [Header("단발성 충격 (Impulse) 설정")]
        [Tooltip("인스펙터에서 쉐이크 타입과 ImpulseSource 컴포넌트를 연결합니다.")]
        [SerializeField]
        private List<ImpulseShakeEntry> _impulseShakes;

        /// <summary>
        /// 단발성 쉐이크를 구분하기 위한 열거형입니다. (기획에 맞춰 추가/수정)
        /// </summary>
        public enum ImpulseShakeType
        {
            None,
            WeakHit,    // 약피격
            StrongHit   // 강피격
            // TODO: MonsterHit_Strong, PerfectGuard 등 필요에 따라 추가
        }

        // 쉐이크 타입과 컴포넌트를 연결하기 위한 헬퍼 클래스
        [System.Serializable]
        public class ImpulseShakeEntry
        {
            public ImpulseShakeType type;
            [Tooltip("이 쉐이크 타입에 사용할 CinemachineImpulseSource 컴포넌트입니다. (이 컴포넌트의 Time Envelope 커브로 강도 조절)")]
            public CinemachineImpulseSource impulseSource;
        }

        // 빠른 조회를 위한 딕셔너리 (런타임에 자동 생성됨)
        private Dictionary<ImpulseShakeType, CinemachineImpulseSource> _impulseShakeDictionary;

        /// <summary>
        /// (외부 호출) 지정된 타입의 단발성 카메라 쉐이크를 재생합니다.
        /// </summary>
        /// <param name="shakeType">재생할 쉐이크의 타입 (Enum)</param>
        public void PlayImpulse(ImpulseShakeType shakeType)
        {
            if (_impulseShakeDictionary.TryGetValue(shakeType, out CinemachineImpulseSource source))
            {
                // 연결된 ImpulseSource에서 쉐이크를 발생시킵니다.
                source.GenerateImpulse();
            }
            else
            {
                Debug.LogWarning($"[CameraManager] 요청된 ImpulseShakeType '{shakeType}'에 연결된 ImpulseSource가 없습니다.", this);
            }
        }

        // --- 2. 지속성 진동 (Perlin Noise) ---

        [Header("지속성 진동 (Noise) 설정")]
        [Tooltip("지속성 쉐이크(지진 등)를 적용할 가상 카메라입니다. (CinemachineNoise 컴포넌트 필요)")]
        [SerializeField]
        private CinemachineCamera _continuousShakeCamera;

        [Tooltip("쉐이크가 시작될 때의 강도 커브입니다. (X축: 시간, Y축: 진폭)")]
        [SerializeField]
        private AnimationCurve _shakeStartCurve;

        [Tooltip("쉐이크가 멈출 때의 강도 커브입니다. (X축: 시간, Y축: 진폭)")]
        [SerializeField]
        private AnimationCurve _shakeStopCurve;

        private CinemachineBasicMultiChannelPerlin _perlinNoise;

        // 현재 실행 중인 코루틴 (중복 실행 방지용)
        private Coroutine _continuousShakeCoroutine;

        /// <summary>
        /// (외부 호출) 지속성 카메라 쉐이크(지진)를 시작합니다.
        /// </summary>
        public void StartContinuousShake()
        {
            if (_perlinNoise == null) return;

            _perlinNoise.ReSeed();

            if (_continuousShakeCoroutine != null)
            {
                StopCoroutine(_continuousShakeCoroutine);
            }
            _continuousShakeCoroutine = StartCoroutine(ShakeCoroutine(_shakeStartCurve));
        }

        /// <summary>
        /// (외부 호출) 지속성 카메라 쉐이크(지진)를 중지합니다.
        /// </summary>
        public void StopContinuousShake()
        {
            if (_perlinNoise == null) return;

            if (_continuousShakeCoroutine != null)
            {
                StopCoroutine(_continuousShakeCoroutine);
            }
            _continuousShakeCoroutine = StartCoroutine(ShakeCoroutine(_shakeStopCurve));
        }

        /// <summary>
        /// AnimationCurve를 기반으로 CinemachineNoise의 진폭을 조절하는 코루틴입니다.
        /// </summary>
        private IEnumerator ShakeCoroutine(AnimationCurve curve)
        {
            float duration = curve.length > 0 ? curve.keys[curve.length - 1].time : 0f;
            float timer = 0f;

            while (timer < duration)
            {
                float strength = curve.Evaluate(timer);

                // [수정됨] PerlinNoise 컴포넌트의 'm_AmplitudeGain' 프로퍼티를 제어
                _perlinNoise.AmplitudeGain = strength;

                timer += Time.deltaTime;
                yield return null;
            }

            _perlinNoise.AmplitudeGain = curve.Evaluate(duration);
            _continuousShakeCoroutine = null;
        }
    }
}