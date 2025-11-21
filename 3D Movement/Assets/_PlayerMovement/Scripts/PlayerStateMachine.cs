using System;
using UnityEngine;

namespace Movement.Assets._PlayerMovement.Scripts
{
    // Player
    public abstract class PlayerActionState :IStateAction
    {
        protected PlayerActionState() { }

        public virtual void EnterAction() { }
        public virtual void ExitAction() { }
        public virtual void Update() { }
    }

    public class IdleState : PlayerActionState { }
    public class WalkState : PlayerActionState { }
    public class JumpState : PlayerActionState { }
    public class SwimState : PlayerActionState { }

    public class PlayerStateMachine : BaseStateMachine<MovementController, PlayerActionState>
    {
        public PlayerStateMachine(MovementController ctrl)
        {
            controller = ctrl;
            rb = ctrl.rb;

            currentState = new IdleState();
        }

        public void HandleEnvironmentChange(string env)
        {
            switch (env)
            {
                case "Water":
                    SetState(new SwimState());
                    break;
                case "Land":
                    SetState(new WalkState());
                    break;
                default:
                    SetState(new IdleState());
                    break;
            }
        }
    }

}



