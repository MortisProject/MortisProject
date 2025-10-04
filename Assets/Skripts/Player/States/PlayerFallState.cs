// Assets/Scripts/Player/States/Airborne/PlayerFallState.cs
using Player.Animation;
using Player.Data; // ������ ���� �߰�
using UnityEngine;

namespace Player.States
{
    // IState ��� PlayerAirborneState�� ��ӹ޽��ϴ�.
    public class PlayerFallState : PlayerAirborneState
    {
        // �����ڿ��� �θ� Ŭ������ �ʿ��� ��� ������ �����մϴ�.
        public PlayerFallState(Player player, PlayerStateMachine stateMachine, PlayerInput input, PlayerMotor motor, PlayerSO data, PlayerAnimationController animController)
            : base(player, stateMachine, input, motor, data, animController)
        {
        }

        public override void Enter()
        {
            base.Enter();
        }

        public override void Update()
        {
            base.Update();

            if (_stateMachine.IsGrounded)
            {
                _stateMachine.ChangeState(_player.IdleState);
                return;
            }
        }
    }
}