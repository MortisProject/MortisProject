// Assets/Scripts/Monster/States/IMonsterAttackState.cs
namespace Monster.States
{
    /// <summary>
    /// 공격 애니메이션이 끝났을 때 Monster.cs로부터
    /// OnAttackFinished() 신호를 받아야 하는 모든 FSM 상태
    /// (BattleState, YellowAttackState, BlueAttackState)가
    /// 상속받는 인터페이스입니다.
    /// </summary>
    public interface IMonsterAttackState : IMonsterState
    {
        /// <summary>
        /// Monster.cs가 애니메이션 이벤트로부터 공격 종료 신호를 받았을 때 호출됩니다.
        /// </summary>
        void OnAttackFinished();
    }
}