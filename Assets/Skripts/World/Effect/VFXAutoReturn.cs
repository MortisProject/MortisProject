// Assets/Skripts/World/Effects/VFXAutoReturn.cs
using UnityEngine;
using World.Manager; // VFXManager를 사용하기 위해 추가

namespace World.Effects
{
    /// <summary>
    /// ParticleSystem이 재생을 완료하면 자동으로 VFXManager 풀에 반환하는 스크립트입니다.
    /// 단발성(One-shot) VFX 프리팹에 부착하여 사용합니다.
    /// </summary>
    [RequireComponent(typeof(ParticleSystem))]
    public class VFXAutoReturn : MonoBehaviour
    {
        [Tooltip("이 VFX가 반환될 풀의 태그입니다. VFXManager의 Pool Tag와 일치해야 합니다.")]
        public string poolTag;

        private ParticleSystem _particleSystem;
        private bool _isCallbackSetup = false; // 중복 설정 방지

        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();
            SetupStopActionCallback();
        }

        // 오브젝트가 활성화될 때마다 콜백이 유효한지 확인 (선택적이지만 안전)
        // PlayVFX 또는 PlayAttachedVFX 이후에 호출됨
        private void OnEnable()
        {
            // PlayAttachedVFX에서 비활성화했을 수 있으므로 다시 활성화
            this.enabled = true;

            // 이미 설정되었다면 다시 설정할 필요 없음
            if (!_isCallbackSetup) SetupStopActionCallback();

            // 풀에서 나올 때 파티클 시스템이 Play 상태가 아닐 수 있으므로 재생 (선택사항)
            // if (_particleSystem != null && !_particleSystem.isPlaying)
            // {
            //     _particleSystem.Play(true);
            // }
        }

        /// <summary>
        /// ParticleSystem의 Stop Action을 Callback으로 설정합니다.
        /// </summary>
        private void SetupStopActionCallback()
        {
            if (_particleSystem == null)
            {
                _particleSystem = GetComponent<ParticleSystem>();
                if (_particleSystem == null)
                {
                    Debug.LogError($"[VFXAutoReturn] ParticleSystem component not found on {gameObject.name}!", this);
                    return;
                }
            }

            var main = _particleSystem.main;
            // Stop Action이 이미 Callback이 아니거나, 아직 설정 안됐을 때만 설정
            if (main.stopAction != ParticleSystemStopAction.Callback)
            {
                main.stopAction = ParticleSystemStopAction.Callback;
                Debug.Log($"[VFXAutoReturn] Set stopAction to Callback for {gameObject.name}");
            }
            _isCallbackSetup = true;
        }

        /// <summary>
        /// ParticleSystem의 모든 파티클 재생이 완료되었을 때 자동으로 호출되는 콜백 메서드입니다.
        /// </summary>
        private void OnParticleSystemStopped()
        {
            // 이 스크립트가 활성화 상태일 때만 자동 반환 실행
            if (this.enabled)
            {
                if (string.IsNullOrEmpty(poolTag))
                {
                    Debug.LogError($"[VFXAutoReturn] Pool Tag is not set on {gameObject.name}! Cannot return to pool.", this);
                    return;
                }
                // VFXManager가 아직 초기화되지 않았거나 파괴된 경우 방지
                if (VFXManager.Instance != null)
                {
                    // Debug.Log($"[VFXAutoReturn] Particle system stopped on {gameObject.name}. Returning to pool '{poolTag}'.");
                    VFXManager.Instance.ReturnToPool(poolTag, gameObject);
                }
            }
            // else: 스크립트가 비활성화된 경우(예: PlayAttachedVFX), 자동 반환을 건너<0xEB><0x9C><0x85>니다.
        }

        // 풀에 반환될 때 콜백 설정이 유지되도록 OnDisable에서 특별히 처리할 필요는 없습니다.
        // private void OnDisable() { }
    }
}