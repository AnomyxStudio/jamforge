using System;

namespace JamForge.StateMachine
{
    public class State : IState
    {
        #region IState

        public float ElapsedTime { get; private set; }

        public bool Active { get; private set; }

        public event Action<IState> StateEnterEvent;

        public event Action<IState> StateExitEvent;

        public event Action<IState, float> StateUpdateEvent;

        public void Enter()
        {
            StateEnterEvent?.Invoke(this);
            Active = true;
            ElapsedTime = 0f;

            OnEnter();
        }

        public void Update(float deltaTime)
        {
            OnUpdate(deltaTime);

            StateUpdateEvent?.Invoke(this, deltaTime);
            ElapsedTime += deltaTime;
        }

        public void Exit()
        {
            OnExit();

            Active = false;
            StateExitEvent?.Invoke(this);
        }

        protected virtual void OnEnter() { }

        protected virtual void OnExit() { }

        protected virtual void OnUpdate(float deltaTime) { }

        #endregion
    }
}
