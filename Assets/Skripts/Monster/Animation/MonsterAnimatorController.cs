// Assets/Scripts/Monster/Animation/MonsterAnimatorController.cs
using UnityEngine;

namespace Monster.Animation
{
    /// <summary>
    /// 몬스터 Animator 컴포넌트를 직접 제어하는 중앙 컨트롤러입니다.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class MonsterAnimatorController : MonoBehaviour
    {
        private Animator _animator;

        // Animator 파라미터 이름을 미리 해시값으로 변환하여 성능을 최적화합니다.
        private readonly int _isWalkingHash = Animator.StringToHash("IsWalking");
        private readonly int _isRunningHash = Animator.StringToHash("IsRunning");
        private readonly int _attackHash = Animator.StringToHash("Attack");
        private readonly int _hitHash = Animator.StringToHash("Hit");
        private readonly int _dieHash = Animator.StringToHash("Die");

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        // --- public 메서드를 통해 상태(State)가 애니메이션을 제어합니다. ---

        public void SetWalking(bool isWalking) => _animator.SetBool(_isWalkingHash, isWalking);
        public void SetRunning(bool isRunning) => _animator.SetBool(_isRunningHash, isRunning);
        public void PlayAttack() => _animator.SetTrigger(_attackHash);
        public void PlayHit() => _animator.SetTrigger(_hitHash);
        public void PlayDie() => _animator.SetTrigger(_dieHash);
    }
}