using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Movement.Assets._PlayerMovement.Scripts
{
    // Vehicle
    public abstract class VehicleActionState :IStateAction
    {
        protected VehicleActionState() { }
        public virtual void EnterAction() { }
        public virtual void ExitAction() { }
        public virtual void Update() { }
    }

    public class VehicleIdleState : VehicleActionState { }
    public class VehicleMoveState : VehicleActionState { }

    public class VehicleStateMachine : BaseStateMachine<MovementController, VehicleActionState>
    {
        public VehicleStateMachine(MovementController ctrl)
        {
            controller = ctrl;
            rb = ctrl.rb;

            currentState = new VehicleIdleState();
        }

        public void StartMoving()
        {
            SetState(new VehicleMoveState());
        }

        public void StopMoving()
        {
            SetState(new VehicleIdleState());
        }
    }
    
}
