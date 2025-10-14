// Assets/Scripts/Monster/States/IMonsterState.cs
namespace Monster.States
{
    /// <summary>
    /// 모든 몬스터 상태 클래스가 상속받아야 할 인터페이스입니다.
    /// </summary>
    public interface IMonsterState
    {
        /// <summary>
        /// 해당 상태에 처음 진입했을 때 한 번 호출됩니다.
        /// </summary>
        void Enter();

        /// <summary>
        /// 해당 상태에 머무르는 동안 매 프레임 호출됩니다.
        /// </summary>
        void Update();

        /// <summary>
        /// 해당 상태에서 다른 상태로 전환될 때 한 번 호출됩니다.
        /// </summary>
        void Exit();
    }
}