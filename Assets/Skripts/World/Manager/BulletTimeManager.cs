// Assets/Skripts/World/Manager/BulletTimeManager.cs
using System.Collections;
using UnityEngine;

namespace World.Manager
{
    /// <summary>
    /// 게임의 전반적인 시간 흐름(Time.timeScale)을 제어하는 싱글턴 매니저입니다.
    /// 퍼펙트 회피, 컷씬 연출 등 다양한 상황에서 사용될 수 있습니다.
    /// </summary>
    public class BulletTimeManager : MonoBehaviour
    {
        #region Singleton

        // 싱글턴 인스턴스: 게임 내 어디서든 'BulletTimeManager.Instance'로 접근 가능합니다.
        public static BulletTimeManager Instance { get; private set; }

        private void Awake()
        {
            // 싱글턴 인스턴스를 설정합니다.
            if (Instance == null)
            {
                Instance = this;
                // 씬이 전환되어도 이 매니저가 파괴되지 않도록 설정합니다.
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                // 이미 인스턴스가 존재하면 중복 생성된 오브젝트를 파괴합니다.
                Destroy(gameObject);
            }
        }

        #endregion

        [Header("Default Settings")]
        [Tooltip("기본적으로 사용할 불릿타임의 시간 배율입니다. (0.0 ~ 1.0)")]
        [Range(0f, 1f)]
        public float defaultTimeScale = 0.1f;

        [Tooltip("시간 배율이 부드럽게 변하는 기본 지속시간(초)입니다.")]
        public float defaultTransitionDuration = 0.2f;

        // 현재 실행 중인 시간 제어 코루틴을 저장하여 중복 실행을 방지합니다.
        private Coroutine _activeBulletTimeCoroutine;

        /// <summary>
        /// 지정된 시간 동안 부드럽게 불릿타임을 시작하고 자동으로 종료합니다.
        /// </summary>
        /// <param name="duration">불릿타임이 지속될 시간(초)입니다.</param>
        public void StartBulletTime(float duration)
        {
            // StartBulletTime의 모든 파라미터를 받는 버전을 기본값으로 호출합니다.
            StartBulletTime(defaultTimeScale, duration, defaultTransitionDuration, defaultTransitionDuration);
        }

        /// <summary>
        /// 모든 옵션을 직접 설정하여, 지정된 시간 동안 부드럽게 불릿타임을 시작하고 자동으로 종료합니다.
        /// </summary>
        /// <param name="targetScale">목표 시간 배율 (0.0 ~ 1.0)</param>
        /// <param name="duration">불릿타임 지속 시간(초)</param>
        /// <param name="fadeInTime">시간이 느려지는 데 걸리는 시간(초)</param>
        /// <param name="fadeOutTime">시간이 원래대로 돌아오는 데 걸리는 시간(초)</param>
        public void StartBulletTime(float targetScale, float duration, float fadeInTime, float fadeOutTime)
        {
            // 이전에 실행 중인 불릿타임 코루틴이 있다면 중지시킵니다.
            if (_activeBulletTimeCoroutine != null)
            {
                StopCoroutine(_activeBulletTimeCoroutine);
            }
            // 새로운 코루틴을 시작하고 변수에 저장합니다.
            _activeBulletTimeCoroutine = StartCoroutine(BulletTimeCoroutine(targetScale, duration, fadeInTime, fadeOutTime));
        }

        /// <summary>
        /// 불릿타임을 강제로 시작합니다. ForceStopBulletTime()을 호출하기 전까지 지속됩니다.
        /// </summary>
        /// <param name="targetScale">목표 시간 배율 (0.0 ~ 1.0)</param>
        public void ForceStartBulletTime(float targetScale)
        {
            if (_activeBulletTimeCoroutine != null)
            {
                StopCoroutine(_activeBulletTimeCoroutine);
            }
            _activeBulletTimeCoroutine = StartCoroutine(TransitionToScale(targetScale, defaultTransitionDuration));
        }

        /// <summary>
        /// 강제로 시작된 불릿타임을 부드럽게 종료합니다.
        /// </summary>
        public void ForceStopBulletTime()
        {
            if (_activeBulletTimeCoroutine != null)
            {
                StopCoroutine(_activeBulletTimeCoroutine);
            }
            _activeBulletTimeCoroutine = StartCoroutine(TransitionToScale(1.0f, defaultTransitionDuration));
        }

        /// <summary>
        /// 실제 시간 배율(Time.timeScale)을 변경하는 코루틴입니다.
        /// </summary>
        private IEnumerator BulletTimeCoroutine(float targetScale, float duration, float fadeInTime, float fadeOutTime)
        {
            // 1. Fade In: 목표 배율로 서서히 시간을 느리게 만듭니다.
            yield return StartCoroutine(TransitionToScale(targetScale, fadeInTime));

            // 2. Duration: 지정된 시간만큼 불릿타임을 유지합니다.
            yield return new WaitForSecondsRealtime(duration); // Time.timeScale에 영향을 받지 않는 대기

            // 3. Fade Out: 원래 시간(1.0)으로 서서히 복귀합니다.
            yield return StartCoroutine(TransitionToScale(1.0f, fadeOutTime));

            // 코루틴이 끝났음을 표시합니다.
            _activeBulletTimeCoroutine = null;
        }

        /// <summary>
        /// 현재 시간 배율에서 목표 배율까지 부드럽게 변경하는 코루틴입니다.
        /// </summary>
        private IEnumerator TransitionToScale(float targetScale, float transitionTime)
        {
            float startScale = Time.timeScale;
            float elapsedTime = 0f;

            while (elapsedTime < transitionTime)
            {
                // Lerp를 사용하여 현재 시간 배율을 목표 배율로 부드럽게 보간합니다.
                Time.timeScale = Mathf.Lerp(startScale, targetScale, elapsedTime / transitionTime);
                elapsedTime += Time.unscaledDeltaTime; // Time.timeScale에 영향을 받지 않는 시간 변화량
                yield return null; // 다음 프레임까지 대기
            }

            // 전환이 끝난 후 정확한 목표 배율로 설정합니다.
            Time.timeScale = targetScale;
        }
    }
}