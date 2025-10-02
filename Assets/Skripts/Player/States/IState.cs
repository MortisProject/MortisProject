// Assets/Scripts/Player/States/Core/IState.cs

namespace Player.States
{
    /// <summary>
    /// 모든 상태 클래스가 상속받아야 할 인터페이스입니다.
    /// 상태에 진입하고, 업데이트하고, 빠져나가는 기본 메서드를 정의합니다.
    /// </summary>
    public interface IState
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