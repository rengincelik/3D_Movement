using System;
using System.Collections.Generic;
using UnityEngine;


namespace Movement.Assets._PlayerMovement.Scripts
{
    public enum EnvironmentType { Land, Water, Ice, Grass, Road, Air }

    public enum VehiclerType { Car, Train, Boat, Plane}

    public class PlayerStateMachine
    {
        public Rigidbody rb;
        public PlayerMovementController controller;
        private EnvironmentState currentEnvironmentState;

        // Events
        public event Action<EnvironmentState> OnEnvironmentChanged;
        public event Action<ActionState> OnActionChanged;

        // Event fırlatma metodları (EnvironmentState'ten çağrılacak)
        public void NotifyActionChanged(ActionState newAction)
        {
            OnActionChanged?.Invoke(newAction);
        }

        private readonly Dictionary<EnvironmentType, EnvironmentState> availableEnvironments;

        public PlayerStateMachine(PlayerMovementController ctrl)
        {
            controller = ctrl;
            rb = ctrl.rb;

            availableEnvironments = new Dictionary<EnvironmentType, EnvironmentState>
            {
                { EnvironmentType.Land, new LandState(this) },
                { EnvironmentType.Water, new WaterState(this) },
                { EnvironmentType.Ice, new IceState(this) },
                { EnvironmentType.Grass, new GrassState(this) }
            };

            currentEnvironmentState = availableEnvironments[EnvironmentType.Land];

            // controller.OnCollided += HandleEnvironmentChanged;
            currentEnvironmentState.EnterEnvironment();
        }

        public void ExitStateMachine()
        {
            // controller.OnCollided -= HandleEnvironmentChanged;
            currentEnvironmentState.ExitEnvironment();
        }

        private void HandleEnvironmentChanged(string envString)
        {
            if (Enum.TryParse<EnvironmentType>(envString, true, out var env))
            {
                if (availableEnvironments.TryGetValue(env, out var nextEnv))
                {
                    ChangeEnvironment(nextEnv);
                }
            }
            else
            {
                Debug.LogWarning($"Unknown environment: {envString}");
            }
        }

        private void ChangeEnvironment(EnvironmentState newEnv)
        {
            if (currentEnvironmentState == newEnv) return;

            currentEnvironmentState.ExitEnvironment();
            currentEnvironmentState = newEnv;
            currentEnvironmentState.EnterEnvironment();

            OnEnvironmentChanged?.Invoke(currentEnvironmentState);
            Debug.Log($"[StateMachine] Environment changed to: {newEnv.GetType().Name}");
        }

        public void Update()
        {
            currentEnvironmentState.Update();
        }

        // Query metodları (opsiyonel - dışarıdan state bilgisi almak için)
        public EnvironmentState GetCurrentEnvironment() => currentEnvironmentState;
        public ActionState GetCurrentAction() => currentEnvironmentState?.GetCurrentAction();
    }


    public abstract class EnvironmentState
    {
        public PlayerStateMachine machine;
        protected ActionState currentActionState;
        protected Dictionary<Type, ActionState> availableActions;

        protected EnvironmentState(PlayerStateMachine machine)
        {
            this.machine = machine;
            availableActions = new Dictionary<Type, ActionState>();
        }

        public virtual void EnterEnvironment()
        {
            currentActionState = DetermineActionState();
            Debug.Log($"action is {currentActionState}");
            currentActionState?.EnterAction();
            machine.NotifyActionChanged(currentActionState);
        }

        public virtual void ExitEnvironment()
        {
            currentActionState?.ExitAction();
        }

        public virtual void Update()
        {
            ActionState next = DetermineActionState();
            if (next != null && next.GetType() != currentActionState?.GetType())
                ChangeActionState(next);

            currentActionState?.Update();
        }

        protected void ChangeActionState(ActionState newAction)
        {
            currentActionState?.ExitAction();
            currentActionState = newAction;
            currentActionState.EnterAction();

            // Event'i machine üzerinden fırlat
            machine.NotifyActionChanged(newAction);
            Debug.Log($"[EnvironmentState] Action changed to: {newAction.GetType().Name}");
        }

        protected abstract ActionState DetermineActionState();

        // Query metodu
        public ActionState GetCurrentAction() => currentActionState;
    }
    public class LandState : EnvironmentState
    {
        public LandState(PlayerStateMachine machine) : base(machine)
        {
            availableActions = new Dictionary<Type, ActionState>
            {
                { typeof(IdleState), new IdleState(this) },
                { typeof(WalkState), new WalkState(this) },
                { typeof(JumpState), new JumpState(this) }
            };
            currentActionState = availableActions[typeof(IdleState)];
        }

        protected override ActionState DetermineActionState()
        {
            if (Mathf.Abs(machine.rb.linearVelocity.x) > 0.01f)
                return availableActions[typeof(JumpState)];
            else if (Mathf.Abs(machine.rb.linearVelocity.x) > 0.01f)
                return availableActions[typeof(WalkState)];
            else
                return availableActions[typeof(IdleState)];
        }

    }

