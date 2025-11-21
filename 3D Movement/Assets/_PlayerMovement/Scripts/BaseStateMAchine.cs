using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;


namespace Movement.Assets._PlayerMovement.Scripts
{
    public interface IStateMachine
    {
        void NotifyActionChanged();
    }
    public interface IStateAction
    {
        void EnterAction();
        void ExitAction();
        void Update();
    }

    public abstract class BaseStateMachine<TController, TActionState> : IStateMachine
        where TActionState : class
    {
        public Rigidbody rb;
        public TController controller;
        protected TActionState currentState;

        public event Action<TActionState> OnActionChanged;

        public void SetState(TActionState newState)
        {
            if (currentState == newState) return;

          if (currentState is IStateAction oldAction)
            oldAction.ExitAction();

        currentState = newState;

        if (currentState is IStateAction newAction)
            newAction.EnterAction();

            OnActionChanged?.Invoke(currentState);
        }

        public virtual void NotifyActionChanged()
        {
            OnActionChanged?.Invoke(currentState);
        }

        public virtual void Update()
        {
            if (currentState is IStateAction action)
                action.Update();
        }

    }

}
