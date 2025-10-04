// Assets/Scripts/Player/States/Airborne/PlayerFallState.cs
using Player.Animation;
using UnityEngine;

namespace Player.States
{
    public class PlayerFallState : IState
    {
        private readonly Player _player;
        private readonly PlayerStateMachine _stateMachine;
        private readonly PlayerMotor _motor;
        private readonly PlayerAnimationController _animController;

        // TODO: ���߿��� ĳ���͸� �ణ�� ������ �� �ִ� '���� ����' ����� �߰��ϸ� ���۰��� ���˴ϴ�.

        public PlayerFallState(Player player, PlayerStateMachine stateMachine, PlayerMotor motor, PlayerAnimationController animController)
        {
            _player = player;
            _stateMachine = stateMachine;
            _motor = motor;
            _animController = animController;
        }

        public void Enter()
        {
            // �߶� �ִϸ��̼��� ����ϵ��� �ִϸ����� �Ķ���͸� �����մϴ�.
            // TODO: PlayerAnimationController�� PlayFall() ���� �޼��带 �߰��ϸ� �����ϴ�.
            // _animController.PlayFall(true);
        }

        public void Update()
        {
            // �� �����Ӹ��� �����ߴ��� Ȯ���մϴ�.
            if (_stateMachine.IsGrounded)
            {       
                // ���� ��ȣ�� �ִϸ��̼� ��Ʈ�ѷ��� �����ϴ�.
                _animController.PlayLand();
                // �� ��, Idle ���·� ��ȯ�մϴ�.
                _stateMachine.ChangeState(_player.IdleState);
            }
        }

        public void Exit()
        {

        }
    }
}