    public class WaterState : EnvironmentState
    {
        public WaterState(PlayerStateMachine machine) : base(machine)
        {
            availableActions = new Dictionary<Type, ActionState>
            {
                { typeof(SwimState), new SwimState(this) }
            };
            currentActionState = availableActions[typeof(SwimState)];
        }

        protected override ActionState DetermineActionState()
        {
            return availableActions[typeof(SwimState)]; // şimdilik tek action
        }
    }

    public class IceState : EnvironmentState
    {
        public IceState(PlayerStateMachine machine) : base(machine)
        {
            availableActions = new Dictionary<Type, ActionState>
            {
                { typeof(IdleState), new IdleState(this) },
                { typeof(WalkState), new WalkState(this) }
            };
            currentActionState = availableActions[typeof(IdleState)];
        }

        protected override ActionState DetermineActionState()
        {
            if (Mathf.Abs(machine.rb.linearVelocity.x) > 0.01f)
                return availableActions[typeof(WalkState)];
            else
                return availableActions[typeof(IdleState)];
        }
    }

    public class GrassState : EnvironmentState
    {
        public GrassState(PlayerStateMachine machine) : base(machine)
        {
            availableActions = new Dictionary<Type, ActionState>
            {
                { typeof(IdleState), new IdleState(this) },
                { typeof(WalkState), new WalkState(this) }
            };
            currentActionState = availableActions[typeof(IdleState)];
        }

        protected override ActionState DetermineActionState()
        {
            if (Mathf.Abs(machine.rb.linearVelocity.x) > 0.01f)
                return availableActions[typeof(WalkState)];
            else
                return availableActions[typeof(IdleState)];
        }
    }

    public abstract class ActionState
    {
        protected EnvironmentState envState;
        protected PlayerStateMachine machine;


        protected ActionState(EnvironmentState env)
        {
            envState = env;
            machine = env.machine;
        }

        public virtual void EnterAction() { Debug.Log($"{envState}"); }
        public virtual void ExitAction() { }
        public virtual void Update() { }
    }


    public class IdleState : ActionState
    {
        public IdleState(EnvironmentState env) : base(env) { }

    }

    public class WalkState : ActionState
    {
        public WalkState(EnvironmentState env) : base(env) { }
    }

    public class JumpState : ActionState
    {
        public JumpState(EnvironmentState env) : base(env) { }
    }

    public class SwimState : ActionState
    {
        public SwimState(EnvironmentState env) : base(env) { }
    }



    public class VehicleStateMachine
    {
        public Rigidbody rb;
        public VehicleMovementController controller;
        private EnvironmentState currentEnvironmentState;

        public event Action<EnvironmentState> OnEnvironmentChanged;
        public event Action<ActionState> OnActionChanged;

        public void NotifyActionChanged(ActionState newAction)
        {
            OnActionChanged?.Invoke(newAction);
        }

        private readonly Dictionary<EnvironmentType, EnvironmentState> availableEnvironments;

        public VehicleStateMachine(VehicleMovementController ctrl)
        {
            controller = ctrl;


            currentEnvironmentState = availableEnvironments[EnvironmentType.Land];

            currentEnvironmentState.EnterEnvironment();
        }

        public void ExitStateMachine()
        {
            currentEnvironmentState.ExitEnvironment();
        }

        private void HandleEnvironmentChanged(string envString)
        {
            if (Enum.TryParse<EnvironmentType>(envString, true, out var env))
            {
                if (availableEnvironments.TryGetValue(env, out var nextEnv))
                {
                    ChangeEnvironment(nextEnv);
                }
            }
            else
            {
                Debug.LogWarning($"Unknown environment: {envString}");
            }
        }

        private void ChangeEnvironment(EnvironmentState newEnv)
        {
            if (currentEnvironmentState == newEnv) return;

            currentEnvironmentState.ExitEnvironment();
            currentEnvironmentState = newEnv;
            currentEnvironmentState.EnterEnvironment();

            OnEnvironmentChanged?.Invoke(currentEnvironmentState);
            Debug.Log($"[StateMachine] Environment changed to: {newEnv.GetType().Name}");
        }

        public void Update()
        {
            currentEnvironmentState.Update();
        }

        public EnvironmentState GetCurrentEnvironment() => currentEnvironmentState;
        public ActionState GetCurrentAction() => currentEnvironmentState?.GetCurrentAction();
    }
    public abstract class VehicleState
    {
        protected EnvironmentState envState;
        protected PlayerStateMachine machine;


        protected VehicleState(EnvironmentState env)
        {
            envState = env;
            machine = env.machine;
        }

        public virtual void EnterAction() { Debug.Log($"{envState}"); }
        public virtual void ExitAction() { }
        public virtual void Update() { }
    }

	public class CarState : VehicleState
	{
		public CarState(EnvironmentState env) : base(env)
		{
		}
	}
	public class BoatState : VehicleState
	{
		public BoatState(EnvironmentState env) : base(env)
		{
		}
	}
	public class PlaneState : VehicleState
	{
		public PlaneState(EnvironmentState env) : base(env)
		{
		}
	}
	public class TrainState : VehicleState
	{
		public TrainState(EnvironmentState env) : base(env)
		{
		}
	}

}


