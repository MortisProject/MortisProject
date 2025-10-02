// Assets/Scripts/Player/Animation/PlayerAnimationController.cs
using UnityEngine;

namespace Player.Animation
{
    /// <summary>
    /// Animator 컴포넌트를 직접 제어하는 중앙 컨트롤러.
    /// 모든 상태(State)는 이 스크립트를 통해 애니메이션 재생을 요청합니다.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class PlayerAnimationController : MonoBehaviour
    {
        // Animator 파라미터 이름을 미리 해시값으로 변환하여 성능 향상
        private readonly int _moveXHash = Animator.StringToHash("MoveX");
        private readonly int _moveYHash = Animator.StringToHash("MoveY");
        private readonly int _jumpHash = Animator.StringToHash("Jump");

        // TODO: 추후 공격, 닷지 등의 애니메이션 해시값을 여기에 추가합니다.
        // private readonly int _attackTriggerHash = Animator.StringToHash("Attack");

        private Animator _animator;

        private void Awake()
        {
            // Animator 컴포넌트를 캐싱
            _animator = GetComponent<Animator>();
        }

        /// <summary>
        /// 이동 애니메이션의 블렌드 트리 값을 설정합니다.
        /// </summary>
        /// <param name="x">좌우 이동 값</param>
        /// <param name="y">앞뒤 이동 값</param>
        public void SetMove(float x, float y)
        {
            _animator.SetFloat(_moveXHash, x, 0.1f, Time.deltaTime);
            _animator.SetFloat(_moveYHash, y, 0.1f, Time.deltaTime);
        }

        /// <summary>
        /// 점프 애니메이션을 재생합니다.
        /// </summary>
        public void PlayJump()
        {
            _animator.SetTrigger(_jumpHash);
        }
    }
}