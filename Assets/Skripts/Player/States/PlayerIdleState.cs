// Assets/Scripts/Player/States/Grounded/PlayerIdleState.cs
using UnityEngine;
using Player.Animation;

namespace Player.States
{
    public class PlayerIdleState : PlayerGroundedState
    {
        public PlayerIdleState(Player player, PlayerStateMachine stateMachine, PlayerInput input, PlayerMotor motor, PlayerAnimationController animController)
            : base(player, stateMachine, input, motor, animController)
        {
        }

        public override void Enter()
        {
            base.Enter(); // �θ� Ŭ������ Enter ���� ����

            // Idle ���¿� ���������Ƿ�, �̵� �ִϸ��̼� �Ķ���͸� 0���� ����
            //_animController.SetMove(0f, 0f);
        }

        public override void Update()
        {
            base.Update(); // �θ� Ŭ������ Update ����(���� ��) ����

            _animController.SetMove(0f, 0f);
            // �̵� �Է��� �ִ��� Ȯ��
            if (_input.MoveInput.sqrMagnitude > 0.01f)
            {
                // �̵� �Է��� �ִٸ�, Move ���·� ��ȯ
                _stateMachine.ChangeState(_player.MoveState);
                return; // ���� ��ȯ�� �Ͼ���Ƿ� �� �̻� Idle ������ ������ �������� ����
            }
        }

        public override void Exit()
        {
            base.Exit(); // �θ� Ŭ������ Exit ���� ����
        }
    }
}