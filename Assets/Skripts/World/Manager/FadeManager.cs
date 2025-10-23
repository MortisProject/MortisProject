// Assets/Skripts/World/Manager/FadeManager.cs
using Player;
using System.Collections;
using UnityEngine;

namespace World.Manager
{
    /// <summary>
    /// 페이드 인/아웃 연출과 텔레포트 로직을 제어하는 싱글턴 매니저입니다.
    /// </summary>
    public class FadeManager : MonoBehaviour
    {
        #region Singleton
        public static FadeManager Instance { get; private set; }

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
            }
        }
        #endregion

        [Header("UI 참조")]
        [Tooltip("페이드 효과에 사용할 CanvasGroup입니다. (Alpha 값을 조절합니다)")]
        [SerializeField] private CanvasGroup _fadeCanvasGroup;

        [Header("페이드 설정")]
        [Tooltip("페이드 인/아웃에 걸리는 시간(초)입니다.")]
        [SerializeField] private float _fadeDuration = 0.5f;

        // 현재 페이드 연출이 실행 중인지 확인하는 플래그 (중복 실행 방지)
        private bool _isFading = false;
        private Coroutine _activeFadeCoroutine;

        private void Start()
        {
            // 게임 시작 시 화면이 투명한지 확인
            if (_fadeCanvasGroup != null)
            {
                _fadeCanvasGroup.alpha = 0f;
            }
        }

        /// <summary>
        /// 화면을 검게 암전시킨 후, 플레이어를 지정된 위치로 이동시키고 다시 밝힙니다.
        /// </summary>
        /// <param name="player">텔레포트시킬 Player 객체</param>
        /// <param name="destination">목표 위치</param>
        public void StartFadeAndTeleport(Player.Player player, Vector3 destination)
        {
            // 이미 연출 중이라면 중복 실행하지 않음
            if (_isFading) return;

            // 새 코루틴 시작
            if (_activeFadeCoroutine != null)
            {
                StopCoroutine(_activeFadeCoroutine);
            }
            _activeFadeCoroutine = StartCoroutine(FadeAndTeleportCoroutine(player, destination));
        }

        /// <summary>
        /// 실제 페이드 및 텔레포트를 처리하는 코루틴입니다.
        /// </summary>
        private IEnumerator FadeAndTeleportCoroutine(Player.Player player, Vector3 destination)
        {
            _isFading = true;

            // 1. 페이드 아웃 (화면 검게)
            yield return StartCoroutine(FadeCoroutine(1f));

            // 2. 실제 텔레포트 로직 (화면이 검을 때 실행)
            player.Motor.Stop(); // [중요] 낙하 속도 등 모든 관성 제거
            player.transform.position = destination; // 위치 즉시 이동
            // [중요] 플레이어가 공중 상태(FallState 등)에 빠져있을 수 있으므로 Idle로 강제 전환
            player.StateMachine.ChangeState(player.IdleState);

            // 3. 페이드 인 (화면 다시 밝게)
            // (텔레포트 후 다음 프레임에 바로 밝아지면 어색할 수 있으므로 아주 살짝 대기)
            yield return new WaitForSeconds(0.1f);
            yield return StartCoroutine(FadeCoroutine(0f));

            _isFading = false;
            _activeFadeCoroutine = null;
        }

        /// <summary>
        /// CanvasGroup의 Alpha 값을 목표 값으로 부드럽게 변경하는 헬퍼 코루틴입니다.
        /// </summary>
        /// <param name="targetAlpha">목표 Alpha 값 (0.0 ~ 1.0)</param>
        private IEnumerator FadeCoroutine(float targetAlpha)
        {
            float startAlpha = _fadeCanvasGroup.alpha;
            float timer = 0f;

            while (timer < _fadeDuration)
            {
                // 불릿타임의 영향을 받지 않도록 unscaledDeltaTime 사용
                timer += Time.unscaledDeltaTime;
                _fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / _fadeDuration);
                yield return null;
            }

            // 정확한 목표 값으로 설정
            _fadeCanvasGroup.alpha = targetAlpha;
        }
    }
}