// Assets/Scripts/Player/States/Core/IState.cs

namespace Player.States
{
    /// <summary>
    /// ��� ���� Ŭ������ ��ӹ޾ƾ� �� �������̽��Դϴ�.
    /// ���¿� �����ϰ�, ������Ʈ�ϰ�, ���������� �⺻ �޼��带 �����մϴ�.
    /// </summary>
    public interface IState
    {
        /// <summary>
        /// �ش� ���¿� ó�� �������� �� �� �� ȣ��˴ϴ�.
        /// </summary>
        void Enter();

        /// <summary>
        /// �ش� ���¿� �ӹ����� ���� �� ������ ȣ��˴ϴ�.
        /// </summary>
        void Update();

        /// <summary>
        /// �ش� ���¿��� �ٸ� ���·� ��ȯ�� �� �� �� ȣ��˴ϴ�.
        /// </summary>
        void Exit();
    }
}